namespace Application.Queries.DTOs
{
    public sealed class CertificateInfoDTO
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string SignedAt { get; set; }
        public string SignedBy { get; set; }
    }
}
