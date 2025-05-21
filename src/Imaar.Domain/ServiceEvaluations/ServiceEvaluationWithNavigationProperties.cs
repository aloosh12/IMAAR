using Imaar.UserProfiles;
using Imaar.ImaarServices;

using System;
using System.Collections.Generic;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationWithNavigationPropertiesBase
    {
        public ServiceEvaluation ServiceEvaluation { get; set; } = null!;

        public UserProfile Evaluator { get; set; } = null!;
        public ImaarService ImaarService { get; set; } = null!;
        

        
    }
}