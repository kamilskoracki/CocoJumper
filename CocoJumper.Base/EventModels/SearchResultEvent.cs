using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.Base.Events
{
    public class SearchResultEvent
    {
        public List<SearchEvent> searchEvents;
        public string SearchString;

        public SearchResultEvent()
        {
            searchEvents = new List<SearchEvent>();
        }
    }
}