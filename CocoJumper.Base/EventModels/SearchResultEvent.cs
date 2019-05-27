using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.Base.Events
{
    public class SearchResultEvent
    {
        public List<SearchEvent> SearchEvents;
        public bool IsHighlightDisabled;

        public SearchResultEvent()
        {
            SearchEvents = new List<SearchEvent>();
        }
    }
}