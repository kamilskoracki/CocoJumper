using CocoJumper.Models;

namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for LetterWithMarker.xaml
    /// </summary>
    public partial class LetterWithMarker
    {
        public LetterWithMarker(string text, double lineHeight)
        {
            InitializeComponent();

            MarkerText = text;
            MarkerFontSize = lineHeight + MarkerViewModel.FontSizeFactor;
            MarkerHeight = lineHeight + MarkerViewModel.HeightFactor;
        }

        public double MarkerFontSize { get; set; }
        public double MarkerHeight { get; set; }
        public string MarkerText { get; set; }
    }
}