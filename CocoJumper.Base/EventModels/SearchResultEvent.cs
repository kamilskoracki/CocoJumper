using CocoJumper.Base.Events;
using System.Collections.Generic;

namespace CocoJumper.Base.EventModels
{
    public class SearchResultEvent
    {
        public SearchResultEvent()
        {
            SearchEvents = new List<SearchEvent>();
        }

        public bool IsHighlightDisabled { get; set; }
        public List<SearchEvent> SearchEvents { get; set; }
    }
}