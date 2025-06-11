using Imaar.Vacancies;
using System;

namespace Imaar.Vacancies
{
    public abstract class VacancyExcelDtoBase
    {
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Number { get; set; } = null!;
        public DateOnly DateOfPublish { get; set; }
        public string? ExpectedExperience { get; set; }
        public string? EducationLevel { get; set; }
        public string? WorkSchedule { get; set; }
        public string? EmploymentType { get; set; }
        public BiologicalSex BiologicalSex { get; set; }
        public string? Languages { get; set; }
        public string? DriveLicense { get; set; }
        public string? Salary { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int ViewCounter { get; set; }
        public int OrderCounter { get; set; }
    }
}