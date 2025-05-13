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
using Imaar.StoryLovers;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class StoryLovers
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<StoryLoverWithNavigationPropertiesDto> StoryLoverList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateStoryLover { get; set; }
        private bool CanEditStoryLover { get; set; }
        private bool CanDeleteStoryLover { get; set; }
        private StoryLoverCreateDto NewStoryLover { get; set; }
        private Validations NewStoryLoverValidations { get; set; } = new();
        private StoryLoverUpdateDto EditingStoryLover { get; set; }
        private Validations EditingStoryLoverValidations { get; set; } = new();
        private Guid EditingStoryLoverId { get; set; }
        private Modal CreateStoryLoverModal { get; set; } = new();
        private Modal EditStoryLoverModal { get; set; } = new();
        private GetStoryLoversInput Filter { get; set; }
        private DataGridEntityActionsColumn<StoryLoverWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "storyLover-create-tab";
        protected string SelectedEditTab = "storyLover-edit-tab";
        private StoryLoverWithNavigationPropertiesDto? SelectedStoryLover;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> StoriesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<StoryLoverWithNavigationPropertiesDto> SelectedStoryLovers { get; set; } = new();
        private bool AllStoryLoversSelected { get; set; }
        
        public StoryLovers()
        {
            NewStoryLover = new StoryLoverCreateDto();
            EditingStoryLover = new StoryLoverUpdateDto();
            Filter = new GetStoryLoversInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            StoryLoverList = new List<StoryLoverWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["StoryLovers"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewStoryLover"], async () =>
            {
                await OpenCreateStoryLoverModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.StoryLovers.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateStoryLover = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.StoryLovers.Create);
            CanEditStoryLover = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryLovers.Edit);
            CanDeleteStoryLover = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryLovers.Delete);
                            
                            
        }

        private async Task GetStoryLoversAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await StoryLoversAppService.GetListAsync(Filter);
            StoryLoverList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetStoryLoversAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await StoryLoversAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/story-lovers/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&UserProfileId={Filter.UserProfileId}&StoryId={Filter.StoryId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StoryLoverWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetStoryLoversAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateStoryLoverModalAsync()
        {
            NewStoryLover = new StoryLoverCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
StoryId = StoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "storyLover-create-tab";
            
            
            await NewStoryLoverValidations.ClearAll();
            await CreateStoryLoverModal.Show();
        }

        private async Task CloseCreateStoryLoverModalAsync()
        {
            NewStoryLover = new StoryLoverCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
StoryId = StoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateStoryLoverModal.Hide();
        }

        private async Task OpenEditStoryLoverModalAsync(StoryLoverWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "storyLover-edit-tab";
            
            
            var storyLover = await StoryLoversAppService.GetWithNavigationPropertiesAsync(input.StoryLover.Id);
            
            EditingStoryLoverId = storyLover.StoryLover.Id;
            EditingStoryLover = ObjectMapper.Map<StoryLoverDto, StoryLoverUpdateDto>(storyLover.StoryLover);
            
            await EditingStoryLoverValidations.ClearAll();
            await EditStoryLoverModal.Show();
        }

        private async Task DeleteStoryLoverAsync(StoryLoverWithNavigationPropertiesDto input)
        {
            await StoryLoversAppService.DeleteAsync(input.StoryLover.Id);
            await GetStoryLoversAsync();
        }

        private async Task CreateStoryLoverAsync()
        {
            try
            {
                if (await NewStoryLoverValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryLoversAppService.CreateAsync(NewStoryLover);
                await GetStoryLoversAsync();
                await CloseCreateStoryLoverModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditStoryLoverModalAsync()
        {
            await EditStoryLoverModal.Hide();
        }

        private async Task UpdateStoryLoverAsync()
        {
            try
            {
                if (await EditingStoryLoverValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryLoversAppService.UpdateAsync(EditingStoryLoverId, EditingStoryLover);
                await GetStoryLoversAsync();
                await EditStoryLoverModal.Hide();                
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









        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        protected virtual async Task OnStoryIdChangedAsync(Guid? storyId)
        {
            Filter.StoryId = storyId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await StoryLoversAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetStoryCollectionLookupAsync(string? newValue = null)
        {
            StoriesCollection = (await StoryLoversAppService.GetStoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllStoryLoversSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllStoryLoversSelected = false;
            SelectedStoryLovers.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedStoryLoverRowsChanged()
        {
            if (SelectedStoryLovers.Count != PageSize)
            {
                AllStoryLoversSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedStoryLoversAsync()
        {
            var message = AllStoryLoversSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedStoryLovers.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllStoryLoversSelected)
            {
                await StoryLoversAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await StoryLoversAppService.DeleteByIdsAsync(SelectedStoryLovers.Select(x => x.StoryLover.Id).ToList());
            }

            SelectedStoryLovers.Clear();
            AllStoryLoversSelected = false;

            await GetStoryLoversAsync();
        }


    }
}
