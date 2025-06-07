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
using Imaar.UserProfiles;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

using Imaar.UserProfiles;



namespace Imaar.Blazor.Pages
{
    public partial class UserProfiles
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserProfileDto> UserProfileList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserProfile { get; set; }
        private bool CanEditUserProfile { get; set; }
        private bool CanDeleteUserProfile { get; set; }
        private UserProfileCreateDto NewUserProfile { get; set; }
        private Validations NewUserProfileValidations { get; set; } = new();
        private UserProfileUpdateDto EditingUserProfile { get; set; }
        private Validations EditingUserProfileValidations { get; set; } = new();
        private Guid EditingUserProfileId { get; set; }
        private Modal CreateUserProfileModal { get; set; } = new();
        private Modal EditUserProfileModal { get; set; } = new();
        private GetUserProfilesInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserProfileDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userProfile-create-tab";
        protected string SelectedEditTab = "userProfile-edit-tab";
        private UserProfileDto? SelectedUserProfile;
        
        
        
        
        
        private List<UserProfileDto> SelectedUserProfiles { get; set; } = new();
        private bool AllUserProfilesSelected { get; set; }
        
        public UserProfiles()
        {
            NewUserProfile = new UserProfileCreateDto();
            EditingUserProfile = new UserProfileUpdateDto();
            Filter = new GetUserProfilesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserProfileList = new List<UserProfileDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserProfiles"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserProfile"], async () =>
            {
                await OpenCreateUserProfileModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.UserProfiles.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserProfile = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.UserProfiles.Create);
            CanEditUserProfile = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserProfiles.Edit);
            CanDeleteUserProfile = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserProfiles.Delete);
                            
                            
        }

        private async Task GetUserProfilesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserProfilesAppService.GetListAsync(Filter);
            UserProfileList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserProfilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserProfilesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/user-profiles/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&SecurityNumber={HttpUtility.UrlEncode(Filter.SecurityNumber)}&BiologicalSex={Filter.BiologicalSex}&DateOfBirthMin={Filter.DateOfBirthMin}&DateOfBirthMax={Filter.DateOfBirthMax}&Latitude={HttpUtility.UrlEncode(Filter.Latitude)}&Longitude={HttpUtility.UrlEncode(Filter.Longitude)}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&Email={HttpUtility.UrlEncode(Filter.Email)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserProfileDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserProfilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserProfileModalAsync()
        {
            NewUserProfile = new UserProfileCreateDto{
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now),

                
            };

            SelectedCreateTab = "userProfile-create-tab";
            
            
            await NewUserProfileValidations.ClearAll();
            await CreateUserProfileModal.Show();
        }

        private async Task CloseCreateUserProfileModalAsync()
        {
            NewUserProfile = new UserProfileCreateDto{
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now),

                
            };
            await CreateUserProfileModal.Hide();
        }

        private async Task OpenEditUserProfileModalAsync(UserProfileDto input)
        {
            SelectedEditTab = "userProfile-edit-tab";
            
            
            var userProfile = await UserProfilesAppService.GetAsync(input.Id);
            
            EditingUserProfileId = userProfile.Id;
            EditingUserProfile = ObjectMapper.Map<UserProfileDto, UserProfileUpdateDto>(userProfile);
            
            await EditingUserProfileValidations.ClearAll();
            await EditUserProfileModal.Show();
        }

        private async Task DeleteUserProfileAsync(UserProfileDto input)
        {
            await UserProfilesAppService.DeleteAsync(input.Id);
            await GetUserProfilesAsync();
        }

        private async Task CreateUserProfileAsync()
        {
            try
            {
                if (await NewUserProfileValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserProfilesAppService.CreateAsync(NewUserProfile);
                await GetUserProfilesAsync();
                await CloseCreateUserProfileModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserProfileModalAsync()
        {
            await EditUserProfileModal.Hide();
        }

        private async Task UpdateUserProfileAsync()
        {
            try
            {
                if (await EditingUserProfileValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserProfilesAppService.UpdateAsync(EditingUserProfileId, EditingUserProfile);
                await GetUserProfilesAsync();
                await EditUserProfileModal.Hide();                
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









        protected virtual async Task OnSecurityNumberChangedAsync(string? securityNumber)
        {
            Filter.SecurityNumber = securityNumber;
            await SearchAsync();
        }
        protected virtual async Task OnBiologicalSexChangedAsync(BiologicalSex? biologicalSex)
        {
            Filter.BiologicalSex = biologicalSex;
            await SearchAsync();
        }
        protected virtual async Task OnDateOfBirthMinChangedAsync(DateOnly? dateOfBirthMin)
        {
            Filter.DateOfBirthMin = dateOfBirthMin;
            await SearchAsync();
        }
        protected virtual async Task OnDateOfBirthMaxChangedAsync(DateOnly? dateOfBirthMax)
        {
            Filter.DateOfBirthMax = dateOfBirthMax;
            await SearchAsync();
        }
        protected virtual async Task OnLatitudeChangedAsync(string? latitude)
        {
            Filter.Latitude = latitude;
            await SearchAsync();
        }
        protected virtual async Task OnLongitudeChangedAsync(string? longitude)
        {
            Filter.Longitude = longitude;
            await SearchAsync();
        }
        protected virtual async Task OnFirstNameChangedAsync(string? firstName)
        {
            Filter.FirstName = firstName;
            await SearchAsync();
        }
        protected virtual async Task OnLastNameChangedAsync(string? lastName)
        {
            Filter.LastName = lastName;
            await SearchAsync();
        }
        protected virtual async Task OnPhoneNumberChangedAsync(string? phoneNumber)
        {
            Filter.PhoneNumber = phoneNumber;
            await SearchAsync();
        }
        protected virtual async Task OnEmailChangedAsync(string? email)
        {
            Filter.Email = email;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllUserProfilesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserProfilesSelected = false;
            SelectedUserProfiles.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserProfileRowsChanged()
        {
            if (SelectedUserProfiles.Count != PageSize)
            {
                AllUserProfilesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserProfilesAsync()
        {
            var message = AllUserProfilesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserProfiles.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserProfilesSelected)
            {
                await UserProfilesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserProfilesAppService.DeleteByIdsAsync(SelectedUserProfiles.Select(x => x.Id).ToList());
            }

            SelectedUserProfiles.Clear();
            AllUserProfilesSelected = false;

            await GetUserProfilesAsync();
        }


    }
}
