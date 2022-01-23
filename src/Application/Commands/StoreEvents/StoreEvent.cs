using Newtonsoft.Json;

namespace Application.Commands.StoreEvents
{
    public abstract class StoreEvent
    {
        internal string ApplicationEventType { get; }

        protected StoreEvent(string applicationEventType)
        {
            ApplicationEventType = applicationEventType;
        }

        internal string Serialize() =>
            JsonConvert.SerializeObject(this);

        internal T Deserialize<T>(string serializedData) where T: StoreEvent, new() =>
            JsonConvert.DeserializeObject<T>(serializedData);
    }
}
