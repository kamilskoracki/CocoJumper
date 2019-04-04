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
    }
}