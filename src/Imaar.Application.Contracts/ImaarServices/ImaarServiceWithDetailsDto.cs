using Imaar.Medias;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using static Imaar.Permissions.ImaarPermissions;

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceWithDetailsDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ServiceLocation { get; set; } = null!;
        public string ServiceNumber { get; set; } = null!;
        public string DefaultMedia { get; set; } = null!;
        public DateOnly DateOfPublish { get; set; }
        public int Price { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }

        public List<MediaDto> Medias { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

    }
}