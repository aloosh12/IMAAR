using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.ServiceEvaluations;
using Volo.Abp.Content;

namespace Imaar.Controllers.ServiceEvaluations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ServiceEvaluation")]
    [Route("api/app/service-evaluations")]

    public abstract class ServiceEvaluationControllerBase : AbpController
    {
        protected IServiceEvaluationsAppService _serviceEvaluationsAppService;

        public ServiceEvaluationControllerBase(IServiceEvaluationsAppService serviceEvaluationsAppService)
        {
            _serviceEvaluationsAppService = serviceEvaluationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ServiceEvaluationWithNavigationPropertiesDto>> GetListAsync(GetServiceEvaluationsInput input)
        {
            return _serviceEvaluationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<ServiceEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _serviceEvaluationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ServiceEvaluationDto> GetAsync(Guid id)
        {
            return _serviceEvaluationsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _serviceEvaluationsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpGet]
        [Route("imaar-service-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input)
        {
            return _serviceEvaluationsAppService.GetImaarServiceLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<ServiceEvaluationDto> CreateAsync(ServiceEvaluationCreateDto input)
        {
            return _serviceEvaluationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ServiceEvaluationDto> UpdateAsync(Guid id, ServiceEvaluationUpdateDto input)
        {
            return _serviceEvaluationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _serviceEvaluationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ServiceEvaluationExcelDownloadDto input)
        {
            return _serviceEvaluationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _serviceEvaluationsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> serviceevaluationIds)
        {
            return _serviceEvaluationsAppService.DeleteByIdsAsync(serviceevaluationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetServiceEvaluationsInput input)
        {
            return _serviceEvaluationsAppService.DeleteAllAsync(input);
        }
    }
}