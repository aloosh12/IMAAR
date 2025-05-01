using Volo.Abp.Modularity;

namespace Imaar;

[DependsOn(
    typeof(ImaarApplicationModule),
    typeof(ImaarDomainTestModule)
)]
public class ImaarApplicationTestModule : AbpModule
{

}
