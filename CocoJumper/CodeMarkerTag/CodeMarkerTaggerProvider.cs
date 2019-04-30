using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CocoJumper.CodeMarkerTag
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(IntraTextAdornmentTag))]
    public class CodeSearcherTaggerProvider : IViewTaggerProvider
    {
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public CodeSearcherTaggerProvider(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer != textView.TextBuffer)
                return null;

            return new CodeMarkerTagger(textView, buffer, _eventAggregator) as ITagger<T>;
        }
    }
}