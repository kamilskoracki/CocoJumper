using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for LetterWithMarker.xaml
    /// </summary>
    public partial class LetterWithMarker : UserControl
    {
        public LetterWithMarker(string text)
        {
            InitializeComponent();
            this.Content = text;
        }
    }
}
