using CocoJumper.Base.Events;
using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;

namespace CocoJumper.CodeHighlighterTag
{
    internal class CodeHighlighterTagger : ITagger<CodeHighlighterTag>
    {
        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;

        private List<SearchEvent> _searchEvents;

        public CodeHighlighterTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
        {
            _textView = textView;
            _buffer = buffer;
            eventAggregator.AddListener(new DelegateListener<SearchResultEvent>(OnSearch), true);
            eventAggregator.AddListener(new DelegateListener<ExitEvent>(OnExit), true);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<CodeHighlighterTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (_searchEvents == null)
                yield break;
            foreach (SearchEvent searchEvent in _searchEvents)
            {
                yield return new TagSpan<CodeHighlighterTag>(
                    new SnapshotSpan(_buffer.CurrentSnapshot, searchEvent.StartPosition, searchEvent.Length),
                    new CodeHighlighterTag()
                );
            }
        }

        private void OnExit(ExitEvent e)
        {
            _searchEvents.Clear();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }

        private void OnSearch(SearchResultEvent e)
        {
            _searchEvents = e.SearchEvents;
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }
    }
}