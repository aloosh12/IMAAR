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
using Imaar.VacancyEvaluations;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class VacancyEvaluations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<VacancyEvaluationWithNavigationPropertiesDto> VacancyEvaluationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateVacancyEvaluation { get; set; }
        private bool CanEditVacancyEvaluation { get; set; }
        private bool CanDeleteVacancyEvaluation { get; set; }
        private VacancyEvaluationCreateDto NewVacancyEvaluation { get; set; }
        private Validations NewVacancyEvaluationValidations { get; set; } = new();
        private VacancyEvaluationUpdateDto EditingVacancyEvaluation { get; set; }
        private Validations EditingVacancyEvaluationValidations { get; set; } = new();
        private Guid EditingVacancyEvaluationId { get; set; }
        private Modal CreateVacancyEvaluationModal { get; set; } = new();
        private Modal EditVacancyEvaluationModal { get; set; } = new();
        private GetVacancyEvaluationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<VacancyEvaluationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "vacancyEvaluation-create-tab";
        protected string SelectedEditTab = "vacancyEvaluation-edit-tab";
        private VacancyEvaluationWithNavigationPropertiesDto? SelectedVacancyEvaluation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> VacanciesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<VacancyEvaluationWithNavigationPropertiesDto> SelectedVacancyEvaluations { get; set; } = new();
        private bool AllVacancyEvaluationsSelected { get; set; }
        
        public VacancyEvaluations()
        {
            NewVacancyEvaluation = new VacancyEvaluationCreateDto();
            EditingVacancyEvaluation = new VacancyEvaluationUpdateDto();
            Filter = new GetVacancyEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            VacancyEvaluationList = new List<VacancyEvaluationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            await GetVacancyCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["VacancyEvaluations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewVacancyEvaluation"], async () =>
            {
                await OpenCreateVacancyEvaluationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.VacancyEvaluations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateVacancyEvaluation = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.VacancyEvaluations.Create);
            CanEditVacancyEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.VacancyEvaluations.Edit);
            CanDeleteVacancyEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.VacancyEvaluations.Delete);
                            
                            
        }

        private async Task GetVacancyEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await VacancyEvaluationsAppService.GetListAsync(Filter);
            VacancyEvaluationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetVacancyEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await VacancyEvaluationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/vacancy-evaluations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&RateMin={Filter.RateMin}&RateMax={Filter.RateMax}&UserProfileId={Filter.UserProfileId}&VacancyId={Filter.VacancyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<VacancyEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetVacancyEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateVacancyEvaluationModalAsync()
        {
            NewVacancyEvaluation = new VacancyEvaluationCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
VacancyId = VacanciesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "vacancyEvaluation-create-tab";
            
            
            await NewVacancyEvaluationValidations.ClearAll();
            await CreateVacancyEvaluationModal.Show();
        }

        private async Task CloseCreateVacancyEvaluationModalAsync()
        {
            NewVacancyEvaluation = new VacancyEvaluationCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
VacancyId = VacanciesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateVacancyEvaluationModal.Hide();
        }

        private async Task OpenEditVacancyEvaluationModalAsync(VacancyEvaluationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "vacancyEvaluation-edit-tab";
            
            
            var vacancyEvaluation = await VacancyEvaluationsAppService.GetWithNavigationPropertiesAsync(input.VacancyEvaluation.Id);
            
            EditingVacancyEvaluationId = vacancyEvaluation.VacancyEvaluation.Id;
            EditingVacancyEvaluation = ObjectMapper.Map<VacancyEvaluationDto, VacancyEvaluationUpdateDto>(vacancyEvaluation.VacancyEvaluation);
            
            await EditingVacancyEvaluationValidations.ClearAll();
            await EditVacancyEvaluationModal.Show();
        }

        private async Task DeleteVacancyEvaluationAsync(VacancyEvaluationWithNavigationPropertiesDto input)
        {
            await VacancyEvaluationsAppService.DeleteAsync(input.VacancyEvaluation.Id);
            await GetVacancyEvaluationsAsync();
        }

        private async Task CreateVacancyEvaluationAsync()
        {
            try
            {
                if (await NewVacancyEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await VacancyEvaluationsAppService.CreateAsync(NewVacancyEvaluation);
                await GetVacancyEvaluationsAsync();
                await CloseCreateVacancyEvaluationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditVacancyEvaluationModalAsync()
        {
            await EditVacancyEvaluationModal.Hide();
        }

        private async Task UpdateVacancyEvaluationAsync()
        {
            try
            {
                if (await EditingVacancyEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await VacancyEvaluationsAppService.UpdateAsync(EditingVacancyEvaluationId, EditingVacancyEvaluation);
                await GetVacancyEvaluationsAsync();
                await EditVacancyEvaluationModal.Hide();                
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









        protected virtual async Task OnRateMinChangedAsync(int? rateMin)
        {
            Filter.RateMin = rateMin;
            await SearchAsync();
        }
        protected virtual async Task OnRateMaxChangedAsync(int? rateMax)
        {
            Filter.RateMax = rateMax;
            await SearchAsync();
        }
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        protected virtual async Task OnVacancyIdChangedAsync(Guid? vacancyId)
        {
            Filter.VacancyId = vacancyId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await VacancyEvaluationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetVacancyCollectionLookupAsync(string? newValue = null)
        {
            VacanciesCollection = (await VacancyEvaluationsAppService.GetVacancyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllVacancyEvaluationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllVacancyEvaluationsSelected = false;
            SelectedVacancyEvaluations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedVacancyEvaluationRowsChanged()
        {
            if (SelectedVacancyEvaluations.Count != PageSize)
            {
                AllVacancyEvaluationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedVacancyEvaluationsAsync()
        {
            var message = AllVacancyEvaluationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedVacancyEvaluations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllVacancyEvaluationsSelected)
            {
                await VacancyEvaluationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await VacancyEvaluationsAppService.DeleteByIdsAsync(SelectedVacancyEvaluations.Select(x => x.VacancyEvaluation.Id).ToList());
            }

            SelectedVacancyEvaluations.Clear();
            AllVacancyEvaluationsSelected = false;

            await GetVacancyEvaluationsAsync();
        }


    }
}
