using Imaar.ImaarServices;
using Imaar.Vacancies;
using Imaar.Stories;

using System;
using System.Collections.Generic;

namespace Imaar.Medias
{
    public abstract class MediaWithNavigationPropertiesBase
    {
        public Media Media { get; set; } = null!;

        public ImaarService ImaarService { get; set; } = null!;
        public Vacancy Vacancy { get; set; } = null!;
        public Story Story { get; set; } = null!;
        

        
    }
}