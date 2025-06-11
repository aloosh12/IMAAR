using Imaar.Buildings;
using Imaar.BuildingFacades;
using Imaar.FurnishingLevels;
using Imaar.Regions;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Imaar.Buildings
{
    public abstract class BuildingBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string MainTitle { get; set; }

        [NotNull]
        public virtual string Description { get; set; }

        [NotNull]
        public virtual string Price { get; set; }

        [NotNull]
        public virtual string BuildingArea { get; set; }

        [NotNull]
        public virtual string NumberOfRooms { get; set; }

        [NotNull]
        public virtual string NumberOfBaths { get; set; }

        [NotNull]
        public virtual string FloorNo { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }

        [NotNull]
        public virtual string PhoneNumber { get; set; }

        public virtual int ViewCounter { get; set; }

        public virtual int OrderCounter { get; set; }
        public Guid RegionId { get; set; }
        public Guid FurnishingLevelId { get; set; }
        public Guid BuildingFacadeId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }
        public ICollection<BuildingMainAmenity> MainAmenities { get; private set; }
        public ICollection<BuildingSecondaryAmenity> SecondaryAmenities { get; private set; }

        protected BuildingBase()
        {

        }

        public BuildingBase(Guid id, Guid regionId, Guid furnishingLevelId, Guid buildingFacadeId, Guid serviceTypeId, Guid userProfileId, string mainTitle, string description, string price, string buildingArea, string numberOfRooms, string numberOfBaths, string floorNo, string phoneNumber, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null)
        {

            Id = id;
            Check.NotNull(mainTitle, nameof(mainTitle));
            Check.NotNull(description, nameof(description));
            Check.NotNull(price, nameof(price));
            Check.NotNull(buildingArea, nameof(buildingArea));
            Check.NotNull(numberOfRooms, nameof(numberOfRooms));
            Check.NotNull(numberOfBaths, nameof(numberOfBaths));
            Check.NotNull(floorNo, nameof(floorNo));
            Check.NotNull(phoneNumber, nameof(phoneNumber));
            MainTitle = mainTitle;
            Description = description;
            Price = price;
            BuildingArea = buildingArea;
            NumberOfRooms = numberOfRooms;
            NumberOfBaths = numberOfBaths;
            FloorNo = floorNo;
            PhoneNumber = phoneNumber;
            ViewCounter = viewCounter;
            OrderCounter = orderCounter;
            Latitude = latitude;
            Longitude = longitude;
            RegionId = regionId;
            FurnishingLevelId = furnishingLevelId;
            BuildingFacadeId = buildingFacadeId;
            ServiceTypeId = serviceTypeId;
            UserProfileId = userProfileId;
            MainAmenities = new Collection<BuildingMainAmenity>();
            SecondaryAmenities = new Collection<BuildingSecondaryAmenity>();
        }
        public virtual void AddMainAmenity(Guid mainAmenityId)
        {
            Check.NotNull(mainAmenityId, nameof(mainAmenityId));

            if (IsInMainAmenities(mainAmenityId))
            {
                return;
            }

            MainAmenities.Add(new BuildingMainAmenity(Id, mainAmenityId));
        }

        public virtual void RemoveMainAmenity(Guid mainAmenityId)
        {
            Check.NotNull(mainAmenityId, nameof(mainAmenityId));

            if (!IsInMainAmenities(mainAmenityId))
            {
                return;
            }

            MainAmenities.RemoveAll(x => x.MainAmenityId == mainAmenityId);
        }

        public virtual void RemoveAllMainAmenitiesExceptGivenIds(List<Guid> mainAmenityIds)
        {
            Check.NotNullOrEmpty(mainAmenityIds, nameof(mainAmenityIds));

            MainAmenities.RemoveAll(x => !mainAmenityIds.Contains(x.MainAmenityId));
        }

        public virtual void RemoveAllMainAmenities()
        {
            MainAmenities.RemoveAll(x => x.BuildingId == Id);
        }

        private bool IsInMainAmenities(Guid mainAmenityId)
        {
            return MainAmenities.Any(x => x.MainAmenityId == mainAmenityId);
        }

        public virtual void AddSecondaryAmenity(Guid secondaryAmenityId)
        {
            Check.NotNull(secondaryAmenityId, nameof(secondaryAmenityId));

            if (IsInSecondaryAmenities(secondaryAmenityId))
            {
                return;
            }

            SecondaryAmenities.Add(new BuildingSecondaryAmenity(Id, secondaryAmenityId));
        }

        public virtual void RemoveSecondaryAmenity(Guid secondaryAmenityId)
        {
            Check.NotNull(secondaryAmenityId, nameof(secondaryAmenityId));

            if (!IsInSecondaryAmenities(secondaryAmenityId))
            {
                return;
            }

            SecondaryAmenities.RemoveAll(x => x.SecondaryAmenityId == secondaryAmenityId);
        }

        public virtual void RemoveAllSecondaryAmenitiesExceptGivenIds(List<Guid> secondaryAmenityIds)
        {
            Check.NotNullOrEmpty(secondaryAmenityIds, nameof(secondaryAmenityIds));

            SecondaryAmenities.RemoveAll(x => !secondaryAmenityIds.Contains(x.SecondaryAmenityId));
        }

        public virtual void RemoveAllSecondaryAmenities()
        {
            SecondaryAmenities.RemoveAll(x => x.BuildingId == Id);
        }

        private bool IsInSecondaryAmenities(Guid secondaryAmenityId)
        {
            return SecondaryAmenities.Any(x => x.SecondaryAmenityId == secondaryAmenityId);
        }
    }
}