using System;
using Volo.Abp.Domain.Entities;

namespace Imaar.Vacancies
{
    public class VacancyVacancyAdditionalFeature : Entity
    {

        public Guid VacancyId { get; protected set; }

        public Guid VacancyAdditionalFeatureId { get; protected set; }

        private VacancyVacancyAdditionalFeature()
        {

        }

        public VacancyVacancyAdditionalFeature(Guid vacancyId, Guid vacancyAdditionalFeatureId)
        {
            VacancyId = vacancyId;
            VacancyAdditionalFeatureId = vacancyAdditionalFeatureId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    VacancyId,
                    VacancyAdditionalFeatureId
                };
        }
    }
}