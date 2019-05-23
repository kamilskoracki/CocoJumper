using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;

namespace CocoJumper.Base.Logic
{
    public static class TaggerCommon
    {
        public static void InvokeTagsChanged(this ITagger<ITag> sender, EventHandler<SnapshotSpanEventArgs> tagsChangedEvent, ITextBuffer buffer)
        {
            tagsChangedEvent.Invoke(sender, new SnapshotSpanEventArgs(
                new SnapshotSpan(buffer.CurrentSnapshot, new Span(0, buffer.CurrentSnapshot.Length - 1))));
        }
    }
}