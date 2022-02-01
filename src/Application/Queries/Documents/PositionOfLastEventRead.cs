namespace Application.Queries.Documents
{
    public sealed class PositionOfLastEventRead : Document
    {
        private string _id;

        public override string Id { get => _id; set => _id = value; }

        public ulong Position { get; set; }
    }
}
