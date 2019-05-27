using CocoJumper.Provider;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace CocoJumper.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(CocoJumperOptions), "Environment\\Keyboard", "CocoJumper", 0, 0, true, ProvidesLocalizedCategoryName = false)]
    [Guid(PackageGuidString)]
    public sealed class CocoJumperCommandPackage : AsyncPackage
    {
        public const string PackageGuidString = "cd8f3565-1f57-4c09-b5c1-01fe488ab080";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            MefProvider.ComponentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            await CocoJumperMultiSearchCommand.InitializeAsync(this);
            await CocoJumperSingleSearchCommand.InitializeAsync(this);
            await CocoJumperSingleSearchHighlightCommand.InitializeAsync(this);
            await base.InitializeAsync(cancellationToken, progress);
        }

        public int LimitResults
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.LimitResults;
            }
        }

        public int TimerInterval
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.TimerInterval;
            }
        }

        public bool JumpAfterChoosedElement
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.JumpAfterChoosedElement;
            }
        }

        public int AutomaticallyExitInterval
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.AutomaticallyExitInterval;
            }
        }

        public bool DisableHighlightForMultiSearch
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.DisableHighlightForMultiSearch;
            }
        }

        public bool DisableHighlightForSingleHighlight
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.DisableHighlightForSingleHighlight;
            }
        }

        public bool DisableHighlightForSingleSearch
        {
            get
            {
                CocoJumperOptions page = (CocoJumperOptions)GetDialogPage(typeof(CocoJumperOptions));
                return page.DisableHighlightForSingleSearch;
            }
        }
    }
}