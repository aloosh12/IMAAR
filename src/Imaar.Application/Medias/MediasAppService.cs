using Imaar.Shared;
using Imaar.Stories;
using Imaar.Vacancies;
using Imaar.ImaarServices;
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
using Imaar.Medias;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Imaar.Shared;

namespace Imaar.Medias
{

    [Authorize(ImaarPermissions.Medias.Default)]
    public abstract class MediasAppServiceBase : ImaarAppService
    {
        protected IDistributedCache<MediaDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IMediaRepository _mediaRepository;
        protected MediaManager _mediaManager;

        protected IRepository<Imaar.ImaarServices.ImaarService, Guid> _imaarServiceRepository;
        protected IRepository<Imaar.Vacancies.Vacancy, Guid> _vacancyRepository;
        protected IRepository<Imaar.Stories.Story, Guid> _storyRepository;

        public MediasAppServiceBase(IMediaRepository mediaRepository, MediaManager mediaManager, IDistributedCache<MediaDownloadTokenCacheItem, string> downloadTokenCache, IRepository<Imaar.ImaarServices.ImaarService, Guid> imaarServiceRepository, IRepository<Imaar.Vacancies.Vacancy, Guid> vacancyRepository, IRepository<Imaar.Stories.Story, Guid> storyRepository)
        {
            _downloadTokenCache = downloadTokenCache;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager; _imaarServiceRepository = imaarServiceRepository;
            _vacancyRepository = vacancyRepository;
            _storyRepository = storyRepository;

        }

        public virtual async Task<PagedResultDto<MediaWithNavigationPropertiesDto>> GetListAsync(GetMediasInput input)
        {
            var totalCount = await _mediaRepository.GetCountAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ImaarServiceId, input.VacancyId, input.StoryId);
            var items = await _mediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ImaarServiceId, input.VacancyId, input.StoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaWithNavigationProperties>, List<MediaWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<MediaWithNavigationProperties, MediaWithNavigationPropertiesDto>
                (await _mediaRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MediaDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Media, MediaDto>(await _mediaRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetImaarServiceLookupAsync(LookupRequestDto input)
        {
            var query = (await _imaarServiceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.ImaarServices.ImaarService>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.ImaarServices.ImaarService>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetVacancyLookupAsync(LookupRequestDto input)
        {
            var query = (await _vacancyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Vacancies.Vacancy>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Vacancies.Vacancy>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetStoryLookupAsync(LookupRequestDto input)
        {
            var query = (await _storyRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Title != null &&
                         x.Title.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Imaar.Stories.Story>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Imaar.Stories.Story>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _mediaRepository.DeleteAsync(id);
        }

        [Authorize(ImaarPermissions.Medias.Create)]
        public virtual async Task<MediaDto> CreateAsync(MediaCreateDto input)
        {

            var media = await _mediaManager.CreateAsync(
            input.ImaarServiceId, input.VacancyId, input.StoryId, input.File, input.Order, input.IsActive, input.Title
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [Authorize(ImaarPermissions.Medias.Edit)]
        public virtual async Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input)
        {

            var media = await _mediaManager.UpdateAsync(
            id,
            input.ImaarServiceId, input.VacancyId, input.StoryId, input.File, input.Order, input.IsActive, input.Title, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var medias = await _mediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ImaarServiceId, input.VacancyId, input.StoryId);
            var items = medias.Select(item => new
            {
                Title = item.Media.Title,
                File = item.Media.File,
                Order = item.Media.Order,
                IsActive = item.Media.IsActive,

                ImaarService = item.ImaarService?.Title,
                Vacancy = item.Vacancy?.Title,
                Story = item.Story?.Title,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Medias.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> mediaIds)
        {
            await _mediaRepository.DeleteManyAsync(mediaIds);
        }

        [Authorize(ImaarPermissions.Medias.Delete)]
        public virtual async Task DeleteAllAsync(GetMediasInput input)
        {
            await _mediaRepository.DeleteAllAsync(input.FilterText, input.Title, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ImaarServiceId, input.VacancyId, input.StoryId);
        }
        public virtual async Task<Imaar.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new MediaDownloadTokenCacheItem { Token = token },
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