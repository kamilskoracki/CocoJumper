using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace CocoJumper.CodeHighlighterTag
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/DocumentedCodeFormatDefinition")]
    [UserVisible(true)]
    internal class FormatDefinition : MarkerFormatDefinition
    {
        public FormatDefinition()
        {
            SolidColorBrush orange = Brushes.Orange.Clone();
            orange.Opacity = 0.25;
            Fill = orange;
            Border = new Pen(Brushes.Gray, 1.0);
            ZOrder = 5;
        }
    }
}