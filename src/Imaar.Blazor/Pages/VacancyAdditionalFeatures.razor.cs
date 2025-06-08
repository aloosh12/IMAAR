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
using Imaar.VacancyAdditionalFeatures;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class VacancyAdditionalFeatures
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<VacancyAdditionalFeatureDto> VacancyAdditionalFeatureList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateVacancyAdditionalFeature { get; set; }
        private bool CanEditVacancyAdditionalFeature { get; set; }
        private bool CanDeleteVacancyAdditionalFeature { get; set; }
        private VacancyAdditionalFeatureCreateDto NewVacancyAdditionalFeature { get; set; }
        private Validations NewVacancyAdditionalFeatureValidations { get; set; } = new();
        private VacancyAdditionalFeatureUpdateDto EditingVacancyAdditionalFeature { get; set; }
        private Validations EditingVacancyAdditionalFeatureValidations { get; set; } = new();
        private Guid EditingVacancyAdditionalFeatureId { get; set; }
        private Modal CreateVacancyAdditionalFeatureModal { get; set; } = new();
        private Modal EditVacancyAdditionalFeatureModal { get; set; } = new();
        private GetVacancyAdditionalFeaturesInput Filter { get; set; }
        private DataGridEntityActionsColumn<VacancyAdditionalFeatureDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "vacancyAdditionalFeature-create-tab";
        protected string SelectedEditTab = "vacancyAdditionalFeature-edit-tab";
        private VacancyAdditionalFeatureDto? SelectedVacancyAdditionalFeature;
        
        
        
        
        
        private List<VacancyAdditionalFeatureDto> SelectedVacancyAdditionalFeatures { get; set; } = new();
        private bool AllVacancyAdditionalFeaturesSelected { get; set; }
        
        public VacancyAdditionalFeatures()
        {
            NewVacancyAdditionalFeature = new VacancyAdditionalFeatureCreateDto();
            EditingVacancyAdditionalFeature = new VacancyAdditionalFeatureUpdateDto();
            Filter = new GetVacancyAdditionalFeaturesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            VacancyAdditionalFeatureList = new List<VacancyAdditionalFeatureDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["VacancyAdditionalFeatures"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewVacancyAdditionalFeature"], async () =>
            {
                await OpenCreateVacancyAdditionalFeatureModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.VacancyAdditionalFeatures.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateVacancyAdditionalFeature = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.VacancyAdditionalFeatures.Create);
            CanEditVacancyAdditionalFeature = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.VacancyAdditionalFeatures.Edit);
            CanDeleteVacancyAdditionalFeature = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.VacancyAdditionalFeatures.Delete);
                            
                            
        }

        private async Task GetVacancyAdditionalFeaturesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await VacancyAdditionalFeaturesAppService.GetListAsync(Filter);
            VacancyAdditionalFeatureList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetVacancyAdditionalFeaturesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await VacancyAdditionalFeaturesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/vacancy-additional-features/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<VacancyAdditionalFeatureDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetVacancyAdditionalFeaturesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateVacancyAdditionalFeatureModalAsync()
        {
            NewVacancyAdditionalFeature = new VacancyAdditionalFeatureCreateDto{
                
                
            };

            SelectedCreateTab = "vacancyAdditionalFeature-create-tab";
            
            
            await NewVacancyAdditionalFeatureValidations.ClearAll();
            await CreateVacancyAdditionalFeatureModal.Show();
        }

        private async Task CloseCreateVacancyAdditionalFeatureModalAsync()
        {
            NewVacancyAdditionalFeature = new VacancyAdditionalFeatureCreateDto{
                
                
            };
            await CreateVacancyAdditionalFeatureModal.Hide();
        }

        private async Task OpenEditVacancyAdditionalFeatureModalAsync(VacancyAdditionalFeatureDto input)
        {
            SelectedEditTab = "vacancyAdditionalFeature-edit-tab";
            
            
            var vacancyAdditionalFeature = await VacancyAdditionalFeaturesAppService.GetAsync(input.Id);
            
            EditingVacancyAdditionalFeatureId = vacancyAdditionalFeature.Id;
            EditingVacancyAdditionalFeature = ObjectMapper.Map<VacancyAdditionalFeatureDto, VacancyAdditionalFeatureUpdateDto>(vacancyAdditionalFeature);
            
            await EditingVacancyAdditionalFeatureValidations.ClearAll();
            await EditVacancyAdditionalFeatureModal.Show();
        }

        private async Task DeleteVacancyAdditionalFeatureAsync(VacancyAdditionalFeatureDto input)
        {
            await VacancyAdditionalFeaturesAppService.DeleteAsync(input.Id);
            await GetVacancyAdditionalFeaturesAsync();
        }

        private async Task CreateVacancyAdditionalFeatureAsync()
        {
            try
            {
                if (await NewVacancyAdditionalFeatureValidations.ValidateAll() == false)
                {
                    return;
                }

                await VacancyAdditionalFeaturesAppService.CreateAsync(NewVacancyAdditionalFeature);
                await GetVacancyAdditionalFeaturesAsync();
                await CloseCreateVacancyAdditionalFeatureModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditVacancyAdditionalFeatureModalAsync()
        {
            await EditVacancyAdditionalFeatureModal.Hide();
        }

        private async Task UpdateVacancyAdditionalFeatureAsync()
        {
            try
            {
                if (await EditingVacancyAdditionalFeatureValidations.ValidateAll() == false)
                {
                    return;
                }

                await VacancyAdditionalFeaturesAppService.UpdateAsync(EditingVacancyAdditionalFeatureId, EditingVacancyAdditionalFeature);
                await GetVacancyAdditionalFeaturesAsync();
                await EditVacancyAdditionalFeatureModal.Hide();                
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









        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
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
        





        private Task SelectAllItems()
        {
            AllVacancyAdditionalFeaturesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllVacancyAdditionalFeaturesSelected = false;
            SelectedVacancyAdditionalFeatures.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedVacancyAdditionalFeatureRowsChanged()
        {
            if (SelectedVacancyAdditionalFeatures.Count != PageSize)
            {
                AllVacancyAdditionalFeaturesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedVacancyAdditionalFeaturesAsync()
        {
            var message = AllVacancyAdditionalFeaturesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedVacancyAdditionalFeatures.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllVacancyAdditionalFeaturesSelected)
            {
                await VacancyAdditionalFeaturesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await VacancyAdditionalFeaturesAppService.DeleteByIdsAsync(SelectedVacancyAdditionalFeatures.Select(x => x.Id).ToList());
            }

            SelectedVacancyAdditionalFeatures.Clear();
            AllVacancyAdditionalFeaturesSelected = false;

            await GetVacancyAdditionalFeaturesAsync();
        }


    }
}
