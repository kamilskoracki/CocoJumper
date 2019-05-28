using CocoJumper.Base.Enum;
using CocoJumper.Base.Events;
using CocoJumper.Base.Logic;
using CocoJumper.Events;
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
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CocoJumperWordSearchCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4131;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("29fda481-672d-4ce9-9793-0bebf8b4c6c8");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;

        private readonly AsyncPackage _package;
        private readonly IVsTextManager _vsTextManager;
        private InputListener _inputListener;
        private ICocoJumperLogic _logic;

        /// <summary>
        /// Initializes a new instance of the <see cref="CocoJumperWordSearchCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CocoJumperWordSearchCommand(AsyncPackage package, OleMenuCommandService commandService,
            IVsTextManager textManager, IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IEventAggregator eventAggregator)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            _vsTextManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            _editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));
            eventAggregator.AddListener(new DelegateListener<ExitEvent>(OnExit), true);

            CommandID menuCommandID = new CommandID(CommandSet, CommandId);
            MenuCommand menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CocoJumperWordSearchCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return _package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsTextManager vsTextManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            IComponentModel componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(componentModel);
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            IEventAggregator eventAggregator = componentModel.GetService<IEventAggregator>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            Instance = new CocoJumperWordSearchCommand(package, commandService, vsTextManager, editor,
                eventAggregator);
        }

        private void CleanupLogicAndInputListener()
        {
            _logic?.Dispose();
            _inputListener?.Dispose();
            _logic = null;
            _inputListener = null;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView textView = _vsTextManager.GetActiveView();
            IWpfTextView wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(textView);
            CocoJumperCommandPackage cocoJumperCommandPackage = (CocoJumperCommandPackage)_package;

            CleanupLogicAndInputListener();
            WpfViewProvider renderer = new WpfViewProvider(wpfTextView);

            _logic = new CocoJumperLogic(renderer, cocoJumperCommandPackage);
            _inputListener = new InputListener(textView);
            _inputListener.KeyPressEvent += OnKeyboardAction;
            _logic.ActivateSearching(false, false, true);
            OnKeyboardAction(this, null, KeyEventType.KeyPress);
        }

        private void OnExit(ExitEvent e)
        {
            CleanupLogicAndInputListener();
        }

        private void OnKeyboardAction(object oSender, char? key, KeyEventType eventType)
        {
            _logic = _logic ?? throw new Exception($"{nameof(OnKeyboardAction)} in {nameof(CocoJumperWordSearchCommand)}, {nameof(_logic)} is null");
            if (_logic.KeyboardAction(key, eventType) == CocoJumperKeyboardActionResult.Finished)
            {
                CleanupLogicAndInputListener();
            }
        }
    }
}