using CocoJumper.Base.Events;

namespace CocoJumper.Base.EventModels
{
    public class StartNewSearchEvent : SearchEvent
    {
        public int MatchNumber { get; set; }
        public string Text { get; set; }
    }
}