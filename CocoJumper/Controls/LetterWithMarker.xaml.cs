namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for LetterWithMarker.xaml
    /// </summary>
    public partial class LetterWithMarker
    {
        private const int FontSizeFactor = -5;
        private const int HeightFactor = -4;

        public LetterWithMarker(string text, double lineHeight)
        {
            InitializeComponent();

            MarkerText = text;
            MarkerFontSize = lineHeight + FontSizeFactor;
            MarkerHeight = lineHeight + HeightFactor;
        }

        public double MarkerFontSize { get; set; }
        public double MarkerHeight { get; set; }
        public string MarkerText { get; set; }
    }
}