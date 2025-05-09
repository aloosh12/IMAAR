using Imaar.ServiceTypes;
using Imaar.UserProfiles;

using System;
using System.Collections.Generic;

namespace Imaar.ImaarServices
{
    public abstract class ImaarServiceWithNavigationPropertiesBase
    {
        public ImaarService ImaarService { get; set; } = null!;

        public ServiceType ServiceType { get; set; } = null!;
        public UserProfile UserProfile { get; set; } = null!;
        

        
    }
}