using CocoJumper.Events;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.CodeHighlighter
{
    public class Tagger : TextMarkerTag
    {
        public Tagger() : base("MarkerFormatDefinition/DocumentedCodeFormatDefinition")
        {
        }
    }

    internal class CodeHighlighterTagger : ITagger<Tagger>
    {
        private readonly DelegateListener<ExitEvent> _exitListener;
        private readonly DelegateListener<SearchEvent> _listener;
        private readonly ITextView _textView;
        private ITextBuffer _buffer;
        private IEventAggregator _eventAggregator;
        private List<ITagSpan<Tagger>> _taggers = new List<ITagSpan<Tagger>>();

        public CodeHighlighterTagger(ITextView textView, ITextBuffer buffer, IEventAggregator eventAggregator)
        {
            _textView = textView;
            _buffer = buffer;
            _eventAggregator = eventAggregator;
            _listener = new DelegateListener<SearchEvent>(OnSearch);
            _eventAggregator.AddListener(_listener);
            _exitListener = new DelegateListener<ExitEvent>(OnExit);
            _eventAggregator.AddListener(_exitListener);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<Tagger>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _taggers;
        }

        private void OnExit(ExitEvent e)
        {
            _taggers.Clear();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(0, _buffer.CurrentSnapshot.Length - 1))));
        }

        private void OnSearch(SearchEvent e)
        {
            SnapshotSpan snapshotSpan = new SnapshotSpan(_buffer.CurrentSnapshot, e.StartPosition, e.Length);
            _taggers = _taggers
                .Except(_taggers
                    .Where(p => p.Span.Overlap(snapshotSpan) != null))
                .ToList();

            _taggers.Add(new TagSpan<Tagger>(snapshotSpan, new Tagger()));

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
                new SnapshotSpan(_buffer.CurrentSnapshot, new Span(e.StartPosition, e.Length))));
        }
    }
}