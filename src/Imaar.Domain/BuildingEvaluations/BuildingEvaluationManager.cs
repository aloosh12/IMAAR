using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace Imaar.BuildingEvaluations
{
    public abstract class BuildingEvaluationManagerBase : DomainService
    {
        protected IBuildingEvaluationRepository _buildingEvaluationRepository;

        public BuildingEvaluationManagerBase(IBuildingEvaluationRepository buildingEvaluationRepository)
        {
            _buildingEvaluationRepository = buildingEvaluationRepository;
        }

        public virtual async Task<BuildingEvaluation> CreateAsync(
        Guid evaluatorId, Guid buildingId, int rate)
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(buildingId, nameof(buildingId));

            var buildingEvaluation = new BuildingEvaluation(
             GuidGenerator.Create(),
             evaluatorId, buildingId, rate
             );

            return await _buildingEvaluationRepository.InsertAsync(buildingEvaluation);
        }

        public virtual async Task<BuildingEvaluation> UpdateAsync(
            Guid id,
            Guid evaluatorId, Guid buildingId, int rate, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(buildingId, nameof(buildingId));

            var buildingEvaluation = await _buildingEvaluationRepository.GetAsync(id);

            buildingEvaluation.EvaluatorId = evaluatorId;
            buildingEvaluation.BuildingId = buildingId;
            buildingEvaluation.Rate = rate;

            buildingEvaluation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _buildingEvaluationRepository.UpdateAsync(buildingEvaluation);
        }

    }
}