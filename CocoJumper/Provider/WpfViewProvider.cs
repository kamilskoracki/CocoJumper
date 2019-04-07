using CocoJumper.Base.Enum;
using CocoJumper.Base.Events;
using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using CocoJumper.Controls;
using CocoJumper.Events;
using CocoJumper.Models;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace CocoJumper.Provider
{
    public class WpfViewProvider : IWpfViewProvider
    {
        private readonly MarkerViewModel _markerViewModel;
        private readonly IWpfTextView wpfTextView;
        private IEventAggregator _eventAggregator;

        public WpfViewProvider(IWpfTextView _wpfTextView)
        {
            _eventAggregator = MefProvider.ComponentModel.GetService<IEventAggregator>();
            _markerViewModel = new MarkerViewModel();
            wpfTextView = _wpfTextView ?? throw new ArgumentNullException(
                              $"{nameof(WpfViewProvider)} in {nameof(WpfViewProvider)}, {nameof(_wpfTextView)} was null");
        }

        public void ExitSearch()
        {
            _eventAggregator.SendMessage<ExitEvent>();
        }

        public IEnumerable<LineData> GetCurrentRenderedText()
        {
            //TODO - find better method to download all rendered lines, this may be slow
            foreach (var item in wpfTextView.TextViewLines)
            {
                yield return new LineData
                {
                    Start = item.Start.Position,
                    Data = wpfTextView.TextSnapshot.GetText(item.Start.Position, item.Length),
                    DataLength = item.Length
                };
            }
        }

        public void MoveCaretTo(int position)
        {
            SnapshotPoint snapshotPoint = new SnapshotPoint(wpfTextView.TextSnapshot, position);
            if (wpfTextView.Selection.IsActive)
            {
                wpfTextView.Selection.Clear();
            }
            wpfTextView.Caret.MoveTo(snapshotPoint);
        }

        public SearchEvent GetSearchEvent(ElementType type, int stringStart, int length, string text)
        {
            _markerViewModel.Update(text, wpfTextView.LineHeight, null);
            IWpfTextViewLineCollection textViewLines = wpfTextView.TextViewLines;

            SnapshotSpan span = new SnapshotSpan(wpfTextView.TextSnapshot, Span.FromBounds(stringStart, stringStart + length));

            Geometry g = textViewLines.GetMarkerGeometry(span);
            if (g == null) return null;
            return new SearchEvent
            {
                Length = length,
                StartPosition = stringStart
            };
        }

        public void RenderSearcherControlByCaretPosition(string searchText, int matchNumber)
        {
            //_markerViewModel.Update(searchText, wpfTextView.LineHeight, matchNumber);

            //if (_searcherControl != null) return;

            //SnapshotSpan span = new SnapshotSpan(wpfTextView.TextSnapshot,
            //    Span.FromBounds(wpfTextView.Caret.Position.BufferPosition.Position, wpfTextView.Caret.Position.BufferPosition.Position));

            //_searcherControl = new SearcherWithMarker(_markerViewModel);
            //Canvas.SetLeft(_searcherControl, wpfTextView.Caret.Left);
            //Canvas.SetTop(_searcherControl, wpfTextView.Caret.Top);
            //adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, _searcherControl, null);
        }
    }
}