using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.VerificationCodes;
using Volo.Abp.Content;
using Imaar.Shared;

namespace Imaar.Controllers.VerificationCodes
{
    [RemoteService]
    [Area("app")]
    [ControllerName("VerificationCode")]
    [Route("api/app/verification-codes")]

    public abstract class VerificationCodeControllerBase : AbpController
    {
        protected IVerificationCodesAppService _verificationCodesAppService;

        public VerificationCodeControllerBase(IVerificationCodesAppService verificationCodesAppService)
        {
            _verificationCodesAppService = verificationCodesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<VerificationCodeDto>> GetListAsync(GetVerificationCodesInput input)
        {
            return _verificationCodesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<VerificationCodeDto> GetAsync(Guid id)
        {
            return _verificationCodesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<VerificationCodeDto> CreateAsync(VerificationCodeCreateDto input)
        {
            return _verificationCodesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<VerificationCodeDto> UpdateAsync(Guid id, VerificationCodeUpdateDto input)
        {
            return _verificationCodesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _verificationCodesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(VerificationCodeExcelDownloadDto input)
        {
            return _verificationCodesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _verificationCodesAppService.GetDownloadTokenAsync();
        }

        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> verificationcodeIds)
        {
            return _verificationCodesAppService.DeleteByIdsAsync(verificationcodeIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetVerificationCodesInput input)
        {
            return _verificationCodesAppService.DeleteAllAsync(input);
        }
    }
}