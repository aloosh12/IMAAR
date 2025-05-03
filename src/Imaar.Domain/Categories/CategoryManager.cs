using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Categories
{
    public abstract class CategoryManagerBase : DomainService
    {
        protected ICategoryRepository _categoryRepository;

        public CategoryManagerBase(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public virtual async Task<Category> CreateAsync(
        string title, string icon, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(icon, nameof(icon));

            var category = new Category(
             GuidGenerator.Create(),
             title, icon, order, isActive
             );

            return await _categoryRepository.InsertAsync(category);
        }

        public virtual async Task<Category> UpdateAsync(
            Guid id,
            string title, string icon, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(icon, nameof(icon));

            var category = await _categoryRepository.GetAsync(id);

            category.Title = title;
            category.Icon = icon;
            category.Order = order;
            category.IsActive = isActive;

            category.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _categoryRepository.UpdateAsync(category);
        }

    }
}