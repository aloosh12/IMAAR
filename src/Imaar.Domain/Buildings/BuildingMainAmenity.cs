using System;
using Volo.Abp.Domain.Entities;

namespace Imaar.Buildings
{
    public class BuildingMainAmenity : Entity
    {

        public Guid BuildingId { get; protected set; }

        public Guid MainAmenityId { get; protected set; }

        private BuildingMainAmenity()
        {

        }

        public BuildingMainAmenity(Guid buildingId, Guid mainAmenityId)
        {
            BuildingId = buildingId;
            MainAmenityId = mainAmenityId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    BuildingId,
                    MainAmenityId
                };
        }
    }
}