using System;

using Newtonsoft.Json;

namespace WebAPI.Utils
{
    public class Envelope<T>
    {
        [JsonProperty("result")]
        public T Result { get; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; }

        [JsonProperty("timeGenerated")]
        public DateTime TimeGenerated { get; }

        protected internal Envelope(T result, string errorMessage)
        {
            Result = result;
            ErrorMessage = errorMessage;
            TimeGenerated = DateTime.UtcNow;
        }
    }

    public sealed class Envelope : Envelope<string>
    {
        private Envelope(string errorMessage)
            : base(null, errorMessage)
        {
        }

        public static Envelope<T> Ok<T>(T result) =>
            new Envelope<T>(result, null);

        public static Envelope Ok() =>
            new Envelope(null);

        public static Envelope Error(string errorMessage) =>
            new Envelope(errorMessage);
    }
}
