using Imaar.ServiceTypes;

using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Imaar.Categories;

public class CategoryDeletedEventHandler : ILocalEventHandler<EntityDeletedEventData<Category>>, ITransientDependency
{
    private readonly IServiceTypeRepository _serviceTypeRepository;

    public CategoryDeletedEventHandler(IServiceTypeRepository serviceTypeRepository)
    {
        _serviceTypeRepository = serviceTypeRepository;

    }

    public async Task HandleEventAsync(EntityDeletedEventData<Category> eventData)
    {
        if (eventData.Entity is not ISoftDelete softDeletedEntity)
        {
            return;
        }

        if (!softDeletedEntity.IsDeleted)
        {
            return;
        }

        try
        {
            await _serviceTypeRepository.DeleteManyAsync(await _serviceTypeRepository.GetListByCategoryIdAsync(eventData.Entity.Id));

        }
        catch
        {
            //...
        }
    }
}