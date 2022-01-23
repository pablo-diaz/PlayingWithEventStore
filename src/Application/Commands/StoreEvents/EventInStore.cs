using Newtonsoft.Json;

namespace Application.Commands.StoreEvents
{
    public abstract class EventInStore
    {
        internal string ApplicationEventType { get; }

        protected EventInStore(string applicationEventType)
        {
            ApplicationEventType = applicationEventType;
        }

        internal string Serialize() =>
            JsonConvert.SerializeObject(this);

        internal static T Deserialize<T>(string serializedData) where T: EventInStore, new() =>
            JsonConvert.DeserializeObject<T>(serializedData);
    }
}
