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
using Imaar.SecondaryAmenities;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class SecondaryAmenities
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<SecondaryAmenityDto> SecondaryAmenityList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSecondaryAmenity { get; set; }
        private bool CanEditSecondaryAmenity { get; set; }
        private bool CanDeleteSecondaryAmenity { get; set; }
        private SecondaryAmenityCreateDto NewSecondaryAmenity { get; set; }
        private Validations NewSecondaryAmenityValidations { get; set; } = new();
        private SecondaryAmenityUpdateDto EditingSecondaryAmenity { get; set; }
        private Validations EditingSecondaryAmenityValidations { get; set; } = new();
        private Guid EditingSecondaryAmenityId { get; set; }
        private Modal CreateSecondaryAmenityModal { get; set; } = new();
        private Modal EditSecondaryAmenityModal { get; set; } = new();
        private GetSecondaryAmenitiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<SecondaryAmenityDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "secondaryAmenity-create-tab";
        protected string SelectedEditTab = "secondaryAmenity-edit-tab";
        private SecondaryAmenityDto? SelectedSecondaryAmenity;
        
        
        
        
        
        private List<SecondaryAmenityDto> SelectedSecondaryAmenities { get; set; } = new();
        private bool AllSecondaryAmenitiesSelected { get; set; }
        
        public SecondaryAmenities()
        {
            NewSecondaryAmenity = new SecondaryAmenityCreateDto();
            EditingSecondaryAmenity = new SecondaryAmenityUpdateDto();
            Filter = new GetSecondaryAmenitiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SecondaryAmenityList = new List<SecondaryAmenityDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SecondaryAmenities"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSecondaryAmenity"], async () =>
            {
                await OpenCreateSecondaryAmenityModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.SecondaryAmenities.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSecondaryAmenity = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.SecondaryAmenities.Create);
            CanEditSecondaryAmenity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.SecondaryAmenities.Edit);
            CanDeleteSecondaryAmenity = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.SecondaryAmenities.Delete);
                            
                            
        }

        private async Task GetSecondaryAmenitiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SecondaryAmenitiesAppService.GetListAsync(Filter);
            SecondaryAmenityList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSecondaryAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await SecondaryAmenitiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/secondary-amenities/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SecondaryAmenityDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSecondaryAmenitiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSecondaryAmenityModalAsync()
        {
            NewSecondaryAmenity = new SecondaryAmenityCreateDto{
                
                
            };

            SelectedCreateTab = "secondaryAmenity-create-tab";
            
            
            await NewSecondaryAmenityValidations.ClearAll();
            await CreateSecondaryAmenityModal.Show();
        }

        private async Task CloseCreateSecondaryAmenityModalAsync()
        {
            NewSecondaryAmenity = new SecondaryAmenityCreateDto{
                
                
            };
            await CreateSecondaryAmenityModal.Hide();
        }

        private async Task OpenEditSecondaryAmenityModalAsync(SecondaryAmenityDto input)
        {
            SelectedEditTab = "secondaryAmenity-edit-tab";
            
            
            var secondaryAmenity = await SecondaryAmenitiesAppService.GetAsync(input.Id);
            
            EditingSecondaryAmenityId = secondaryAmenity.Id;
            EditingSecondaryAmenity = ObjectMapper.Map<SecondaryAmenityDto, SecondaryAmenityUpdateDto>(secondaryAmenity);
            
            await EditingSecondaryAmenityValidations.ClearAll();
            await EditSecondaryAmenityModal.Show();
        }

        private async Task DeleteSecondaryAmenityAsync(SecondaryAmenityDto input)
        {
            await SecondaryAmenitiesAppService.DeleteAsync(input.Id);
            await GetSecondaryAmenitiesAsync();
        }

        private async Task CreateSecondaryAmenityAsync()
        {
            try
            {
                if (await NewSecondaryAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await SecondaryAmenitiesAppService.CreateAsync(NewSecondaryAmenity);
                await GetSecondaryAmenitiesAsync();
                await CloseCreateSecondaryAmenityModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSecondaryAmenityModalAsync()
        {
            await EditSecondaryAmenityModal.Hide();
        }

        private async Task UpdateSecondaryAmenityAsync()
        {
            try
            {
                if (await EditingSecondaryAmenityValidations.ValidateAll() == false)
                {
                    return;
                }

                await SecondaryAmenitiesAppService.UpdateAsync(EditingSecondaryAmenityId, EditingSecondaryAmenity);
                await GetSecondaryAmenitiesAsync();
                await EditSecondaryAmenityModal.Hide();                
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
            AllSecondaryAmenitiesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllSecondaryAmenitiesSelected = false;
            SelectedSecondaryAmenities.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedSecondaryAmenityRowsChanged()
        {
            if (SelectedSecondaryAmenities.Count != PageSize)
            {
                AllSecondaryAmenitiesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedSecondaryAmenitiesAsync()
        {
            var message = AllSecondaryAmenitiesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSecondaryAmenities.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllSecondaryAmenitiesSelected)
            {
                await SecondaryAmenitiesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await SecondaryAmenitiesAppService.DeleteByIdsAsync(SelectedSecondaryAmenities.Select(x => x.Id).ToList());
            }

            SelectedSecondaryAmenities.Clear();
            AllSecondaryAmenitiesSelected = false;

            await GetSecondaryAmenitiesAsync();
        }


    }
}
