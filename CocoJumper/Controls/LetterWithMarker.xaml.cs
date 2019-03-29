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

            letterContent.Text = text;
            FontSize = lineHeight + FontSizeFactor;
            Height = lineHeight + HeightFactor;
        }
    }
}