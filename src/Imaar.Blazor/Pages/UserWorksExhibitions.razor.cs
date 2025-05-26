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
using Imaar.UserWorksExhibitions;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class UserWorksExhibitions
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserWorksExhibitionWithNavigationPropertiesDto> UserWorksExhibitionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserWorksExhibition { get; set; }
        private bool CanEditUserWorksExhibition { get; set; }
        private bool CanDeleteUserWorksExhibition { get; set; }
        private UserWorksExhibitionCreateDto NewUserWorksExhibition { get; set; }
        private Validations NewUserWorksExhibitionValidations { get; set; } = new();
        private UserWorksExhibitionUpdateDto EditingUserWorksExhibition { get; set; }
        private Validations EditingUserWorksExhibitionValidations { get; set; } = new();
        private Guid EditingUserWorksExhibitionId { get; set; }
        private Modal CreateUserWorksExhibitionModal { get; set; } = new();
        private Modal EditUserWorksExhibitionModal { get; set; } = new();
        private GetUserWorksExhibitionsInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserWorksExhibitionWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userWorksExhibition-create-tab";
        protected string SelectedEditTab = "userWorksExhibition-edit-tab";
        private UserWorksExhibitionWithNavigationPropertiesDto? SelectedUserWorksExhibition;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<UserWorksExhibitionWithNavigationPropertiesDto> SelectedUserWorksExhibitions { get; set; } = new();
        private bool AllUserWorksExhibitionsSelected { get; set; }
        
        public UserWorksExhibitions()
        {
            NewUserWorksExhibition = new UserWorksExhibitionCreateDto();
            EditingUserWorksExhibition = new UserWorksExhibitionUpdateDto();
            Filter = new GetUserWorksExhibitionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserWorksExhibitionList = new List<UserWorksExhibitionWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserWorksExhibitions"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserWorksExhibition"], async () =>
            {
                await OpenCreateUserWorksExhibitionModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.UserWorksExhibitions.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserWorksExhibition = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.UserWorksExhibitions.Create);
            CanEditUserWorksExhibition = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserWorksExhibitions.Edit);
            CanDeleteUserWorksExhibition = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserWorksExhibitions.Delete);
                            
                            
        }

        private async Task GetUserWorksExhibitionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserWorksExhibitionsAppService.GetListAsync(Filter);
            UserWorksExhibitionList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserWorksExhibitionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserWorksExhibitionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/user-works-exhibitions/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&File={HttpUtility.UrlEncode(Filter.File)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&UserProfileId={Filter.UserProfileId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserWorksExhibitionWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserWorksExhibitionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserWorksExhibitionModalAsync()
        {
            NewUserWorksExhibition = new UserWorksExhibitionCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "userWorksExhibition-create-tab";
            
            
            await NewUserWorksExhibitionValidations.ClearAll();
            await CreateUserWorksExhibitionModal.Show();
        }

        private async Task CloseCreateUserWorksExhibitionModalAsync()
        {
            NewUserWorksExhibition = new UserWorksExhibitionCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateUserWorksExhibitionModal.Hide();
        }

        private async Task OpenEditUserWorksExhibitionModalAsync(UserWorksExhibitionWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "userWorksExhibition-edit-tab";
            
            
            var userWorksExhibition = await UserWorksExhibitionsAppService.GetWithNavigationPropertiesAsync(input.UserWorksExhibition.Id);
            
            EditingUserWorksExhibitionId = userWorksExhibition.UserWorksExhibition.Id;
            EditingUserWorksExhibition = ObjectMapper.Map<UserWorksExhibitionDto, UserWorksExhibitionUpdateDto>(userWorksExhibition.UserWorksExhibition);
            
            await EditingUserWorksExhibitionValidations.ClearAll();
            await EditUserWorksExhibitionModal.Show();
        }

        private async Task DeleteUserWorksExhibitionAsync(UserWorksExhibitionWithNavigationPropertiesDto input)
        {
            await UserWorksExhibitionsAppService.DeleteAsync(input.UserWorksExhibition.Id);
            await GetUserWorksExhibitionsAsync();
        }

        private async Task CreateUserWorksExhibitionAsync()
        {
            try
            {
                if (await NewUserWorksExhibitionValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserWorksExhibitionsAppService.CreateAsync(NewUserWorksExhibition);
                await GetUserWorksExhibitionsAsync();
                await CloseCreateUserWorksExhibitionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserWorksExhibitionModalAsync()
        {
            await EditUserWorksExhibitionModal.Hide();
        }

        private async Task UpdateUserWorksExhibitionAsync()
        {
            try
            {
                if (await EditingUserWorksExhibitionValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserWorksExhibitionsAppService.UpdateAsync(EditingUserWorksExhibitionId, EditingUserWorksExhibition);
                await GetUserWorksExhibitionsAsync();
                await EditUserWorksExhibitionModal.Hide();                
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
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await UserWorksExhibitionsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllUserWorksExhibitionsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserWorksExhibitionsSelected = false;
            SelectedUserWorksExhibitions.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserWorksExhibitionRowsChanged()
        {
            if (SelectedUserWorksExhibitions.Count != PageSize)
            {
                AllUserWorksExhibitionsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserWorksExhibitionsAsync()
        {
            var message = AllUserWorksExhibitionsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserWorksExhibitions.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserWorksExhibitionsSelected)
            {
                await UserWorksExhibitionsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserWorksExhibitionsAppService.DeleteByIdsAsync(SelectedUserWorksExhibitions.Select(x => x.UserWorksExhibition.Id).ToList());
            }

            SelectedUserWorksExhibitions.Clear();
            AllUserWorksExhibitionsSelected = false;

            await GetUserWorksExhibitionsAsync();
        }


    }
}
