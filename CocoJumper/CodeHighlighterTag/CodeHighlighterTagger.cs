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
        private readonly ITextView _textView;
        private readonly ITextBuffer _buffer;
        private readonly Dictionary<int, ITagSpan<CodeHighlighterTag>> _taggers = new Dictionary<int, ITagSpan<CodeHighlighterTag>>();

        public CodeHighlighterTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
        {
            _textView = textView;
            _buffer = buffer;
            eventAggregator.AddListener(new DelegateListener<SearchResultEvent>(OnSearch), true);
            eventAggregator.AddListener(new DelegateListener<ExitEvent>(OnExit), true);
            _textView.LayoutChanged += ViewLayoutChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // If a new snapshot wasn't generated, then skip this layout
            if (e.NewViewState.EditSnapshot != e.OldViewState.EditSnapshot)
            {
               // UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        public IEnumerable<ITagSpan<CodeHighlighterTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _taggers.Values;
        }

        private void OnExit(ExitEvent e)
        {
            _taggers.Clear();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }

        private void OnSearch(SearchResultEvent e)
        {
            //TODO - is there a way to update span?
            _taggers.Clear();
            foreach (SearchEvent eSearchEvent in e.SearchEvents)
            {
                if (!_taggers.ContainsKey(eSearchEvent.StartPosition))
                {
                    _taggers.Add(eSearchEvent.StartPosition,
                        new TagSpan<CodeHighlighterTag>(
                            new SnapshotSpan(_buffer.CurrentSnapshot, eSearchEvent.StartPosition, eSearchEvent.Length),
                            new CodeHighlighterTag()
                        )
                    );
                }
            }

            List<int> keysToRemove = _taggers.Keys.Where(p => p != 0 && !e.SearchEvents.Exists(x => x.StartPosition == p)).ToList();
            foreach (int i in keysToRemove)
            {
                _taggers.Remove(i);
            }

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }
    }
}