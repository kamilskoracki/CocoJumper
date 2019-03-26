using CocoJumper.Base.Enum;
using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using CocoJumper.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace CocoJumper.Provider
{
    public class WpfViewProvider : IWpfViewProvider
    {
        private readonly IAdornmentLayer adornmentLayer;
        private readonly Dictionary<ElementType, Type> elementTypes;
        private readonly IWpfTextView wpfTextView;

        public WpfViewProvider(IWpfTextView _wpfTextView)
        {
            wpfTextView = _wpfTextView ?? throw new ArgumentNullException(
                              $"{nameof(WpfViewProvider)} in {nameof(WpfViewProvider)}, {nameof(_wpfTextView)} was null");
            adornmentLayer = _wpfTextView.GetAdornmentLayer("CocoJumper");
            elementTypes = new Dictionary<ElementType, Type>
            {
                {
                    ElementType.LetterWithMarker,
                    typeof(LetterWithMarker)
                }
            };
        }

        public void ClearAllElementsByType(ElementType type)
        {
            for (int i = 0; i < adornmentLayer.Elements.Count; i++)
            {
                if (adornmentLayer.Elements[i].Adornment.GetType() != elementTypes[type])
                    continue;
                adornmentLayer.RemoveAdornment(adornmentLayer.Elements[i].Adornment);
                i--;
            }
        }

        public void Dispose()
        {
            adornmentLayer.RemoveAllAdornments();
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

        public void RenderControlByStringPosition(ElementType type, int stringStart, int length, string text)
        {
            IWpfTextViewLineCollection textViewLines = wpfTextView.TextViewLines;

            var kk = GetCurrentRenderedText().ToList();

            var span = new SnapshotSpan(wpfTextView.TextSnapshot, Span.FromBounds(stringStart, stringStart + length));

            Geometry g = textViewLines.GetMarkerGeometry(span);
            if (g == null) return;

            LetterWithMarker letterReference = new LetterWithMarker(text, wpfTextView.LineHeight);
            Canvas.SetLeft(letterReference, g.Bounds.Left);
            Canvas.SetTop(letterReference, g.Bounds.Top);

            adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, letterReference, null);
        }
    }
}