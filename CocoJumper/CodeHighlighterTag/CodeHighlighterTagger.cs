using CocoJumper.Base.Events;
using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.CodeHighlighterTag
{
    internal class CodeHighlighterTagger : ITagger<CodeHighlighterTag>
    {
        private readonly DelegateListener<ExitEvent> _exitListener;
        private readonly DelegateListener<SearchResultEvent> _searchResultEvent;
        private readonly ITextView _textView;
        private ITextBuffer _buffer;
        private IEventAggregator _eventAggregator;
        private List<ITagSpan<CodeHighlighterTag>> _taggers = new List<ITagSpan<CodeHighlighterTag>>();

        public CodeHighlighterTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
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

        public IEnumerable<ITagSpan<CodeHighlighterTag>> GetTags(NormalizedSnapshotSpanCollection spans)
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
            _taggers = e.searchEvents.Select(p =>
                new TagSpan<CodeHighlighterTag>(
                    new SnapshotSpan(_buffer.CurrentSnapshot, p.StartPosition, p.Length),
                    new CodeHighlighterTag()
                ) as ITagSpan<CodeHighlighterTag>
            ).ToList();

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }
    }
}