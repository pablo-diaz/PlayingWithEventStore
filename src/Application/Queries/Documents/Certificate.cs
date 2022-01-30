namespace Application.Queries.Documents
{
    public class Certificate: Document
    {
        private string _id;
        public override string Id { get => _id; set => _id = value; }

        public string Number { get; set; }
        public string Status { get; set; }
        public string SignedAt { get; set; }
        public string SignedBy { get; set; }
    }
}
