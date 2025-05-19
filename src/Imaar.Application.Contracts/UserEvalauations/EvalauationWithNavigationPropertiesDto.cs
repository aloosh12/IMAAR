using Imaar.UserProfiles;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.UserEvalauations
{
    public abstract class UserEvalauationWithNavigationPropertiesDtoBase
    {
        public UserEvalauationDto UserEvalauation { get; set; } = null!;

        public UserProfileDto Evaluatord { get; set; } = null!;
        public UserProfileDto EvaluatedPerson { get; set; } = null!;

    }
}