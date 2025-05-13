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

namespace Imaar.Controllers.Vacancies
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Vacancy")]
    [Route("api/app/vacancies")]

    public abstract class VacancyControllerBase : AbpController
    {
        protected IVacanciesAppService _vacanciesAppService;

        public VacancyControllerBase(IVacanciesAppService vacanciesAppService)
        {
            _vacanciesAppService = vacanciesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<VacancyWithNavigationPropertiesDto>> GetListAsync(GetVacanciesInput input)
        {
            return _vacanciesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<VacancyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _vacanciesAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<VacancyDto> GetAsync(Guid id)
        {
            return _vacanciesAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("service-type-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetServiceTypeLookupAsync(LookupRequestDto input)
        {
            return _vacanciesAppService.GetServiceTypeLookupAsync(input);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _vacanciesAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<VacancyDto> CreateAsync(VacancyCreateDto input)
        {
            return _vacanciesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<VacancyDto> UpdateAsync(Guid id, VacancyUpdateDto input)
        {
            return _vacanciesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _vacanciesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(VacancyExcelDownloadDto input)
        {
            return _vacanciesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _vacanciesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> vacancyIds)
        {
            return _vacanciesAppService.DeleteByIdsAsync(vacancyIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetVacanciesInput input)
        {
            return _vacanciesAppService.DeleteAllAsync(input);
        }
    }
}