using System.Threading.Tasks;

namespace Imaar.Data;

public interface IImaarDbSchemaMigrator
{
    Task MigrateAsync();
}
