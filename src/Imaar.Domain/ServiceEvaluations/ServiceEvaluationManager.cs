using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.ServiceEvaluations
{
    public abstract class ServiceEvaluationManagerBase : DomainService
    {
        protected IServiceEvaluationRepository _serviceEvaluationRepository;

        public ServiceEvaluationManagerBase(IServiceEvaluationRepository serviceEvaluationRepository)
        {
            _serviceEvaluationRepository = serviceEvaluationRepository;
        }

        public virtual async Task<ServiceEvaluation> CreateAsync(
        Guid evaluatorId, Guid imaarServiceId, int rate)
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(imaarServiceId, nameof(imaarServiceId));

            var serviceEvaluation = new ServiceEvaluation(
             GuidGenerator.Create(),
             evaluatorId, imaarServiceId, rate
             );

            return await _serviceEvaluationRepository.InsertAsync(serviceEvaluation);
        }

        public virtual async Task<ServiceEvaluation> UpdateAsync(
            Guid id,
            Guid evaluatorId, Guid imaarServiceId, int rate, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(imaarServiceId, nameof(imaarServiceId));

            var serviceEvaluation = await _serviceEvaluationRepository.GetAsync(id);

            serviceEvaluation.EvaluatorId = evaluatorId;
            serviceEvaluation.ImaarServiceId = imaarServiceId;
            serviceEvaluation.Rate = rate;

            serviceEvaluation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _serviceEvaluationRepository.UpdateAsync(serviceEvaluation);
        }

    }
}