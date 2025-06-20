using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.FurnishingLevels
{
    public class FurnishingLevel : FurnishingLevelBase
    {
        //<suite-custom-code-autogenerated>
        protected FurnishingLevel()
        {

        }

        public FurnishingLevel(Guid id, string name, bool isActive, int? order = null)
            : base(id, name, isActive, order)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
    }
}