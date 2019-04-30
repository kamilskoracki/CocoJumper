using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Formatting;

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
            SnapshotPoint snapshotPoint = new SnapshotPoint(_wpfTextView.TextSnapshot, position);
            if (_wpfTextView.Selection.IsActive)
            {
                _wpfTextView.Selection.Clear();
            }
            _wpfTextView.Caret.MoveTo(snapshotPoint);
        }

        public int GetCaretPosition()
        {
            return _wpfTextView.Caret.Position.BufferPosition.Position;
        }
    }
}