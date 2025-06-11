using Imaar.Vacancies;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Imaar.Vacancies
{
    public abstract class VacancyCreateDtoBase
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string Location { get; set; } = null!;
        [Required]
        public string Number { get; set; } = null!;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateOnly DateOfPublish { get; set; }
        public string? ExpectedExperience { get; set; }
        public string? EducationLevel { get; set; }
        public string? WorkSchedule { get; set; }
        public string? EmploymentType { get; set; }
        public BiologicalSex BiologicalSex { get; set; } = ((BiologicalSex[])Enum.GetValues(typeof(BiologicalSex)))[0];
        public string? Languages { get; set; }
        public string? DriveLicense { get; set; }
        public string? Salary { get; set; }
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public int ViewCounter { get; set; } = 0;
        public int OrderCounter { get; set; } = 0;
        public Guid ServiceTypeId { get; set; }
        public Guid UserProfileId { get; set; }
        public List<Guid> VacancyAdditionalFeatureIds { get; set; }
    }
}