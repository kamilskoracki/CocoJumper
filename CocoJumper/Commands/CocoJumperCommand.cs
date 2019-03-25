using CocoJumper.Base.Enum;
using CocoJumper.Extensions;
using CocoJumper.Listeners;
using CocoJumper.Logic;
using CocoJumper.Provider;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace CocoJumper.Commands
{
    internal class CocoJumperCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("29fda481-672d-4ce9-9793-0bebf8b4c6c8");
        private readonly IVsEditorAdaptersFactoryService editorAdaptersFactoryService;
        private readonly AsyncPackage package;
        private readonly IVsTextManager vsTextManager;
        private InputListener inputListener;
        private CocoJumperLogic logic;

        private CocoJumperCommand(AsyncPackage _package, OleMenuCommandService _commandService, IVsTextManager _textManager, IVsEditorAdaptersFactoryService _editorAdaptersFactoryService)
        {
            package = _package ?? throw new ArgumentNullException(nameof(_package));
            _commandService = _commandService ?? throw new ArgumentNullException(nameof(_commandService));
            vsTextManager = _textManager ?? throw new ArgumentNullException(nameof(_textManager));
            editorAdaptersFactoryService = _editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(_editorAdaptersFactoryService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            _commandService.AddCommand(menuItem);
        }

        public static CocoJumperCommand Instance
        {
            get;
            private set;
        }

        private IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return package;
            }
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsTextManager vsTextManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            IComponentModel componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(componentModel);
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            Instance = new CocoJumperCommand(package, commandService, vsTextManager, editor);
        }

        private void CleanupLogicAndInputListener()
        {
            logic?.Dispose();
            inputListener?.Dispose();
            logic = null;
            inputListener = null;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView textView = vsTextManager.GetActiveView();
            IWpfTextView wpfTextView = editorAdaptersFactoryService.GetWpfTextView(textView);

            CleanupLogicAndInputListener();
            var renderer = new WpfViewProvider(wpfTextView);
            logic = new CocoJumperLogic(renderer);
            inputListener = new InputListener(textView);
            inputListener.KeyPressEvent += OnKeyboardAction;
            logic.ActivateSearching();

            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "CocoJumperCommand";

            //VsShellUtilities.ShowMessageBox(
            //    this.package,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void OnKeyboardAction(object oSender, char? key, KeyEventType eventType)
        {
            logic = logic ?? throw new Exception($"{nameof(OnKeyboardAction)} in {nameof(CocoJumperCommand)}, {nameof(logic)} is null");
            if (logic.KeyboardAction(key, eventType) == CocoJumperKeyboardActionResult.Finished)
            {
                CleanupLogicAndInputListener();
            }
        }
    }
}