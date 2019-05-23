using CocoJumper.Models;

namespace CocoJumper.Controls
{
    public partial class SearcherWithMarker
    {
        public SearcherWithMarker(MarkerViewModel markerViewModel)
        {
            InitializeComponent();
            DataContext = markerViewModel;
        }
    }
}