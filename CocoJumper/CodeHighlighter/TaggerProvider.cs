using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace CocoJumper.CodeHighlighter
{
    [Export]
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(Tagger))]
    public class TaggerProvider : IViewTaggerProvider
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public TaggerProvider(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        /// <summary>
        /// This method is called by VS to generate the tagger
        /// </summary>
        /// <param name="textView"> The text view we are creating a tagger for</param>
        /// <param name="buffer"> The buffer that the tagger will examine for instances of the current word</param>
        /// <returns> Returns a HighlightWordTagger instance</returns>
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            // Only provide highlighting on the top-level buffer
            if (textView.TextBuffer != buffer)
                return null;

            return new CodeHighlighterTagger(textView, buffer, _eventAggregator) as ITagger<T>;
        }
    }
}