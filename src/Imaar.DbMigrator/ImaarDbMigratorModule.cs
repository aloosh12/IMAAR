using Imaar.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Imaar.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ImaarEntityFrameworkCoreModule),
    typeof(ImaarApplicationContractsModule)
    )]
public class ImaarDbMigratorModule : AbpModule
{
}
