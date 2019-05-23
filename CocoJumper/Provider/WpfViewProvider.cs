using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;

namespace CocoJumper.Provider
{
    public class WpfViewProvider : IWpfViewProvider
    {
        private readonly IWpfTextView _wpfTextView;

        public WpfViewProvider(IWpfTextView wpfTextView)
        {
            this._wpfTextView = wpfTextView ?? throw new ArgumentNullException(
                              $"{nameof(WpfViewProvider)} in {nameof(WpfViewProvider)}, {nameof(wpfTextView)} was null");
        }

        public IEnumerable<LineData> GetCurrentRenderedText()
        {
            foreach (ITextViewLine item in _wpfTextView.TextViewLines)
            {
                yield return new LineData
                {
                    Start = item.Start.Position,
                    Data = _wpfTextView.TextSnapshot.GetText(item.Start.Position, item.Length),
                    DataLength = item.Length
                };
            }
        }

        public void MoveCaretTo(int position)
        {
            if (position > _wpfTextView.TextSnapshot.Length)
                position = _wpfTextView.TextSnapshot.Length;
            SnapshotPoint snapshotPoint = new SnapshotPoint(_wpfTextView.TextSnapshot, position);
            if (_wpfTextView.Selection.IsActive)
            {
                _wpfTextView.Selection.Clear();
            }
            _wpfTextView.Caret.MoveTo(snapshotPoint);
        }

        public void SelectFromTo(int from, int to)
        {
            if (from < 0)
                from = 0;
            if (to < 0)
                to = 0;
            if (from > _wpfTextView.TextSnapshot.Length)
                from = _wpfTextView.TextSnapshot.Length;
            if (to > _wpfTextView.TextSnapshot.Length)
                to = _wpfTextView.TextSnapshot.Length;
            SnapshotPoint snapshotPointFrom = new SnapshotPoint(_wpfTextView.TextSnapshot, from);
            SnapshotPoint snapshotPointTo = new SnapshotPoint(_wpfTextView.TextSnapshot, to);
            SnapshotSpan destinationSnapshotSpan = from > to ? new SnapshotSpan(snapshotPointTo, snapshotPointFrom) : new SnapshotSpan(snapshotPointFrom, snapshotPointTo);
            
            if (_wpfTextView.Selection.IsActive)
            {
                _wpfTextView.Selection.Clear();
            }
            _wpfTextView.Selection.Select(destinationSnapshotSpan, from > to);
        }

        public int GetCaretPosition()
        {
            return _wpfTextView.Caret.Position.BufferPosition.Position;
        }
    }
}