using Imaar.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Imaar.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ImaarController : AbpControllerBase
{
    protected ImaarController()
    {
        LocalizationResource = typeof(ImaarResource);
    }
}
