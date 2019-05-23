using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace CocoJumper.Commands
{
    public class CocoJumperOptions : DialogPage
    {
        private const string GeneralCategory = "General";

        [Category(GeneralCategory)]
        [DisplayName("Limit results")]
        [Description("Limts results that are rendered on one page.")]
        [DefaultValue(50)]
        public int LimitResults { get; set; }

        [Category(GeneralCategory)]
        [DisplayName("Timer interval(ms)")]
        [Description("Determines how much ms must pass before rendering components, counting from last key press.")]
        [DefaultValue(250)]
        public int TimerInterval { get; set; }

        [Category(GeneralCategory)]
        [DisplayName("Automatically exit after(ms)")]
        [Description("Determines how much ms must pass before logic will automaticly exit.")]
        [DefaultValue(5000)]
        public int AutomaticallyExitInterval { get; set; }

        public override void SaveSettingsToStorage()
        {
            if (AutomaticallyExitInterval <= 0)
                AutomaticallyExitInterval = 5000;
            if (TimerInterval <= 0)
                TimerInterval = 250;
            if (LimitResults < 0)
                LimitResults = 50;
            base.SaveSettingsToStorage();

            Saved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Saved;
    }
}