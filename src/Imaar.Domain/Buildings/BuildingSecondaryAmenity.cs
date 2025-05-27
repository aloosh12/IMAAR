using System;
using Volo.Abp.Domain.Entities;

namespace Imaar.Buildings
{
    public class BuildingSecondaryAmenity : Entity
    {

        public Guid BuildingId { get; protected set; }

        public Guid SecondaryAmenityId { get; protected set; }

        private BuildingSecondaryAmenity()
        {

        }

        public BuildingSecondaryAmenity(Guid buildingId, Guid secondaryAmenityId)
        {
            BuildingId = buildingId;
            SecondaryAmenityId = secondaryAmenityId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    BuildingId,
                    SecondaryAmenityId
                };
        }
    }
}