using Volo.Abp.Settings;

namespace Imaar.Settings;

public class ImaarSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ImaarSettings.MySetting1));
    }
}
