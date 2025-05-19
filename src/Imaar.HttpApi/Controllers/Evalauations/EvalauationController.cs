using Imaar.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Evalauations;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.Evalauations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Evalauation")]
    [Route("api/app/evalauations")]

    public abstract class EvalauationControllerBase : AbpController
    {
        protected IEvalauationsAppService _evalauationsAppService;

        public EvalauationControllerBase(IEvalauationsAppService evalauationsAppService)
        {
            _evalauationsAppService = evalauationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<EvalauationWithNavigationPropertiesDto>> GetListAsync(GetEvalauationsInput input)
        {
            return _evalauationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<EvalauationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _evalauationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<EvalauationDto> GetAsync(Guid id)
        {
            return _evalauationsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("user-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input)
        {
            return _evalauationsAppService.GetUserProfileLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<EvalauationDto> CreateAsync(EvalauationCreateDto input)
        {
            return _evalauationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<EvalauationDto> UpdateAsync(Guid id, EvalauationUpdateDto input)
        {
            return _evalauationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _evalauationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(EvalauationExcelDownloadDto input)
        {
            return _evalauationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _evalauationsAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> evalauationIds)
        {
            return _evalauationsAppService.DeleteByIdsAsync(evalauationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetEvalauationsInput input)
        {
            return _evalauationsAppService.DeleteAllAsync(input);
        }
    }
}