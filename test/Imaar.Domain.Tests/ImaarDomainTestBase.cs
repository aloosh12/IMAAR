using Volo.Abp.Modularity;

namespace Imaar;

/* Inherit from this class for your domain layer tests. */
public abstract class ImaarDomainTestBase<TStartupModule> : ImaarTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
