using System.ComponentModel.Composition;

namespace CocoJumper.Events
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IEventAggregator))]
    public class EventAggregatorExport : EventAggregator
    {
        public EventAggregatorExport() : base()
        {
        }
    }
}