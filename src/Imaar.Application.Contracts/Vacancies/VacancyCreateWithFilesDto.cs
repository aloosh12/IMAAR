using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Imaar.Vacancies;

namespace Imaar.Vacancies
{
    public class VacancyCreateWithFilesDto
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
        
        public DateOnly DateOfPublish { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        
        public string? ExpectedExperience { get; set; }
        
        public string? EducationLevel { get; set; }
        
        public string? WorkSchedule { get; set; }
        
        public string? EmploymentType { get; set; }
        
        public BiologicalSex BiologicalSex { get; set; } = ((BiologicalSex[])Enum.GetValues(typeof(BiologicalSex)))[0];
        
        public string? Languages { get; set; }
        
        public string? DriveLicense { get; set; }
        
        public string? Salary { get; set; }
        
        public Guid ServiceTypeId { get; set; }
        
        public Guid UserProfileId { get; set; }
        
        [Required]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
} 