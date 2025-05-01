using Volo.Abp.Modularity;

namespace Imaar;

[DependsOn(
    typeof(ImaarDomainModule),
    typeof(ImaarTestBaseModule)
)]
public class ImaarDomainTestModule : AbpModule
{

}
