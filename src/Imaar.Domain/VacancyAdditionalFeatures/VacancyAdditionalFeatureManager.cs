using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.VacancyAdditionalFeatures
{
    public abstract class VacancyAdditionalFeatureManagerBase : DomainService
    {
        protected IVacancyAdditionalFeatureRepository _vacancyAdditionalFeatureRepository;

        public VacancyAdditionalFeatureManagerBase(IVacancyAdditionalFeatureRepository vacancyAdditionalFeatureRepository)
        {
            _vacancyAdditionalFeatureRepository = vacancyAdditionalFeatureRepository;
        }

        public virtual async Task<VacancyAdditionalFeature> CreateAsync(
        string name, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var vacancyAdditionalFeature = new VacancyAdditionalFeature(
             GuidGenerator.Create(),
             name, order, isActive
             );

            return await _vacancyAdditionalFeatureRepository.InsertAsync(vacancyAdditionalFeature);
        }

        public virtual async Task<VacancyAdditionalFeature> UpdateAsync(
            Guid id,
            string name, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var vacancyAdditionalFeature = await _vacancyAdditionalFeatureRepository.GetAsync(id);

            vacancyAdditionalFeature.Name = name;
            vacancyAdditionalFeature.Order = order;
            vacancyAdditionalFeature.IsActive = isActive;

            vacancyAdditionalFeature.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _vacancyAdditionalFeatureRepository.UpdateAsync(vacancyAdditionalFeature);
        }

    }
}