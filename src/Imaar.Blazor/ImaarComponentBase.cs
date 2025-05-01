using Imaar.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Imaar.Blazor;

public abstract class ImaarComponentBase : AbpComponentBase
{
    protected ImaarComponentBase()
    {
        LocalizationResource = typeof(ImaarResource);
    }
}
