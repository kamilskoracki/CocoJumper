using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace CocoJumper.CodeHighlighterTag
{
    [Export]
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(CodeHighlighterTag))]
    public class CodeHighlighterTaggerProvider : IViewTaggerProvider
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public CodeHighlighterTaggerProvider(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView.TextBuffer != buffer)
                return null;

            return new CodeHighlighterTagger(textView, buffer, _eventAggregator) as ITagger<T>;
        }
    }
}