using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Imaar.Permissions;
using Imaar.VerificationCodes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;
using System.Net.Http;

namespace Imaar.VerificationCodes
{

    [RemoteService(false)]
    [Authorize(ImaarPermissions.VerificationCodes.Default)]
    public abstract class VerificationCodesAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<VerificationCodeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IVerificationCodeRepository _verificationCodeRepository;
        protected VerificationCodeManager _verificationCodeManager;
        public VerificationCodesAppServiceBase(IVerificationCodeRepository verificationCodeRepository, VerificationCodeManager verificationCodeManager, IDistributedCache<VerificationCodeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _verificationCodeRepository = verificationCodeRepository;
            _verificationCodeManager = verificationCodeManager;
     
        }

        public virtual async Task<PagedResultDto<VerificationCodeDto>> GetListAsync(GetVerificationCodesInput input)
        {
            var totalCount = await _verificationCodeRepository.GetCountAsync(input.FilterText, input.PhoneNumber, input.SecurityCodeMin, input.SecurityCodeMax, input.IsFinish);
            var items = await _verificationCodeRepository.GetListAsync(input.FilterText, input.PhoneNumber, input.SecurityCodeMin, input.SecurityCodeMax, input.IsFinish, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<VerificationCodeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<VerificationCode>, List<VerificationCodeDto>>(items)
            };
        }

        public virtual async Task<VerificationCodeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(await _verificationCodeRepository.GetAsync(id));
        }

        [Authorize(ImaarPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _verificationCodeRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.VerificationCodes.Create)]
        public virtual async Task<VerificationCodeDto> CreateAsync(VerificationCodeCreateDto input)
        {

            var verificationCode = await _verificationCodeManager.CreateAsync(
            input.PhoneNumber, input.SecurityCode, input.IsFinish
            );

            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(verificationCode);
        }

        [Authorize(ImaarPermissions.VerificationCodes.Edit)]
        public virtual async Task<VerificationCodeDto> UpdateAsync(Guid id, VerificationCodeUpdateDto input)
        {

            var verificationCode = await _verificationCodeManager.UpdateAsync(
            id,
            input.PhoneNumber, input.SecurityCode, input.IsFinish, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(verificationCode);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(VerificationCodeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _verificationCodeRepository.GetListAsync(input.FilterText, input.PhoneNumber, input.SecurityCodeMin, input.SecurityCodeMax, input.IsFinish);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<VerificationCode>, List<VerificationCodeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "VerificationCodes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> verificationcodeIds)
        {
            await _verificationCodeRepository.DeleteManyAsync(verificationcodeIds);
        }

        [Authorize(ImaarPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteAllAsync(GetVerificationCodesInput input)
        {
            await _verificationCodeRepository.DeleteAllAsync(input.FilterText, input.PhoneNumber, input.SecurityCodeMin, input.SecurityCodeMax, input.IsFinish);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new VerificationCodeDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Imaar.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}