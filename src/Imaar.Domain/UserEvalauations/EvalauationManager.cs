using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.UserEvalauations
{
    public abstract class UserEvalauationManagerBase : DomainService
    {
        protected IUserEvalauationRepository _userEvalauationRepository;

        public UserEvalauationManagerBase(IUserEvalauationRepository userEvalauationRepository)
        {
            _userEvalauationRepository = userEvalauationRepository;
        }

        public virtual async Task<UserEvalauation> CreateAsync(
        Guid evaluatord, Guid evaluatedPersonId, int speedOfCompletion, int dealing, int cleanliness, int perfection, int price)
        {
            Check.NotNull(evaluatord, nameof(evaluatord));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));

            var userEvalauation = new UserEvalauation(
             GuidGenerator.Create(),
             evaluatord, evaluatedPersonId, speedOfCompletion, dealing, cleanliness, perfection, price
             );

            return await _userEvalauationRepository.InsertAsync(userEvalauation);
        }

        public virtual async Task<UserEvalauation> UpdateAsync(
            Guid id,
            Guid evaluatord, Guid evaluatedPersonId, int speedOfCompletion, int dealing, int cleanliness, int perfection, int price, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(evaluatord, nameof(evaluatord));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));

            var userEvalauation = await _userEvalauationRepository.GetAsync(id);

            userEvalauation.Evaluatord = evaluatord;
            userEvalauation.EvaluatedPersonId = evaluatedPersonId;
            userEvalauation.SpeedOfCompletion = speedOfCompletion;
            userEvalauation.Dealing = dealing;
            userEvalauation.Cleanliness = cleanliness;
            userEvalauation.Perfection = perfection;
            userEvalauation.Price = price;

            userEvalauation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _userEvalauationRepository.UpdateAsync(userEvalauation);
        }

    }
}