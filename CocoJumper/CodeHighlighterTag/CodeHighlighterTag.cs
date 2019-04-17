using Microsoft.VisualStudio.Text.Tagging;

namespace CocoJumper.CodeHighlighterTag
{
    public class CodeHighlighterTag : TextMarkerTag
    {
        public CodeHighlighterTag() : base("MarkerFormatDefinition/DocumentedCodeFormatDefinition")
        {
        }
    }
}