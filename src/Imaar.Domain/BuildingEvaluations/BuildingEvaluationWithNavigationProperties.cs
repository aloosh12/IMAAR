using Imaar.UserProfiles;
using Imaar.Buildings;

using System;
using System.Collections.Generic;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationWithNavigationPropertiesBase
    {
        public BuildingEvaluation BuildingEvaluation { get; set; } = null!;

        public UserProfile Evaluator { get; set; } = null!;
        public Building Building { get; set; } = null!;
        

        
    }
}