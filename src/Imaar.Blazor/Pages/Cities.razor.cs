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
using Imaar.Cities;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Cities
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<CityDto> CityList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCity { get; set; }
        private bool CanEditCity { get; set; }
        private bool CanDeleteCity { get; set; }
        private CityCreateDto NewCity { get; set; }
        private Validations NewCityValidations { get; set; } = new();
        private CityUpdateDto EditingCity { get; set; }
        private Validations EditingCityValidations { get; set; } = new();
        private Guid EditingCityId { get; set; }
        private Modal CreateCityModal { get; set; } = new();
        private Modal EditCityModal { get; set; } = new();
        private GetCitiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<CityDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "city-create-tab";
        protected string SelectedEditTab = "city-edit-tab";
        private CityDto? SelectedCity;
        
        
        
        
        
        private List<CityDto> SelectedCities { get; set; } = new();
        private bool AllCitiesSelected { get; set; }
        
        public Cities()
        {
            NewCity = new CityCreateDto();
            EditingCity = new CityUpdateDto();
            Filter = new GetCitiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CityList = new List<CityDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Cities"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCity"], async () =>
            {
                await OpenCreateCityModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Cities.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCity = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Cities.Create);
            CanEditCity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Cities.Edit);
            CanDeleteCity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Cities.Delete);
                            
                            
        }

        private async Task GetCitiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CitiesAppService.GetListAsync(Filter);
            CityList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await CitiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/cities/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CityDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCityModalAsync()
        {
            NewCity = new CityCreateDto{
                
                
            };

            SelectedCreateTab = "city-create-tab";
            
            
            await NewCityValidations.ClearAll();
            await CreateCityModal.Show();
        }

        private async Task CloseCreateCityModalAsync()
        {
            NewCity = new CityCreateDto{
                
                
            };
            await CreateCityModal.Hide();
        }

        private async Task OpenEditCityModalAsync(CityDto input)
        {
            SelectedEditTab = "city-edit-tab";
            
            
            var city = await CitiesAppService.GetAsync(input.Id);
            
            EditingCityId = city.Id;
            EditingCity = ObjectMapper.Map<CityDto, CityUpdateDto>(city);
            
            await EditingCityValidations.ClearAll();
            await EditCityModal.Show();
        }

        private async Task DeleteCityAsync(CityDto input)
        {
            await CitiesAppService.DeleteAsync(input.Id);
            await GetCitiesAsync();
        }

        private async Task CreateCityAsync()
        {
            try
            {
                if (await NewCityValidations.ValidateAll() == false)
                {
                    return;
                }

                await CitiesAppService.CreateAsync(NewCity);
                await GetCitiesAsync();
                await CloseCreateCityModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCityModalAsync()
        {
            await EditCityModal.Hide();
        }

        private async Task UpdateCityAsync()
        {
            try
            {
                if (await EditingCityValidations.ValidateAll() == false)
                {
                    return;
                }

                await CitiesAppService.UpdateAsync(EditingCityId, EditingCity);
                await GetCitiesAsync();
                await EditCityModal.Hide();                
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
            AllCitiesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllCitiesSelected = false;
            SelectedCities.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedCityRowsChanged()
        {
            if (SelectedCities.Count != PageSize)
            {
                AllCitiesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedCitiesAsync()
        {
            var message = AllCitiesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedCities.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllCitiesSelected)
            {
                await CitiesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await CitiesAppService.DeleteByIdsAsync(SelectedCities.Select(x => x.Id).ToList());
            }

            SelectedCities.Clear();
            AllCitiesSelected = false;

            await GetCitiesAsync();
        }


    }
}
