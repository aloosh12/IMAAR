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
using Imaar.Regions;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Regions
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<RegionWithNavigationPropertiesDto> RegionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateRegion { get; set; }
        private bool CanEditRegion { get; set; }
        private bool CanDeleteRegion { get; set; }
        private RegionCreateDto NewRegion { get; set; }
        private Validations NewRegionValidations { get; set; } = new();
        private RegionUpdateDto EditingRegion { get; set; }
        private Validations EditingRegionValidations { get; set; } = new();
        private Guid EditingRegionId { get; set; }
        private Modal CreateRegionModal { get; set; } = new();
        private Modal EditRegionModal { get; set; } = new();
        private GetRegionsInput Filter { get; set; }
        private DataGridEntityActionsColumn<RegionWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "region-create-tab";
        protected string SelectedEditTab = "region-edit-tab";
        private RegionWithNavigationPropertiesDto? SelectedRegion;
        private IReadOnlyList<LookupDto<Guid>> CitiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<RegionWithNavigationPropertiesDto> SelectedRegions { get; set; } = new();
        private bool AllRegionsSelected { get; set; }
        
        public Regions()
        {
            NewRegion = new RegionCreateDto();
            EditingRegion = new RegionUpdateDto();
            Filter = new GetRegionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            RegionList = new List<RegionWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetCityCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Regions"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewRegion"], async () =>
            {
                await OpenCreateRegionModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Regions.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateRegion = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Regions.Create);
            CanEditRegion = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Regions.Edit);
            CanDeleteRegion = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Regions.Delete);
                            
                            
        }

        private async Task GetRegionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await RegionsAppService.GetListAsync(Filter);
            RegionList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetRegionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await RegionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/regions/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}&CityId={Filter.CityId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<RegionWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetRegionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateRegionModalAsync()
        {
            NewRegion = new RegionCreateDto{
                
                CityId = CitiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "region-create-tab";
            
            
            await NewRegionValidations.ClearAll();
            await CreateRegionModal.Show();
        }

        private async Task CloseCreateRegionModalAsync()
        {
            NewRegion = new RegionCreateDto{
                
                CityId = CitiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateRegionModal.Hide();
        }

        private async Task OpenEditRegionModalAsync(RegionWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "region-edit-tab";
            
            
            var region = await RegionsAppService.GetWithNavigationPropertiesAsync(input.Region.Id);
            
            EditingRegionId = region.Region.Id;
            EditingRegion = ObjectMapper.Map<RegionDto, RegionUpdateDto>(region.Region);
            
            await EditingRegionValidations.ClearAll();
            await EditRegionModal.Show();
        }

        private async Task DeleteRegionAsync(RegionWithNavigationPropertiesDto input)
        {
            await RegionsAppService.DeleteAsync(input.Region.Id);
            await GetRegionsAsync();
        }

        private async Task CreateRegionAsync()
        {
            try
            {
                if (await NewRegionValidations.ValidateAll() == false)
                {
                    return;
                }

                await RegionsAppService.CreateAsync(NewRegion);
                await GetRegionsAsync();
                await CloseCreateRegionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditRegionModalAsync()
        {
            await EditRegionModal.Hide();
        }

        private async Task UpdateRegionAsync()
        {
            try
            {
                if (await EditingRegionValidations.ValidateAll() == false)
                {
                    return;
                }

                await RegionsAppService.UpdateAsync(EditingRegionId, EditingRegion);
                await GetRegionsAsync();
                await EditRegionModal.Hide();                
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
        protected virtual async Task OnCityIdChangedAsync(Guid? cityId)
        {
            Filter.CityId = cityId;
            await SearchAsync();
        }
        

        private async Task GetCityCollectionLookupAsync(string? newValue = null)
        {
            CitiesCollection = (await RegionsAppService.GetCityLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllRegionsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllRegionsSelected = false;
            SelectedRegions.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedRegionRowsChanged()
        {
            if (SelectedRegions.Count != PageSize)
            {
                AllRegionsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedRegionsAsync()
        {
            var message = AllRegionsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedRegions.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllRegionsSelected)
            {
                await RegionsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await RegionsAppService.DeleteByIdsAsync(SelectedRegions.Select(x => x.Region.Id).ToList());
            }

            SelectedRegions.Clear();
            AllRegionsSelected = false;

            await GetRegionsAsync();
        }


    }
}
