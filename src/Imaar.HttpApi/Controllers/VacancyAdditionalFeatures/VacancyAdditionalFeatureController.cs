using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Vacancies;
using Volo.Abp.Content;
using Imaar.Shared;
using Imaar.VacancyAdditionalFeatures;

namespace Imaar.Controllers.VacancyAdditionalFeatures
{
    [RemoteService]
    [Area("app")]
    [ControllerName("VacancyAdditionalFeature")]
    [Route("api/mobile/vacancy-additional-features")]

    public class VacancyAdditionalFeatureController : AbpController
    {
        protected IVacancyAdditionalFeaturesAppService _vacancyAdditionalFeaturesApp;

        public VacancyAdditionalFeatureController(IVacancyAdditionalFeaturesAppService vacancyAdditionalFeaturesApp)
        {
            _vacancyAdditionalFeaturesApp = vacancyAdditionalFeaturesApp;
        }

       
        [HttpPost("create")]
        public virtual Task<VacancyAdditionalFeatureDto> CreateAsync(VacancyAdditionalFeatureCreateDto input)
        {
            return _vacancyAdditionalFeaturesApp.CreateAsync(input);
        }
    }
}