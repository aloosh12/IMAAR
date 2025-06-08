using Imaar.Vacancies;
using Imaar.ServiceTypes;
using Imaar.UserProfiles;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Imaar.Vacancies
{
    public abstract class VacancyBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Title { get; set; }

        [NotNull]
        public virtual string Description { get; set; }

        [NotNull]
        public virtual string Location { get; set; }

        [NotNull]
        public virtual string Number { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }

        public virtual DateOnly DateOfPublish { get; set; }

        [CanBeNull]
        public virtual string? ExpectedExperience { get; set; }

        [CanBeNull]
        public virtual string? EducationLevel { get; set; }

        [CanBeNull]
        public virtual string? WorkSchedule { get; set; }

        [CanBeNull]
        public virtual string? EmploymentType { get; set; }

        public virtual BiologicalSex BiologicalSex { get; set; }

        [CanBeNull]
        public virtual string? Languages { get; set; }

        [CanBeNull]
        public virtual string? DriveLicense { get; set; }

        [CanBeNull]
        public virtual string? Salary { get; set; }

        public virtual int ViewCounter { get; set; }

        public virtual int OrderCounter { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }
        public ICollection<VacancyVacancyAdditionalFeature> VacancyAdditionalFeatures { get; private set; }

        protected VacancyBase()
        {

        }

        public VacancyBase(Guid id, Guid serviceTypeId, Guid userProfileId, string title, string description, string location, string number, DateOnly dateOfPublish, BiologicalSex biologicalSex, int viewCounter, int orderCounter, string? latitude = null, string? longitude = null, string? expectedExperience = null, string? educationLevel = null, string? workSchedule = null, string? employmentType = null, string? languages = null, string? driveLicense = null, string? salary = null)
        {

            Id = id;
            Check.NotNull(title, nameof(title));
            Check.NotNull(description, nameof(description));
            Check.NotNull(location, nameof(location));
            Check.NotNull(number, nameof(number));
            Title = title;
            Description = description;
            Location = location;
            Number = number;
            DateOfPublish = dateOfPublish;
            BiologicalSex = biologicalSex;
            ViewCounter = viewCounter;
            OrderCounter = orderCounter;
            Latitude = latitude;
            Longitude = longitude;
            ExpectedExperience = expectedExperience;
            EducationLevel = educationLevel;
            WorkSchedule = workSchedule;
            EmploymentType = employmentType;
            Languages = languages;
            DriveLicense = driveLicense;
            Salary = salary;
            ServiceTypeId = serviceTypeId;
            UserProfileId = userProfileId;
            VacancyAdditionalFeatures = new Collection<VacancyVacancyAdditionalFeature>();
        }
        public virtual void AddVacancyAdditionalFeature(Guid vacancyAdditionalFeatureId)
        {
            Check.NotNull(vacancyAdditionalFeatureId, nameof(vacancyAdditionalFeatureId));

            if (IsInVacancyAdditionalFeatures(vacancyAdditionalFeatureId))
            {
                return;
            }

            VacancyAdditionalFeatures.Add(new VacancyVacancyAdditionalFeature(Id, vacancyAdditionalFeatureId));
        }

        public virtual void RemoveVacancyAdditionalFeature(Guid vacancyAdditionalFeatureId)
        {
            Check.NotNull(vacancyAdditionalFeatureId, nameof(vacancyAdditionalFeatureId));

            if (!IsInVacancyAdditionalFeatures(vacancyAdditionalFeatureId))
            {
                return;
            }

            VacancyAdditionalFeatures.RemoveAll(x => x.VacancyAdditionalFeatureId == vacancyAdditionalFeatureId);
        }

        public virtual void RemoveAllVacancyAdditionalFeaturesExceptGivenIds(List<Guid> vacancyAdditionalFeatureIds)
        {
            Check.NotNullOrEmpty(vacancyAdditionalFeatureIds, nameof(vacancyAdditionalFeatureIds));

            VacancyAdditionalFeatures.RemoveAll(x => !vacancyAdditionalFeatureIds.Contains(x.VacancyAdditionalFeatureId));
        }

        public virtual void RemoveAllVacancyAdditionalFeatures()
        {
            VacancyAdditionalFeatures.RemoveAll(x => x.VacancyId == Id);
        }

        private bool IsInVacancyAdditionalFeatures(Guid vacancyAdditionalFeatureId)
        {
            return VacancyAdditionalFeatures.Any(x => x.VacancyAdditionalFeatureId == vacancyAdditionalFeatureId);
        }
    }
}