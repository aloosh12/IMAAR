using Volo.Abp.Modularity;

namespace Imaar;

public abstract class ImaarApplicationTestBase<TStartupModule> : ImaarTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
