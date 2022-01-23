namespace Application.Commands.StoreEvents
{
    public sealed class NewCertificateHasBeenRegistered: StoreEvent
    {
        internal static readonly string ApplicationEventTypeName = "NewCertificateHasBeenRegistered";

        public string Number { get; set; }

        public NewCertificateHasBeenRegistered()
            : base(applicationEventType: ApplicationEventTypeName)
        {
        }
    }
}
