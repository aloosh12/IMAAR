using Imaar.Vacancies;
using Volo.Abp.Application.Dtos;
using System;

namespace Imaar.Vacancies
{
    public abstract class VacancyExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Number { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateOnly? DateOfPublishMin { get; set; }
        public DateOnly? DateOfPublishMax { get; set; }
        public string? ExpectedExperience { get; set; }
        public string? EducationLevel { get; set; }
        public string? WorkSchedule { get; set; }
        public string? EmploymentType { get; set; }
        public BiologicalSex? BiologicalSex { get; set; }
        public string? Languages { get; set; }
        public string? DriveLicense { get; set; }
        public string? Salary { get; set; }
        public string? PhoneNumber { get; set; }
        public int? ViewCounterMin { get; set; }
        public int? ViewCounterMax { get; set; }
        public int? OrderCounterMin { get; set; }
        public int? OrderCounterMax { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? UserProfileId { get; set; }
        public Guid? VacancyAdditionalFeatureId { get; set; }

        public VacancyExcelDownloadDtoBase()
        {

        }
    }
}