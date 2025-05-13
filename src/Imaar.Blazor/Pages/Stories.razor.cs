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
using Imaar.Stories;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Stories
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<StoryWithNavigationPropertiesDto> StoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateStory { get; set; }
        private bool CanEditStory { get; set; }
        private bool CanDeleteStory { get; set; }
        private StoryCreateDto NewStory { get; set; }
        private Validations NewStoryValidations { get; set; } = new();
        private StoryUpdateDto EditingStory { get; set; }
        private Validations EditingStoryValidations { get; set; } = new();
        private Guid EditingStoryId { get; set; }
        private Modal CreateStoryModal { get; set; } = new();
        private Modal EditStoryModal { get; set; } = new();
        private GetStoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<StoryWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "story-create-tab";
        protected string SelectedEditTab = "story-edit-tab";
        private StoryWithNavigationPropertiesDto? SelectedStory;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<StoryWithNavigationPropertiesDto> SelectedStories { get; set; } = new();
        private bool AllStoriesSelected { get; set; }
        
        public Stories()
        {
            NewStory = new StoryCreateDto();
            EditingStory = new StoryUpdateDto();
            Filter = new GetStoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            StoryList = new List<StoryWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Stories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewStory"], async () =>
            {
                await OpenCreateStoryModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Stories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateStory = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Stories.Create);
            CanEditStory = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Stories.Edit);
            CanDeleteStory = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Stories.Delete);
                            
                            
        }

        private async Task GetStoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await StoriesAppService.GetListAsync(Filter);
            StoryList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetStoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await StoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/stories/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&FromTimeMin={Filter.FromTimeMin?.ToString("O")}&FromTimeMax={Filter.FromTimeMax?.ToString("O")}&ExpiryTimeMin={Filter.ExpiryTimeMin?.ToString("O")}&ExpiryTimeMax={Filter.ExpiryTimeMax?.ToString("O")}&StoryPublisherId={Filter.StoryPublisherId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StoryWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetStoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateStoryModalAsync()
        {
            NewStory = new StoryCreateDto{
                FromTime = DateTime.Now,
ExpiryTime = DateTime.Now,

                
            };

            SelectedCreateTab = "story-create-tab";
            
            
            await NewStoryValidations.ClearAll();
            await CreateStoryModal.Show();
        }

        private async Task CloseCreateStoryModalAsync()
        {
            NewStory = new StoryCreateDto{
                FromTime = DateTime.Now,
ExpiryTime = DateTime.Now,

                
            };
            await CreateStoryModal.Hide();
        }

        private async Task OpenEditStoryModalAsync(StoryWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "story-edit-tab";
            
            
            var story = await StoriesAppService.GetWithNavigationPropertiesAsync(input.Story.Id);
            
            EditingStoryId = story.Story.Id;
            EditingStory = ObjectMapper.Map<StoryDto, StoryUpdateDto>(story.Story);
            
            await EditingStoryValidations.ClearAll();
            await EditStoryModal.Show();
        }

        private async Task DeleteStoryAsync(StoryWithNavigationPropertiesDto input)
        {
            await StoriesAppService.DeleteAsync(input.Story.Id);
            await GetStoriesAsync();
        }

        private async Task CreateStoryAsync()
        {
            try
            {
                if (await NewStoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoriesAppService.CreateAsync(NewStory);
                await GetStoriesAsync();
                await CloseCreateStoryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditStoryModalAsync()
        {
            await EditStoryModal.Hide();
        }

        private async Task UpdateStoryAsync()
        {
            try
            {
                if (await EditingStoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoriesAppService.UpdateAsync(EditingStoryId, EditingStory);
                await GetStoriesAsync();
                await EditStoryModal.Hide();                
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
        protected virtual async Task OnFromTimeMinChangedAsync(DateTime? fromTimeMin)
        {
            Filter.FromTimeMin = fromTimeMin.HasValue ? fromTimeMin.Value.Date : fromTimeMin;
            await SearchAsync();
        }
        protected virtual async Task OnFromTimeMaxChangedAsync(DateTime? fromTimeMax)
        {
            Filter.FromTimeMax = fromTimeMax.HasValue ? fromTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : fromTimeMax;
            await SearchAsync();
        }
        protected virtual async Task OnExpiryTimeMinChangedAsync(DateTime? expiryTimeMin)
        {
            Filter.ExpiryTimeMin = expiryTimeMin.HasValue ? expiryTimeMin.Value.Date : expiryTimeMin;
            await SearchAsync();
        }
        protected virtual async Task OnExpiryTimeMaxChangedAsync(DateTime? expiryTimeMax)
        {
            Filter.ExpiryTimeMax = expiryTimeMax.HasValue ? expiryTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : expiryTimeMax;
            await SearchAsync();
        }
        protected virtual async Task OnStoryPublisherIdChangedAsync(Guid? storyPublisherId)
        {
            Filter.StoryPublisherId = storyPublisherId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await StoriesAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllStoriesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllStoriesSelected = false;
            SelectedStories.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedStoryRowsChanged()
        {
            if (SelectedStories.Count != PageSize)
            {
                AllStoriesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedStoriesAsync()
        {
            var message = AllStoriesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedStories.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllStoriesSelected)
            {
                await StoriesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await StoriesAppService.DeleteByIdsAsync(SelectedStories.Select(x => x.Story.Id).ToList());
            }

            SelectedStories.Clear();
            AllStoriesSelected = false;

            await GetStoriesAsync();
        }


    }
}
