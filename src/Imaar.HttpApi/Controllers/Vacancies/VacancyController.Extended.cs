using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Vacancies;

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
    }
}