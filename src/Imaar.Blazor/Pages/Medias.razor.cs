using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Imaar.Medias;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Medias
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<MediaWithNavigationPropertiesDto> MediaList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMedia { get; set; }
        private bool CanEditMedia { get; set; }
        private bool CanDeleteMedia { get; set; }
        private MediaCreateDto NewMedia { get; set; }
        private Validations NewMediaValidations { get; set; } = new();
        private MediaUpdateDto EditingMedia { get; set; }
        private Validations EditingMediaValidations { get; set; } = new();
        private Guid EditingMediaId { get; set; }
        private Modal CreateMediaModal { get; set; } = new();
        private Modal EditMediaModal { get; set; } = new();
        private GetMediasInput Filter { get; set; }
        private DataGridEntityActionsColumn<MediaWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "media-create-tab";
        protected string SelectedEditTab = "media-edit-tab";
        private MediaWithNavigationPropertiesDto? SelectedMedia;
        private IReadOnlyList<LookupDto<Guid>> ImaarServicesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> VacanciesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> StoriesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<MediaWithNavigationPropertiesDto> SelectedMedias { get; set; } = new();
        private bool AllMediasSelected { get; set; }
        
        public Medias()
        {
            NewMedia = new MediaCreateDto();
            EditingMedia = new MediaUpdateDto();
            Filter = new GetMediasInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MediaList = new List<MediaWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetImaarServiceCollectionLookupAsync();


            await GetVacancyCollectionLookupAsync();


            await GetStoryCollectionLookupAsync();


            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Medias"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMedia"], async () =>
            {
                await OpenCreateMediaModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Medias.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMedia = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Medias.Create);
            CanEditMedia = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Medias.Edit);
            CanDeleteMedia = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Medias.Delete);
                            
                            
        }

        private async Task GetMediasAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MediasAppService.GetListAsync(Filter);
            MediaList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await MediasAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/medias/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&File={HttpUtility.UrlEncode(Filter.File)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}&ImaarServiceId={Filter.ImaarServiceId}&VacancyId={Filter.VacancyId}&StoryId={Filter.StoryId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MediaWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMediaModalAsync()
        {
            NewMedia = new MediaCreateDto{
                
                
            };

            SelectedCreateTab = "media-create-tab";
            
            
            await NewMediaValidations.ClearAll();
            await CreateMediaModal.Show();
        }

        private async Task CloseCreateMediaModalAsync()
        {
            NewMedia = new MediaCreateDto{
                
                
            };
            await CreateMediaModal.Hide();
        }

        private async Task OpenEditMediaModalAsync(MediaWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "media-edit-tab";
            
            
            var media = await MediasAppService.GetWithNavigationPropertiesAsync(input.Media.Id);
            
            EditingMediaId = media.Media.Id;
            EditingMedia = ObjectMapper.Map<MediaDto, MediaUpdateDto>(media.Media);
            
            await EditingMediaValidations.ClearAll();
            await EditMediaModal.Show();
        }

        private async Task DeleteMediaAsync(MediaWithNavigationPropertiesDto input)
        {
            await MediasAppService.DeleteAsync(input.Media.Id);
            await GetMediasAsync();
        }

        private async Task CreateMediaAsync()
        {
            try
            {
                if (await NewMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediasAppService.CreateAsync(NewMedia);
                await GetMediasAsync();
                await CloseCreateMediaModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMediaModalAsync()
        {
            await EditMediaModal.Hide();
        }

        private async Task UpdateMediaAsync()
        {
            try
            {
                if (await EditingMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await MediasAppService.UpdateAsync(EditingMediaId, EditingMedia);
                await GetMediasAsync();
                await EditMediaModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }









        protected virtual async Task OnTitleChangedAsync(string? title)
        {
            Filter.Title = title;
            await SearchAsync();
        }
        protected virtual async Task OnFileChangedAsync(string? file)
        {
            Filter.File = file;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMinChangedAsync(int? orderMin)
        {
            Filter.OrderMin = orderMin;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMaxChangedAsync(int? orderMax)
        {
            Filter.OrderMax = orderMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
        {
            Filter.IsActive = isActive;
            await SearchAsync();
        }
        protected virtual async Task OnImaarServiceIdChangedAsync(Guid? imaarServiceId)
        {
            Filter.ImaarServiceId = imaarServiceId;
            await SearchAsync();
        }
        protected virtual async Task OnVacancyIdChangedAsync(Guid? vacancyId)
        {
            Filter.VacancyId = vacancyId;
            await SearchAsync();
        }
        protected virtual async Task OnStoryIdChangedAsync(Guid? storyId)
        {
            Filter.StoryId = storyId;
            await SearchAsync();
        }
        

        private async Task GetImaarServiceCollectionLookupAsync(string? newValue = null)
        {
            ImaarServicesCollection = (await MediasAppService.GetImaarServiceLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetVacancyCollectionLookupAsync(string? newValue = null)
        {
            VacanciesCollection = (await MediasAppService.GetVacancyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetStoryCollectionLookupAsync(string? newValue = null)
        {
            StoriesCollection = (await MediasAppService.GetStoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllMediasSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllMediasSelected = false;
            SelectedMedias.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedMediaRowsChanged()
        {
            if (SelectedMedias.Count != PageSize)
            {
                AllMediasSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedMediasAsync()
        {
            var message = AllMediasSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedMedias.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllMediasSelected)
            {
                await MediasAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await MediasAppService.DeleteByIdsAsync(SelectedMedias.Select(x => x.Media.Id).ToList());
            }

            SelectedMedias.Clear();
            AllMediasSelected = false;

            await GetMediasAsync();
        }


    }
}
