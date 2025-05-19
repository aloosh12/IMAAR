using Imaar.UserProfiles;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.UserEvalauations
{
    public abstract class UserEvalauationWithNavigationPropertiesBase
    {
        public UserEvalauation UserEvalauation { get; set; } = null!;

        public UserProfile Evaluatord { get; set; } = null!;
        public UserProfile EvaluatedPerson { get; set; } = null!;
        

        
    }
}