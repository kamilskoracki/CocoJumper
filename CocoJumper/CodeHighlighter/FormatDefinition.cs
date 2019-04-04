using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace CocoJumper.CodeHighlighter
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/DocumentedCodeFormatDefinition")]
    [UserVisible(true)]
    internal class FormatDefinition : MarkerFormatDefinition
    {
        public FormatDefinition()
        {
            var orange = Brushes.Orange.Clone();
            orange.Opacity = 0.25;
            Fill = orange;
            Border = new Pen(Brushes.Gray, 1.0);
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }

    internal class x : ClassificationFormatDefinition
    {
    }
}