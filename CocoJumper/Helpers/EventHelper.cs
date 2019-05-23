using CocoJumper.Base.EventModels;
using CocoJumper.Events;
using CocoJumper.Provider;

namespace CocoJumper.Helpers
{
    public class EventHelper
    {
        private static EventHelper _eventHelper;
        private readonly IEventAggregator _eventAggregator;

        public static EventHelper EventHelperInstance => _eventHelper ?? (_eventHelper = new EventHelper());

        private EventHelper()
        {
            _eventAggregator = MefProvider.ComponentModel.GetService<IEventAggregator>();
            _eventHelper = this;
        }

        public void RaiseEvent<T>() where T : new()
        {
            _eventAggregator.SendMessage<T>(new T());
        }

        public void RaiseEvent<T>(T obj) where T : new()
        {
            _eventAggregator.SendMessage<T>(obj);
        }

        public void RaiseStartNewSearchEvent(string searchText, int matchNumber, int startPosition)
        {
            RaiseEvent(new StartNewSearchEvent
            {
                StartPosition = startPosition,
                MatchNumber = matchNumber,
                Text = searchText
            });
        }
    }
}