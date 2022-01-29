using System;

namespace Application.Commands.StoreEvents
{
    public sealed class CertificateHasBeenSigned : EventInStore
    {
        public static readonly string ApplicationEventTypeName = "CertificateHasBeenSigned";

        public DateTimeOffset SignedAt { get; set; }
        public string SignedBy { get; set; }

        public CertificateHasBeenSigned()
            : base(applicationEventType: ApplicationEventTypeName)
        {
        }
    }
}
