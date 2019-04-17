using CocoJumper.Models;

namespace CocoJumper.Controls
{
    public partial class LetterWithMarker
    {
        public LetterWithMarker(string text, double lineHeight)
        {
            MarkerText = text;
            MarkerFontSize = lineHeight + MarkerViewModel.FontSizeFactor;
            MarkerHeight = lineHeight + MarkerViewModel.HeightFactor;
            MarkerMinWidth = 10;
            InitializeComponent();
        }

        public double MarkerFontSize { get; set; }
        public double MarkerHeight { get; set; }
        public double MarkerMinWidth { get; set; }
        public string MarkerText { get; set; }
    }
}