namespace CocoJumper.Controls
{
    /// <summary>
    /// Interaction logic for LetterWithMarker.xaml
    /// </summary>
    public partial class LetterWithMarker
    {
        public LetterWithMarker(string text)
        {
            InitializeComponent();
            Content = text;
        }

        private void TextBox_TextChanged()
        {

        }
    }
}