using System;
using System.Collections.Generic;
using System.Text;
using Imaar.Localization;
using Volo.Abp.Application.Services;

namespace Imaar;

/* Inherit your application services from this class.
 */
public abstract class ImaarAppService : ApplicationService
{
    protected ImaarAppService()
    {
        LocalizationResource = typeof(ImaarResource);
    }
}
