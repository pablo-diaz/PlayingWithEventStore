using System;

using Domain.Common;

using CSharpFunctionalExtensions;

namespace Domain
{
    public sealed class Certificate: AggregateRoot
    {
        public string Number { get; }
        public CertificateStatus Status { get; internal set; } = CertificateStatus.DRAFT;
        public Maybe<Audit> SignedAudit { get; internal set; } = Maybe<Audit>.None;

        #region Ctors

        private Certificate(string number)
        {
            Id = Guid.NewGuid().ToString();
            Number = number;
        }

        #endregion

        #region Factories

        public static Result<Certificate> Create(string withNumber)
        {
            var result = ValidateNumber(withNumber);
            if (result.IsFailure)
                return Result.Failure<Certificate>(result.Error);

            return new Certificate(withNumber);
        }

        #endregion

        #region Behaviour

        public Result Sign(DateTimeOffset at, string by)
        {
            if (Status != CertificateStatus.DRAFT)
                return Result.Failure("Certificate cannot be Signed, because it is not in the right status");

            var signAuditResult = Audit.Create(at, by);
            if (signAuditResult.IsFailure)
                return signAuditResult;

            Status = CertificateStatus.SIGNED;
            SignedAudit = signAuditResult.Value;

            return Result.Success();
        }

        #endregion

        #region Validators

        private static Result ValidateNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Result.Failure("Certificate Number should be supplied");

            if(value.Length < 5 || value.Length > 50)
                return Result.Failure("Certificate Number should have at least 5 chars and at most 50 chars");

            return Result.Success();
        }

        #endregion
    }
}
