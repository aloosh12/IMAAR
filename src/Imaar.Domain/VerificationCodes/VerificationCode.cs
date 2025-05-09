using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.VerificationCodes
{
    public abstract class VerificationCodeBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string PhoneNumber { get; set; }

        public virtual int SecurityCode { get; set; }

        public virtual bool IsFinish { get; set; }

        protected VerificationCodeBase()
        {

        }

        public VerificationCodeBase(Guid id, string phoneNumber, int securityCode, bool isFinish)
        {

            Id = id;
            Check.NotNull(phoneNumber, nameof(phoneNumber));
            PhoneNumber = phoneNumber;
            SecurityCode = securityCode;
            IsFinish = isFinish;
        }

    }
}