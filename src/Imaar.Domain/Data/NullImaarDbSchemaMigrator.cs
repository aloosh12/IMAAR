using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Imaar.Data;

/* This is used if database provider does't define
 * IImaarDbSchemaMigrator implementation.
 */
public class NullImaarDbSchemaMigrator : IImaarDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
