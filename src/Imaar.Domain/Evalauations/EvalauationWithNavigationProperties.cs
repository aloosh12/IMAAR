using Imaar.UserProfiles;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.Evalauations
{
    public abstract class EvalauationWithNavigationPropertiesBase
    {
        public Evalauation Evalauation { get; set; } = null!;

        public UserProfile Evaluatord { get; set; } = null!;
        public UserProfile EvaluatedPerson { get; set; } = null!;
        

        
    }
}