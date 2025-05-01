using Xunit;

namespace Imaar.EntityFrameworkCore;

[CollectionDefinition(ImaarTestConsts.CollectionDefinitionName)]
public class ImaarEntityFrameworkCoreCollection : ICollectionFixture<ImaarEntityFrameworkCoreFixture>
{

}
