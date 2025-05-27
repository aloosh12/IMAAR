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
using Imaar.UserFollows;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class UserFollows
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserFollowWithNavigationPropertiesDto> UserFollowList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserFollow { get; set; }
        private bool CanEditUserFollow { get; set; }
        private bool CanDeleteUserFollow { get; set; }
        private UserFollowCreateDto NewUserFollow { get; set; }
        private Validations NewUserFollowValidations { get; set; } = new();
        private UserFollowUpdateDto EditingUserFollow { get; set; }
        private Validations EditingUserFollowValidations { get; set; } = new();
        private Guid EditingUserFollowId { get; set; }
        private Modal CreateUserFollowModal { get; set; } = new();
        private Modal EditUserFollowModal { get; set; } = new();
        private GetUserFollowsInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserFollowWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userFollow-create-tab";
        protected string SelectedEditTab = "userFollow-edit-tab";
        private UserFollowWithNavigationPropertiesDto? SelectedUserFollow;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<UserFollowWithNavigationPropertiesDto> SelectedUserFollows { get; set; } = new();
        private bool AllUserFollowsSelected { get; set; }
        
        public UserFollows()
        {
            NewUserFollow = new UserFollowCreateDto();
            EditingUserFollow = new UserFollowUpdateDto();
            Filter = new GetUserFollowsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserFollowList = new List<UserFollowWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserFollows"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserFollow"], async () =>
            {
                await OpenCreateUserFollowModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.UserFollows.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserFollow = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.UserFollows.Create);
            CanEditUserFollow = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserFollows.Edit);
            CanDeleteUserFollow = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserFollows.Delete);
                            
                            
        }

        private async Task GetUserFollowsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserFollowsAppService.GetListAsync(Filter);
            UserFollowList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserFollowsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserFollowsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/user-follows/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FollowerUserId={Filter.FollowerUserId}&FollowingUserId={Filter.FollowingUserId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserFollowWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserFollowsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserFollowModalAsync()
        {
            NewUserFollow = new UserFollowCreateDto{
                
                FollowerUserId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
FollowingUserId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "userFollow-create-tab";
            
            
            await NewUserFollowValidations.ClearAll();
            await CreateUserFollowModal.Show();
        }

        private async Task CloseCreateUserFollowModalAsync()
        {
            NewUserFollow = new UserFollowCreateDto{
                
                FollowerUserId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
FollowingUserId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateUserFollowModal.Hide();
        }

        private async Task OpenEditUserFollowModalAsync(UserFollowWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "userFollow-edit-tab";
            
            
            var userFollow = await UserFollowsAppService.GetWithNavigationPropertiesAsync(input.UserFollow.Id);
            
            EditingUserFollowId = userFollow.UserFollow.Id;
            EditingUserFollow = ObjectMapper.Map<UserFollowDto, UserFollowUpdateDto>(userFollow.UserFollow);
            
            await EditingUserFollowValidations.ClearAll();
            await EditUserFollowModal.Show();
        }

        private async Task DeleteUserFollowAsync(UserFollowWithNavigationPropertiesDto input)
        {
            await UserFollowsAppService.DeleteAsync(input.UserFollow.Id);
            await GetUserFollowsAsync();
        }

        private async Task CreateUserFollowAsync()
        {
            try
            {
                if (await NewUserFollowValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserFollowsAppService.CreateAsync(NewUserFollow);
                await GetUserFollowsAsync();
                await CloseCreateUserFollowModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserFollowModalAsync()
        {
            await EditUserFollowModal.Hide();
        }

        private async Task UpdateUserFollowAsync()
        {
            try
            {
                if (await EditingUserFollowValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserFollowsAppService.UpdateAsync(EditingUserFollowId, EditingUserFollow);
                await GetUserFollowsAsync();
                await EditUserFollowModal.Hide();                
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









        protected virtual async Task OnFollowerUserIdChangedAsync(Guid? followerUserId)
        {
            Filter.FollowerUserId = followerUserId;
            await SearchAsync();
        }
        protected virtual async Task OnFollowingUserIdChangedAsync(Guid? followingUserId)
        {
            Filter.FollowingUserId = followingUserId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await UserFollowsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllUserFollowsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserFollowsSelected = false;
            SelectedUserFollows.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserFollowRowsChanged()
        {
            if (SelectedUserFollows.Count != PageSize)
            {
                AllUserFollowsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserFollowsAsync()
        {
            var message = AllUserFollowsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserFollows.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserFollowsSelected)
            {
                await UserFollowsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserFollowsAppService.DeleteByIdsAsync(SelectedUserFollows.Select(x => x.UserFollow.Id).ToList());
            }

            SelectedUserFollows.Clear();
            AllUserFollowsSelected = false;

            await GetUserFollowsAsync();
        }


    }
}
