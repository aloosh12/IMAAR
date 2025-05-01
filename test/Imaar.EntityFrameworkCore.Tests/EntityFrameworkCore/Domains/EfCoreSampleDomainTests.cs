using Imaar.Samples;
using Xunit;

namespace Imaar.EntityFrameworkCore.Domains;

[Collection(ImaarTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ImaarEntityFrameworkCoreTestModule>
{

}
