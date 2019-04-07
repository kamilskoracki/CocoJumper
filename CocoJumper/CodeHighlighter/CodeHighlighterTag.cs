using Microsoft.VisualStudio.Text.Tagging;

namespace CocoJumper.CodeHighlighter
{
    public class CodeHighlighterTag : TextMarkerTag
    {
        public CodeHighlighterTag() : base("MarkerFormatDefinition/DocumentedCodeFormatDefinition")
        {
        }
    }
}