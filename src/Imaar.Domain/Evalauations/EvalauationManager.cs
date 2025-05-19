using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.Evalauations
{
    public abstract class EvalauationManagerBase : DomainService
    {
        protected IEvalauationRepository _evalauationRepository;

        public EvalauationManagerBase(IEvalauationRepository evalauationRepository)
        {
            _evalauationRepository = evalauationRepository;
        }

        public virtual async Task<Evalauation> CreateAsync(
        Guid evaluatord, Guid evaluatedPersonId, int speedOfCompletion, int dealing, int cleanliness, int perfection, int price)
        {
            Check.NotNull(evaluatord, nameof(evaluatord));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));

            var evalauation = new Evalauation(
             GuidGenerator.Create(),
             evaluatord, evaluatedPersonId, speedOfCompletion, dealing, cleanliness, perfection, price
             );

            return await _evalauationRepository.InsertAsync(evalauation);
        }

        public virtual async Task<Evalauation> UpdateAsync(
            Guid id,
            Guid evaluatord, Guid evaluatedPersonId, int speedOfCompletion, int dealing, int cleanliness, int perfection, int price, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(evaluatord, nameof(evaluatord));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));

            var evalauation = await _evalauationRepository.GetAsync(id);

            evalauation.Evaluatord = evaluatord;
            evalauation.EvaluatedPersonId = evaluatedPersonId;
            evalauation.SpeedOfCompletion = speedOfCompletion;
            evalauation.Dealing = dealing;
            evalauation.Cleanliness = cleanliness;
            evalauation.Perfection = perfection;
            evalauation.Price = price;

            evalauation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _evalauationRepository.UpdateAsync(evalauation);
        }

    }
}