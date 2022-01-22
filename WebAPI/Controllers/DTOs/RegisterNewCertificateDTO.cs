using Application.Commands;

namespace WebAPI.Controllers.DTOs
{
    public sealed class RegisterNewCertificateDTO
    {
        public string Number { get; set; }

        public RegisterNewCertificateCommand ToCommand() =>
            new RegisterNewCertificateCommand(certNumber: this.Number);
    }
}
