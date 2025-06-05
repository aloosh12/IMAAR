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
using Imaar.UserSavedItems;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

using Imaar.UserSavedItems;



namespace Imaar.Blazor.Pages
{
    public partial class UserSavedItems
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserSavedItemWithNavigationPropertiesDto> UserSavedItemList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserSavedItem { get; set; }
        private bool CanEditUserSavedItem { get; set; }
        private bool CanDeleteUserSavedItem { get; set; }
        private UserSavedItemCreateDto NewUserSavedItem { get; set; }
        private Validations NewUserSavedItemValidations { get; set; } = new();
        private UserSavedItemUpdateDto EditingUserSavedItem { get; set; }
        private Validations EditingUserSavedItemValidations { get; set; } = new();
        private Guid EditingUserSavedItemId { get; set; }
        private Modal CreateUserSavedItemModal { get; set; } = new();
        private Modal EditUserSavedItemModal { get; set; } = new();
        private GetUserSavedItemsInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserSavedItemWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userSavedItem-create-tab";
        protected string SelectedEditTab = "userSavedItem-edit-tab";
        private UserSavedItemWithNavigationPropertiesDto? SelectedUserSavedItem;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<UserSavedItemWithNavigationPropertiesDto> SelectedUserSavedItems { get; set; } = new();
        private bool AllUserSavedItemsSelected { get; set; }
        
        public UserSavedItems()
        {
            NewUserSavedItem = new UserSavedItemCreateDto();
            EditingUserSavedItem = new UserSavedItemUpdateDto();
            Filter = new GetUserSavedItemsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserSavedItemList = new List<UserSavedItemWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserSavedItems"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserSavedItem"], async () =>
            {
                await OpenCreateUserSavedItemModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.UserSavedItems.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserSavedItem = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.UserSavedItems.Create);
            CanEditUserSavedItem = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserSavedItems.Edit);
            CanDeleteUserSavedItem = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserSavedItems.Delete);
                            
                            
        }

        private async Task GetUserSavedItemsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserSavedItemsAppService.GetListAsync(Filter);
            UserSavedItemList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserSavedItemsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserSavedItemsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/user-saved-items/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&SourceId={HttpUtility.UrlEncode(Filter.SourceId)}&SavedItemType={Filter.SavedItemType}&UserProfileId={Filter.UserProfileId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserSavedItemWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserSavedItemsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserSavedItemModalAsync()
        {
            NewUserSavedItem = new UserSavedItemCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "userSavedItem-create-tab";
            
            
            await NewUserSavedItemValidations.ClearAll();
            await CreateUserSavedItemModal.Show();
        }

        private async Task CloseCreateUserSavedItemModalAsync()
        {
            NewUserSavedItem = new UserSavedItemCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateUserSavedItemModal.Hide();
        }

        private async Task OpenEditUserSavedItemModalAsync(UserSavedItemWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "userSavedItem-edit-tab";
            
            
            var userSavedItem = await UserSavedItemsAppService.GetWithNavigationPropertiesAsync(input.UserSavedItem.Id);
            
            EditingUserSavedItemId = userSavedItem.UserSavedItem.Id;
            EditingUserSavedItem = ObjectMapper.Map<UserSavedItemDto, UserSavedItemUpdateDto>(userSavedItem.UserSavedItem);
            
            await EditingUserSavedItemValidations.ClearAll();
            await EditUserSavedItemModal.Show();
        }

        private async Task DeleteUserSavedItemAsync(UserSavedItemWithNavigationPropertiesDto input)
        {
            await UserSavedItemsAppService.DeleteAsync(input.UserSavedItem.Id);
            await GetUserSavedItemsAsync();
        }

        private async Task CreateUserSavedItemAsync()
        {
            try
            {
                if (await NewUserSavedItemValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserSavedItemsAppService.CreateAsync(NewUserSavedItem);
                await GetUserSavedItemsAsync();
                await CloseCreateUserSavedItemModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserSavedItemModalAsync()
        {
            await EditUserSavedItemModal.Hide();
        }

        private async Task UpdateUserSavedItemAsync()
        {
            try
            {
                if (await EditingUserSavedItemValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserSavedItemsAppService.UpdateAsync(EditingUserSavedItemId, EditingUserSavedItem);
                await GetUserSavedItemsAsync();
                await EditUserSavedItemModal.Hide();                
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









        protected virtual async Task OnSourceIdChangedAsync(string? sourceId)
        {
            Filter.SourceId = sourceId;
            await SearchAsync();
        }
        protected virtual async Task OnSavedItemTypeChangedAsync(UserSavedItemType? savedItemType)
        {
            Filter.SavedItemType = savedItemType;
            await SearchAsync();
        }
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await UserSavedItemsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllUserSavedItemsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserSavedItemsSelected = false;
            SelectedUserSavedItems.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserSavedItemRowsChanged()
        {
            if (SelectedUserSavedItems.Count != PageSize)
            {
                AllUserSavedItemsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserSavedItemsAsync()
        {
            var message = AllUserSavedItemsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserSavedItems.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserSavedItemsSelected)
            {
                await UserSavedItemsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserSavedItemsAppService.DeleteByIdsAsync(SelectedUserSavedItems.Select(x => x.UserSavedItem.Id).ToList());
            }

            SelectedUserSavedItems.Clear();
            AllUserSavedItemsSelected = false;

            await GetUserSavedItemsAsync();
        }


    }
}
