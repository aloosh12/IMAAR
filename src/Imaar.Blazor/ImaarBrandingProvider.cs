using Microsoft.Extensions.Localization;
using Imaar.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Imaar.Blazor;

[Dependency(ReplaceServices = true)]
public class ImaarBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ImaarResource> _localizer;

    public ImaarBrandingProvider(IStringLocalizer<ImaarResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
