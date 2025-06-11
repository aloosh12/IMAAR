using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.VacancyEvaluations
{
    public abstract class VacancyEvaluationManagerBase : DomainService
    {
        protected IVacancyEvaluationRepository _vacancyEvaluationRepository;

        public VacancyEvaluationManagerBase(IVacancyEvaluationRepository vacancyEvaluationRepository)
        {
            _vacancyEvaluationRepository = vacancyEvaluationRepository;
        }

        public virtual async Task<VacancyEvaluation> CreateAsync(
        Guid userProfileId, Guid vacancyId, int rate)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(vacancyId, nameof(vacancyId));

            var vacancyEvaluation = new VacancyEvaluation(
             GuidGenerator.Create(),
             userProfileId, vacancyId, rate
             );

            return await _vacancyEvaluationRepository.InsertAsync(vacancyEvaluation);
        }

        public virtual async Task<VacancyEvaluation> UpdateAsync(
            Guid id,
            Guid userProfileId, Guid vacancyId, int rate, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(vacancyId, nameof(vacancyId));

            var vacancyEvaluation = await _vacancyEvaluationRepository.GetAsync(id);

            vacancyEvaluation.UserProfileId = userProfileId;
            vacancyEvaluation.VacancyId = vacancyId;
            vacancyEvaluation.Rate = rate;

            vacancyEvaluation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _vacancyEvaluationRepository.UpdateAsync(vacancyEvaluation);
        }

    }
}