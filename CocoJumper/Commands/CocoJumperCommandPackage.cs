using CocoJumper.Provider;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
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
            await CocoJumperMultiSearchCommand.InitializeAsync(this);
            await CocoJumperSingleSearchCommand.InitializeAsync(this);
            await base.InitializeAsync(cancellationToken, progress);

            MefProvider.ComponentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
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
    }

    public class CocoJumperOptions : DialogPage
    {
        private const string GeneralCategory = "General";

        [Category(GeneralCategory)]
        [DisplayName("Limit results")]
        [Description("Limts results that are rendered on one page.")]
        [DefaultValue(50)]
        public int LimitResults { get; set; }

        [Category(GeneralCategory)]
        [DisplayName("Timer interval")]
        [Description("Determines how much ms must pass before rendering components, counting from last key press.")]
        [DefaultValue(250)]
        public int TimerInterval { get; set; }

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            Saved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Saved;
    }
}