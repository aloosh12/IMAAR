using Imaar.UserProfiles;
using Imaar.Buildings;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationWithNavigationPropertiesDtoBase
    {
        public BuildingEvaluationDto BuildingEvaluation { get; set; } = null!;

        public UserProfileDto Evaluator { get; set; } = null!;
        public BuildingDto Building { get; set; } = null!;

    }
}