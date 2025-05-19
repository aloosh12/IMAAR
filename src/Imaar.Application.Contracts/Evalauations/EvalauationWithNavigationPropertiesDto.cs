using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.Evalauations
{
    public abstract class EvalauationWithNavigationPropertiesDtoBase
    {
        public EvalauationDto Evalauation { get; set; } = null!;

        public UserProfileDto Evaluatord { get; set; } = null!;
        public UserProfileDto EvaluatedPerson { get; set; } = null!;

    }
}