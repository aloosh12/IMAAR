using Imaar.UserProfiles;
using Imaar.Regions;
using Imaar.FurnishingLevels;
using Imaar.BuildingFacades;
using Imaar.ServiceTypes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Buildings
{
    public class Building : BuildingBase
    {
        //<suite-custom-code-autogenerated>
        protected Building()
        {

        }

        public Building(Guid id, Guid regionId, Guid furnishingLevelId, Guid buildingFacadeId, Guid serviceTypeId, Guid userProfileId, string mainTitle, string description, string price, string buildingArea, string numberOfRooms, string numberOfBaths, string floorNo, string phoneNumber, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null)
            : base(id, regionId, furnishingLevelId, buildingFacadeId, serviceTypeId, userProfileId, mainTitle, description, price, buildingArea, numberOfRooms, numberOfBaths, floorNo, phoneNumber, viewCounter, orderCounter, latitude, longitude)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
    }
}