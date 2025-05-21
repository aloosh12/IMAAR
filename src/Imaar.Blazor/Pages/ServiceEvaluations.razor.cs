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
using Imaar.ServiceEvaluations;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class ServiceEvaluations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ServiceEvaluationWithNavigationPropertiesDto> ServiceEvaluationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateServiceEvaluation { get; set; }
        private bool CanEditServiceEvaluation { get; set; }
        private bool CanDeleteServiceEvaluation { get; set; }
        private ServiceEvaluationCreateDto NewServiceEvaluation { get; set; }
        private Validations NewServiceEvaluationValidations { get; set; } = new();
        private ServiceEvaluationUpdateDto EditingServiceEvaluation { get; set; }
        private Validations EditingServiceEvaluationValidations { get; set; } = new();
        private Guid EditingServiceEvaluationId { get; set; }
        private Modal CreateServiceEvaluationModal { get; set; } = new();
        private Modal EditServiceEvaluationModal { get; set; } = new();
        private GetServiceEvaluationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ServiceEvaluationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "serviceEvaluation-create-tab";
        protected string SelectedEditTab = "serviceEvaluation-edit-tab";
        private ServiceEvaluationWithNavigationPropertiesDto? SelectedServiceEvaluation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> ImaarServicesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<ServiceEvaluationWithNavigationPropertiesDto> SelectedServiceEvaluations { get; set; } = new();
        private bool AllServiceEvaluationsSelected { get; set; }
        
        public ServiceEvaluations()
        {
            NewServiceEvaluation = new ServiceEvaluationCreateDto();
            EditingServiceEvaluation = new ServiceEvaluationUpdateDto();
            Filter = new GetServiceEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ServiceEvaluationList = new List<ServiceEvaluationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            await GetImaarServiceCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ServiceEvaluations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewServiceEvaluation"], async () =>
            {
                await OpenCreateServiceEvaluationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.ServiceEvaluations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateServiceEvaluation = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceEvaluations.Create);
            CanEditServiceEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceEvaluations.Edit);
            CanDeleteServiceEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceEvaluations.Delete);
                            
                            
        }

        private async Task GetServiceEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ServiceEvaluationsAppService.GetListAsync(Filter);
            ServiceEvaluationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetServiceEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ServiceEvaluationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/service-evaluations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&RateMin={Filter.RateMin}&RateMax={Filter.RateMax}&EvaluatorId={Filter.EvaluatorId}&ImaarServiceId={Filter.ImaarServiceId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ServiceEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetServiceEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateServiceEvaluationModalAsync()
        {
            NewServiceEvaluation = new ServiceEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
ImaarServiceId = ImaarServicesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "serviceEvaluation-create-tab";
            
            
            await NewServiceEvaluationValidations.ClearAll();
            await CreateServiceEvaluationModal.Show();
        }

        private async Task CloseCreateServiceEvaluationModalAsync()
        {
            NewServiceEvaluation = new ServiceEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
ImaarServiceId = ImaarServicesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateServiceEvaluationModal.Hide();
        }

        private async Task OpenEditServiceEvaluationModalAsync(ServiceEvaluationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "serviceEvaluation-edit-tab";
            
            
            var serviceEvaluation = await ServiceEvaluationsAppService.GetWithNavigationPropertiesAsync(input.ServiceEvaluation.Id);
            
            EditingServiceEvaluationId = serviceEvaluation.ServiceEvaluation.Id;
            EditingServiceEvaluation = ObjectMapper.Map<ServiceEvaluationDto, ServiceEvaluationUpdateDto>(serviceEvaluation.ServiceEvaluation);
            
            await EditingServiceEvaluationValidations.ClearAll();
            await EditServiceEvaluationModal.Show();
        }

        private async Task DeleteServiceEvaluationAsync(ServiceEvaluationWithNavigationPropertiesDto input)
        {
            await ServiceEvaluationsAppService.DeleteAsync(input.ServiceEvaluation.Id);
            await GetServiceEvaluationsAsync();
        }

        private async Task CreateServiceEvaluationAsync()
        {
            try
            {
                if (await NewServiceEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceEvaluationsAppService.CreateAsync(NewServiceEvaluation);
                await GetServiceEvaluationsAsync();
                await CloseCreateServiceEvaluationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditServiceEvaluationModalAsync()
        {
            await EditServiceEvaluationModal.Hide();
        }

        private async Task UpdateServiceEvaluationAsync()
        {
            try
            {
                if (await EditingServiceEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceEvaluationsAppService.UpdateAsync(EditingServiceEvaluationId, EditingServiceEvaluation);
                await GetServiceEvaluationsAsync();
                await EditServiceEvaluationModal.Hide();                
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
        protected virtual async Task OnEvaluatorIdChangedAsync(Guid? evaluatorId)
        {
            Filter.EvaluatorId = evaluatorId;
            await SearchAsync();
        }
        protected virtual async Task OnImaarServiceIdChangedAsync(Guid? imaarServiceId)
        {
            Filter.ImaarServiceId = imaarServiceId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await ServiceEvaluationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetImaarServiceCollectionLookupAsync(string? newValue = null)
        {
            ImaarServicesCollection = (await ServiceEvaluationsAppService.GetImaarServiceLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllServiceEvaluationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllServiceEvaluationsSelected = false;
            SelectedServiceEvaluations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedServiceEvaluationRowsChanged()
        {
            if (SelectedServiceEvaluations.Count != PageSize)
            {
                AllServiceEvaluationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedServiceEvaluationsAsync()
        {
            var message = AllServiceEvaluationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedServiceEvaluations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllServiceEvaluationsSelected)
            {
                await ServiceEvaluationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ServiceEvaluationsAppService.DeleteByIdsAsync(SelectedServiceEvaluations.Select(x => x.ServiceEvaluation.Id).ToList());
            }

            SelectedServiceEvaluations.Clear();
            AllServiceEvaluationsSelected = false;

            await GetServiceEvaluationsAsync();
        }


    }
}
