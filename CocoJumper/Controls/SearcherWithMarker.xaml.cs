using CocoJumper.Models;

namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for SearcherWithMarker.xaml
    /// </summary>
    public partial class SearcherWithMarker
    {
        public SearcherWithMarker(MarkerViewModel markerViewModel)
        {
            InitializeComponent();
            DataContext = markerViewModel;
        }
    }
}