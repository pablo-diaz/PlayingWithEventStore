using System;

using Application.Commands;

namespace WebAPI.Controllers.DTOs
{
    public sealed class SignCertificateDTO
    {
        public string SignedBy { get; set; }

        public SignCertificateCommand ToCommand(string certificateId) =>
            new SignCertificateCommand(Guid.Parse(certificateId), this.SignedBy);
    }
}
