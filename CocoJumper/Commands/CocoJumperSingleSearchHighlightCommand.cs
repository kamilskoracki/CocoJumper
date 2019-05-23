﻿using CocoJumper.Base.Enum;
using CocoJumper.Base.Logic;
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
    internal sealed class CocoJumperSingleSearchHighlightCommand
    {
        public const int CommandId = 4130;

        public static readonly Guid CommandSet = new Guid("29fda481-672d-4ce9-9793-0bebf8b4c6c8");
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        private readonly AsyncPackage _package;
        private readonly IVsTextManager _vsTextManager;
        private InputListener _inputListener;
        private ICocoJumperLogic _logic;
        private readonly AsyncPackage package;

        private CocoJumperSingleSearchHighlightCommand(AsyncPackage package, OleMenuCommandService commandService, IVsTextManager textManager, IVsEditorAdaptersFactoryService editorAdaptersFactoryService)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _vsTextManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            this._editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));

            CommandID menuCommandId = new CommandID(CommandSet, CommandId);
            MenuCommand menuItem = new MenuCommand(Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static CocoJumperSingleSearchHighlightCommand Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
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
            Instance = new CocoJumperSingleSearchHighlightCommand(package, commandService, vsTextManager, editor);
        }

        private void CleanupLogicAndInputListener()
        {
            _logic?.Dispose();
            _inputListener?.Dispose();
            _logic = null;
            _inputListener = null;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView textView = _vsTextManager.GetActiveView();
            IWpfTextView wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(textView);
            CocoJumperCommandPackage cocoJumperCommandPackage = (CocoJumperCommandPackage)_package;

            CleanupLogicAndInputListener();
            WpfViewProvider renderer = new WpfViewProvider(wpfTextView);

            _logic = new CocoJumperLogic(renderer,
                cocoJumperCommandPackage.LimitResults,
                cocoJumperCommandPackage.TimerInterval,
                cocoJumperCommandPackage.AutomaticallyExitInterval,
                cocoJumperCommandPackage.JumpAfterChoosedElement);
            _inputListener = new InputListener(textView);
            _inputListener.KeyPressEvent += OnKeyboardAction;
            _logic.ActivateSearching(true, true);
        }

        private void OnKeyboardAction(object oSender, char? key, KeyEventType eventType)
        {
            _logic = _logic ?? throw new Exception($"{nameof(OnKeyboardAction)} in {nameof(CocoJumperMultiSearchCommand)}, {nameof(_logic)} is null");
            if (_logic.KeyboardAction(key, eventType) == CocoJumperKeyboardActionResult.Finished)
            {
                CleanupLogicAndInputListener();
            }
        }
    }
}