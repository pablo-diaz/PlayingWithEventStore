using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public sealed class EventsStorePosition
    {
        public readonly bool IsItFromStart;
        public readonly bool IsItFromEnd;
        public readonly Maybe<ulong> Position;

        private EventsStorePosition(Maybe<ulong> position, bool fromStart, bool fromEnd)
        {
            this.Position = position;
            this.IsItFromStart = fromStart;
            this.IsItFromEnd = fromEnd;
        }

        public static EventsStorePosition FromStart =>
            new EventsStorePosition(Maybe<ulong>.None, true, false);

        public static EventsStorePosition FromEnd =>
            new EventsStorePosition(Maybe<ulong>.None, false, true);

        public static EventsStorePosition From(ulong position) =>
            new EventsStorePosition(position, false, false);
    }
}
