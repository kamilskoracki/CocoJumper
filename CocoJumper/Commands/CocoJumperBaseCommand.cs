using CocoJumper.Base.Enum;
using CocoJumper.Base.Events;
using CocoJumper.Base.Exception;
using CocoJumper.Base.Logic;
using CocoJumper.Events;
using CocoJumper.Extensions;
using CocoJumper.Listeners;
using CocoJumper.Logic;
using CocoJumper.Provider;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace CocoJumper.Commands
{
    internal abstract class CocoJumperBaseCommand
    {
        protected ICocoJumperLogic Logic;
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        private readonly AsyncPackage _package;
        private readonly IVsTextManager _vsTextManager;
        private InputListener _inputListener;

        protected CocoJumperBaseCommand(AsyncPackage package,
            OleMenuCommandService commandService,
            IVsTextManager textManager,
            IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IEventAggregator eventAggregator,
            Guid commandSet,
            int commandId)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            _vsTextManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            _editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));

            eventAggregator.AddListener(new DelegateListener<ExitEvent>(OnExit), true);

            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            CommandID menuCommandID = new CommandID(commandSet, commandId);
            MenuCommand menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        protected IAsyncServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        protected static async
            Task<(OleMenuCommandService commandService,
                IVsTextManager vsTextManager,
                IVsEditorAdaptersFactoryService editor,
                IEventAggregator eventAggregator)> GetServicesAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsTextManager vsTextManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            IComponentModel componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel
                ?? throw new NotFoundException($"{nameof(IComponentModel)} not found.");
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            IEventAggregator eventAggregator = componentModel.GetService<IEventAggregator>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            return (commandService, vsTextManager, editor, eventAggregator);
        }

        protected void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView textView = _vsTextManager.GetActiveView();
            IWpfTextView wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(textView);
            CocoJumperCommandPackage cocoJumperCommandPackage = (CocoJumperCommandPackage)_package;

            CleanupLogicAndInputListener();
            WpfViewProvider renderer = new WpfViewProvider(wpfTextView);

            Logic = new CocoJumperLogic(renderer, cocoJumperCommandPackage);
            _inputListener = new InputListener(textView);
            _inputListener.KeyPressEvent += OnKeyboardAction;

            ExecutePostAction();
        }

        protected abstract void ExecutePostAction();

        protected void OnExit(ExitEvent e)
        {
            CleanupLogicAndInputListener();
        }

        protected void OnKeyboardAction(object oSender, char? key, KeyEventType eventType)
        {
            Logic = Logic ?? throw new Exception($"{nameof(OnKeyboardAction)} in {nameof(CocoJumperBaseCommand)}, {nameof(Logic)} is null");
            if (Logic.KeyboardAction(key, eventType) == CocoJumperKeyboardActionResult.Finished)
            {
                CleanupLogicAndInputListener();
            }
        }

        private void CleanupLogicAndInputListener()
        {
            Logic?.Dispose();
            _inputListener?.Dispose();
            Logic = null;
            _inputListener = null;
        }
    }
}