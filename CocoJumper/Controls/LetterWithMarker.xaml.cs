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

            letterContent.Text = text;
            FontSize = lineHeight - 5;
            Height = lineHeight;
        }
    }
}