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
using Imaar.MainAmenities;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class MainAmenities
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<MainAmenityDto> MainAmenityList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMainAmenity { get; set; }
        private bool CanEditMainAmenity { get; set; }
        private bool CanDeleteMainAmenity { get; set; }
        private MainAmenityCreateDto NewMainAmenity { get; set; }
        private Validations NewMainAmenityValidations { get; set; } = new();
        private MainAmenityUpdateDto EditingMainAmenity { get; set; }
        private Validations EditingMainAmenityValidations { get; set; } = new();
        private Guid EditingMainAmenityId { get; set; }
        private Modal CreateMainAmenityModal { get; set; } = new();
        private Modal EditMainAmenityModal { get; set; } = new();
        private GetMainAmenitiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<MainAmenityDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "mainAmenity-create-tab";
        protected string SelectedEditTab = "mainAmenity-edit-tab";
        private MainAmenityDto? SelectedMainAmenity;
        
        
        
        
        
        private List<MainAmenityDto> SelectedMainAmenities { get; set; } = new();
        private bool AllMainAmenitiesSelected { get; set; }
        
        public MainAmenities()
        {
            NewMainAmenity = new MainAmenityCreateDto();
            EditingMainAmenity = new MainAmenityUpdateDto();
            Filter = new GetMainAmenitiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MainAmenityList = new List<MainAmenityDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["MainAmenities"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMainAmenity"], async () =>
            {
                await OpenCreateMainAmenityModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.MainAmenities.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateMainAmenity = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.MainAmenities.Create);
            CanEditMainAmenity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.MainAmenities.Edit);
            CanDeleteMainAmenity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.MainAmenities.Delete);
                            
                            
        }

        private async Task GetMainAmenitiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MainAmenitiesAppService.GetListAsync(Filter);
            MainAmenityList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMainAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await MainAmenitiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/main-amenities/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MainAmenityDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMainAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMainAmenityModalAsync()
        {
            NewMainAmenity = new MainAmenityCreateDto{
                
                
            };

            SelectedCreateTab = "mainAmenity-create-tab";
            
            
            await NewMainAmenityValidations.ClearAll();
            await CreateMainAmenityModal.Show();
        }

        private async Task CloseCreateMainAmenityModalAsync()
        {
            NewMainAmenity = new MainAmenityCreateDto{
                
                
            };
            await CreateMainAmenityModal.Hide();
        }

        private async Task OpenEditMainAmenityModalAsync(MainAmenityDto input)
        {
            SelectedEditTab = "mainAmenity-edit-tab";
            
            
            var mainAmenity = await MainAmenitiesAppService.GetAsync(input.Id);
            
            EditingMainAmenityId = mainAmenity.Id;
            EditingMainAmenity = ObjectMapper.Map<MainAmenityDto, MainAmenityUpdateDto>(mainAmenity);
            
            await EditingMainAmenityValidations.ClearAll();
            await EditMainAmenityModal.Show();
        }

        private async Task DeleteMainAmenityAsync(MainAmenityDto input)
        {
            await MainAmenitiesAppService.DeleteAsync(input.Id);
            await GetMainAmenitiesAsync();
        }

        private async Task CreateMainAmenityAsync()
        {
            try
            {
                if (await NewMainAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await MainAmenitiesAppService.CreateAsync(NewMainAmenity);
                await GetMainAmenitiesAsync();
                await CloseCreateMainAmenityModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMainAmenityModalAsync()
        {
            await EditMainAmenityModal.Hide();
        }

        private async Task UpdateMainAmenityAsync()
        {
            try
            {
                if (await EditingMainAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await MainAmenitiesAppService.UpdateAsync(EditingMainAmenityId, EditingMainAmenity);
                await GetMainAmenitiesAsync();
                await EditMainAmenityModal.Hide();                
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
            AllMainAmenitiesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllMainAmenitiesSelected = false;
            SelectedMainAmenities.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedMainAmenityRowsChanged()
        {
            if (SelectedMainAmenities.Count != PageSize)
            {
                AllMainAmenitiesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedMainAmenitiesAsync()
        {
            var message = AllMainAmenitiesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedMainAmenities.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllMainAmenitiesSelected)
            {
                await MainAmenitiesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await MainAmenitiesAppService.DeleteByIdsAsync(SelectedMainAmenities.Select(x => x.Id).ToList());
            }

            SelectedMainAmenities.Clear();
            AllMainAmenitiesSelected = false;

            await GetMainAmenitiesAsync();
        }


    }
}
