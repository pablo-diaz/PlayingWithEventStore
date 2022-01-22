using System;
using System.Collections.Generic;

using CSharpFunctionalExtensions;

namespace Domain
{
    public class Audit: ValueObject
    {
        public DateTimeOffset At { get; }
        public string By { get; }

        private Audit(DateTimeOffset at, string by)
        {
            At = at;
            By = by;
        }

        internal static Result<Audit> Create(DateTimeOffset at, string by) =>
            new Audit(at, by);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return At;
            yield return By;
        }
    }
}
