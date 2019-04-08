using CocoJumper.Base.Events;
using CocoJumper.Controls;
using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.CodeMarkerTag
{
    internal class CodeMarkerTagger : ITagger<IntraTextAdornmentTag>
    {
        private readonly DelegateListener<ExitEvent> _exitListener;
        private readonly DelegateListener<SearchResultEvent> _searchResultEvent;
        private readonly ITextView _textView;
        private ITextBuffer _buffer;
        private IEventAggregator _eventAggregator;
        private List<ITagSpan<IntraTextAdornmentTag>> _taggers = new List<ITagSpan<IntraTextAdornmentTag>>();

        public CodeMarkerTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
        {
            _textView = textView;
            _buffer = buffer;
            _eventAggregator = eventAggregator;
            _searchResultEvent = new DelegateListener<SearchResultEvent>(OnSearch);
            _eventAggregator.AddListener(_searchResultEvent);
            _exitListener = new DelegateListener<ExitEvent>(OnExit);
            _eventAggregator.AddListener(_exitListener);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _taggers;
        }

        private void OnExit(ExitEvent e)
        {
            _taggers.Clear();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }

        private void OnSearch(SearchResultEvent e)
        {
            double lineHeight = (_textView as IWpfTextView)?.LineHeight ?? 0;
            _taggers = e.searchEvents.Select(p =>
                new TagSpan<IntraTextAdornmentTag>(
                    span: new SnapshotSpan(_buffer.CurrentSnapshot, new Span(p.StartPosition, 0)),
                    tag: new IntraTextAdornmentTag(new LetterWithMarker(p.Letters, lineHeight), null, PositionAffinity.Predecessor)
                    ) as ITagSpan<IntraTextAdornmentTag>
                ).ToList();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }
    }
}