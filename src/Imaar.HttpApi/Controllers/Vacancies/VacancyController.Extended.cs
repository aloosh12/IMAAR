using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Vacancies;
using Microsoft.AspNetCore.Authorization;
using Imaar.MobileResponses;

namespace Imaar.Controllers.Vacancies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Vacancy")]
    [Route("api/mobile/vacancies")]

    public class VacancyController : VacancyControllerBase, IVacanciesAppService
    {
        public VacancyController(IVacanciesAppService vacanciesAppService) : base(vacanciesAppService)
        {
        }
        
        [HttpPost("create-with-files")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> CreateWithFilesAsync([FromForm] VacancyCreateWithFilesDto input)
        {
            return _vacanciesAppService.CreateWithFilesAsync(input);
        }
        
        [HttpGet("{id}/with-details")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> GetVacancyWithDetailsAsync(Guid id)
        {
            return _vacanciesAppService.GetVacancyWithDetailsAsync(id);
        }
        
        [HttpPost("increment-view-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementViewCounterAsync(Guid id)
        {
            return _vacanciesAppService.IncrementViewCounterAsync(id);
        }
        
        [HttpPost("increment-order-counter/{id}")]
        [AllowAnonymous]
        public virtual Task<MobileResponseDto> IncrementOrderCounterAsync(Guid id)
        {
            return _vacanciesAppService.IncrementOrderCounterAsync(id);
        }
    }
}