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
    public enum ElementType
    {
        LetterWithMarker
    }

    public class LineData
    {
        public int Start;
        public string Data;
        public int DataLength;
    }

    public class WpfViewProvider : IDisposable
    {
        private IAdornmentLayer adornmentLayer;
        private Dictionary<ElementType, Type> elementTypes;
        private IWpfTextView wpfTextView;

        public WpfViewProvider(IWpfTextView _wpfTextView)
        {
            wpfTextView = _wpfTextView ?? throw new ArgumentNullException($"{nameof(WpfViewProvider)} in {nameof(WpfViewProvider)}, {nameof(_wpfTextView)} was null");
            adornmentLayer = _wpfTextView.GetAdornmentLayer("CocoJumper");
            elementTypes = new Dictionary<ElementType, Type>
            {
                {
                    ElementType.LetterWithMarker,
                    typeof(LetterWithMarker)
                }
            };
        }

        private WpfViewProvider()
        {
        }

        public void ClearAllElementsByType(ElementType type)
        {
            for (int i = 0; i < this.adornmentLayer.Elements.Count; i++)
            {
                if (adornmentLayer.Elements[i].Adornment.GetType() != elementTypes[type])
                    continue;
                this.adornmentLayer.RemoveAdornment(this.adornmentLayer.Elements[i].Adornment);
                i--;
            }
        }

        public void Dispose()
        {
            this.adornmentLayer.RemoveAllAdornments();
        }

        public IEnumerable<LineData> GetCurrentRenderedText()
        {
            //TODO - find better method to download all rendered lines, this may be slow
            foreach (var item in this.wpfTextView.TextViewLines)
            {
                yield return new LineData
                {
                    Start = item.Start.Position,
                    Data = this.wpfTextView.TextSnapshot.GetText(item.Start.Position, item.Length),
                    DataLength = item.Length
                };
            }
        }

        public void MoveCaretTo(int possition)
        {
            SnapshotPoint snapshotPoint = new SnapshotPoint(wpfTextView.TextSnapshot, possition);
            if (wpfTextView.Selection.IsActive)
            {
                wpfTextView.Selection.Clear();
            }
            wpfTextView.Caret.MoveTo(snapshotPoint);
        }

        public void RenderControlByStringPossition(ElementType type, int stringStart, int length)
        {
            IWpfTextViewLineCollection textViewLines = this.wpfTextView.TextViewLines;

            var kk = GetCurrentRenderedText().ToList();

            var span = new SnapshotSpan(this.wpfTextView.TextSnapshot, Span.FromBounds(stringStart, stringStart + length));

            Geometry g = textViewLines.GetMarkerGeometry(span);
            if (g != null)
            {
                var letterReference = new LetterWithMarker();
                Canvas.SetLeft(letterReference, g.Bounds.Left);
                Canvas.SetTop(letterReference, g.Bounds.Top);

                this.adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, letterReference, null);
            }
        }
    }
}