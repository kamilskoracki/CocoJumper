using CocoJumper.Events;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using Task = System.Threading.Tasks.Task;

namespace CocoJumper.Commands
{
    internal class CocoJumperMultiSearchCommand : CocoJumperBaseCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("29fda481-672d-4ce9-9793-0bebf8b4c6c8");

        private CocoJumperMultiSearchCommand(AsyncPackage package, OleMenuCommandService commandService,
            IVsTextManager textManager, IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IEventAggregator eventAggregator)
            : base(package, commandService, textManager, editorAdaptersFactoryService,
                eventAggregator, CommandSet, CommandId)
        {
        }

        public static CocoJumperMultiSearchCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var (commandService, vsTextManager, editor, eventAggregator) = await GetServicesAsync(package);

            Instance = new CocoJumperMultiSearchCommand(package, commandService,
                vsTextManager, editor, eventAggregator);
        }

        protected override void ExecutePostAction()
        {
            Logic.ActivateSearching(false, false, false);
        }
    }
}