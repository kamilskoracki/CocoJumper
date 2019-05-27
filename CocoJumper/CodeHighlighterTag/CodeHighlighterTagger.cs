using CocoJumper.Base.Events;
using CocoJumper.Base.Logic;
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
        private readonly ITextBuffer _buffer;
        private readonly ITextView _textView;
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
            if (_searchEvents == null
                || _searchEvents.Count == 0 || !_textView.HasAggregateFocus || _textView.IsClosed)
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
            _searchEvents?.Clear();
            this.InvokeTagsChanged(TagsChanged, _buffer);
        }

        private void OnSearch(SearchResultEvent e)
        {
            if (!_textView.HasAggregateFocus || _textView.IsClosed)
                return;
            _searchEvents = e.IsHighlightDisabled ? null : e.SearchEvents;
            this.InvokeTagsChanged(TagsChanged, _buffer);
        }
    }
}