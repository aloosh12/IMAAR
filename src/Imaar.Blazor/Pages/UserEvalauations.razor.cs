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
using Imaar.UserEvalauations;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class UserEvalauations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserEvalauationWithNavigationPropertiesDto> UserEvalauationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserEvalauation { get; set; }
        private bool CanEditUserEvalauation { get; set; }
        private bool CanDeleteUserEvalauation { get; set; }
        private UserEvalauationCreateDto NewUserEvalauation { get; set; }
        private Validations NewUserEvalauationValidations { get; set; } = new();
        private UserEvalauationUpdateDto EditingUserEvalauation { get; set; }
        private Validations EditingUserEvalauationValidations { get; set; } = new();
        private Guid EditingUserEvalauationId { get; set; }
        private Modal CreateUserEvalauationModal { get; set; } = new();
        private Modal EditUserEvalauationModal { get; set; } = new();
        private GetUserEvalauationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserEvalauationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userEvalauation-create-tab";
        protected string SelectedEditTab = "userEvalauation-edit-tab";
        private UserEvalauationWithNavigationPropertiesDto? SelectedUserEvalauation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<UserEvalauationWithNavigationPropertiesDto> SelectedUserEvalauations { get; set; } = new();
        private bool AllUserEvalauationsSelected { get; set; }
        
        public UserEvalauations()
        {
            NewUserEvalauation = new UserEvalauationCreateDto();
            EditingUserEvalauation = new UserEvalauationUpdateDto();
            Filter = new GetUserEvalauationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserEvalauationList = new List<UserEvalauationWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserEvalauations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserEvalauation"], async () =>
            {
                await OpenCreateUserEvalauationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.UserEvalauations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserEvalauation = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.UserEvalauations.Create);
            CanEditUserEvalauation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserEvalauations.Edit);
            CanDeleteUserEvalauation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.UserEvalauations.Delete);
                            
                            
        }

        private async Task GetUserEvalauationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserEvalauationsAppService.GetListAsync(Filter);
            UserEvalauationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserEvalauationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserEvalauationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/userEvalauations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&SpeedOfCompletionMin={Filter.SpeedOfCompletionMin}&SpeedOfCompletionMax={Filter.SpeedOfCompletionMax}&DealingMin={Filter.DealingMin}&DealingMax={Filter.DealingMax}&CleanlinessMin={Filter.CleanlinessMin}&CleanlinessMax={Filter.CleanlinessMax}&PerfectionMin={Filter.PerfectionMin}&PerfectionMax={Filter.PerfectionMax}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&Evaluatord={Filter.Evaluatord}&EvaluatedPersonId={Filter.EvaluatedPersonId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserEvalauationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserEvalauationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserEvalauationModalAsync()
        {
            NewUserEvalauation = new UserEvalauationCreateDto{
                
                Evaluatord = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "userEvalauation-create-tab";
            
            
            await NewUserEvalauationValidations.ClearAll();
            await CreateUserEvalauationModal.Show();
        }

        private async Task CloseCreateUserEvalauationModalAsync()
        {
            NewUserEvalauation = new UserEvalauationCreateDto{
                
                Evaluatord = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateUserEvalauationModal.Hide();
        }

        private async Task OpenEditUserEvalauationModalAsync(UserEvalauationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "userEvalauation-edit-tab";
            
            
            var userEvalauation = await UserEvalauationsAppService.GetWithNavigationPropertiesAsync(input.UserEvalauation.Id);
            
            EditingUserEvalauationId = userEvalauation.UserEvalauation.Id;
            EditingUserEvalauation = ObjectMapper.Map<UserEvalauationDto, UserEvalauationUpdateDto>(userEvalauation.UserEvalauation);
            
            await EditingUserEvalauationValidations.ClearAll();
            await EditUserEvalauationModal.Show();
        }

        private async Task DeleteUserEvalauationAsync(UserEvalauationWithNavigationPropertiesDto input)
        {
            await UserEvalauationsAppService.DeleteAsync(input.UserEvalauation.Id);
            await GetUserEvalauationsAsync();
        }

        private async Task CreateUserEvalauationAsync()
        {
            try
            {
                if (await NewUserEvalauationValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserEvalauationsAppService.CreateAsync(NewUserEvalauation);
                await GetUserEvalauationsAsync();
                await CloseCreateUserEvalauationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserEvalauationModalAsync()
        {
            await EditUserEvalauationModal.Hide();
        }

        private async Task UpdateUserEvalauationAsync()
        {
            try
            {
                if (await EditingUserEvalauationValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserEvalauationsAppService.UpdateAsync(EditingUserEvalauationId, EditingUserEvalauation);
                await GetUserEvalauationsAsync();
                await EditUserEvalauationModal.Hide();                
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









        protected virtual async Task OnSpeedOfCompletionMinChangedAsync(int? speedOfCompletionMin)
        {
            Filter.SpeedOfCompletionMin = speedOfCompletionMin;
            await SearchAsync();
        }
        protected virtual async Task OnSpeedOfCompletionMaxChangedAsync(int? speedOfCompletionMax)
        {
            Filter.SpeedOfCompletionMax = speedOfCompletionMax;
            await SearchAsync();
        }
        protected virtual async Task OnDealingMinChangedAsync(int? dealingMin)
        {
            Filter.DealingMin = dealingMin;
            await SearchAsync();
        }
        protected virtual async Task OnDealingMaxChangedAsync(int? dealingMax)
        {
            Filter.DealingMax = dealingMax;
            await SearchAsync();
        }
        protected virtual async Task OnCleanlinessMinChangedAsync(int? cleanlinessMin)
        {
            Filter.CleanlinessMin = cleanlinessMin;
            await SearchAsync();
        }
        protected virtual async Task OnCleanlinessMaxChangedAsync(int? cleanlinessMax)
        {
            Filter.CleanlinessMax = cleanlinessMax;
            await SearchAsync();
        }
        protected virtual async Task OnPerfectionMinChangedAsync(int? perfectionMin)
        {
            Filter.PerfectionMin = perfectionMin;
            await SearchAsync();
        }
        protected virtual async Task OnPerfectionMaxChangedAsync(int? perfectionMax)
        {
            Filter.PerfectionMax = perfectionMax;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMinChangedAsync(int? priceMin)
        {
            Filter.PriceMin = priceMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMaxChangedAsync(int? priceMax)
        {
            Filter.PriceMax = priceMax;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatordChangedAsync(Guid? evaluatord)
        {
            Filter.Evaluatord = evaluatord;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatedPersonIdChangedAsync(Guid? evaluatedPersonId)
        {
            Filter.EvaluatedPersonId = evaluatedPersonId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await UserEvalauationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllUserEvalauationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserEvalauationsSelected = false;
            SelectedUserEvalauations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserEvalauationRowsChanged()
        {
            if (SelectedUserEvalauations.Count != PageSize)
            {
                AllUserEvalauationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserEvalauationsAsync()
        {
            var message = AllUserEvalauationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserEvalauations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserEvalauationsSelected)
            {
                await UserEvalauationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserEvalauationsAppService.DeleteByIdsAsync(SelectedUserEvalauations.Select(x => x.UserEvalauation.Id).ToList());
            }

            SelectedUserEvalauations.Clear();
            AllUserEvalauationsSelected = false;

            await GetUserEvalauationsAsync();
        }


    }
}
