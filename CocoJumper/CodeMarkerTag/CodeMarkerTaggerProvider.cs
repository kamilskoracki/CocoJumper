using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;

namespace CocoJumper.CodeMarkerTag
{
    [Export]
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(IntraTextAdornmentTag))]
    public class CodeMarkerTaggerProvider : IViewTaggerProvider
    {
        private readonly IEventAggregator _eventAggregator;
#pragma warning disable 649 // "field never assigned to" -- field is set by MEF.
        [Import]
        internal IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService;
#pragma warning restore 649
        [ImportingConstructor]
        public CodeMarkerTaggerProvider(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer != textView.TextBuffer)
                return null;

            return new CodeMarkerTagger(textView, buffer, _eventAggregator) as ITagger<T>;
        }
    }
}