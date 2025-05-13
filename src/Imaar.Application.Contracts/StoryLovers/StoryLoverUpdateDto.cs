using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Imaar.StoryLovers
{
    public abstract class StoryLoverUpdateDtoBase : IHasConcurrencyStamp
    {

        public Guid UserProfileId { get; set; }
        public Guid StoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}