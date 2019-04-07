using System.Windows.Controls;

namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : UserControl
    {
        public Test(string txt)
        {
            InitializeComponent();
            this.Txt = txt;
            DataContext = this;
        }

        public string Txt { get; set; }
    }
}