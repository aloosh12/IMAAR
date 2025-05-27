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
using Imaar.FurnishingLevels;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class FurnishingLevels
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<FurnishingLevelDto> FurnishingLevelList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateFurnishingLevel { get; set; }
        private bool CanEditFurnishingLevel { get; set; }
        private bool CanDeleteFurnishingLevel { get; set; }
        private FurnishingLevelCreateDto NewFurnishingLevel { get; set; }
        private Validations NewFurnishingLevelValidations { get; set; } = new();
        private FurnishingLevelUpdateDto EditingFurnishingLevel { get; set; }
        private Validations EditingFurnishingLevelValidations { get; set; } = new();
        private Guid EditingFurnishingLevelId { get; set; }
        private Modal CreateFurnishingLevelModal { get; set; } = new();
        private Modal EditFurnishingLevelModal { get; set; } = new();
        private GetFurnishingLevelsInput Filter { get; set; }
        private DataGridEntityActionsColumn<FurnishingLevelDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "furnishingLevel-create-tab";
        protected string SelectedEditTab = "furnishingLevel-edit-tab";
        private FurnishingLevelDto? SelectedFurnishingLevel;
        
        
        
        
        
        private List<FurnishingLevelDto> SelectedFurnishingLevels { get; set; } = new();
        private bool AllFurnishingLevelsSelected { get; set; }
        
        public FurnishingLevels()
        {
            NewFurnishingLevel = new FurnishingLevelCreateDto();
            EditingFurnishingLevel = new FurnishingLevelUpdateDto();
            Filter = new GetFurnishingLevelsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            FurnishingLevelList = new List<FurnishingLevelDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["FurnishingLevels"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewFurnishingLevel"], async () =>
            {
                await OpenCreateFurnishingLevelModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.FurnishingLevels.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateFurnishingLevel = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.FurnishingLevels.Create);
            CanEditFurnishingLevel = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.FurnishingLevels.Edit);
            CanDeleteFurnishingLevel = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.FurnishingLevels.Delete);
                            
                            
        }

        private async Task GetFurnishingLevelsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await FurnishingLevelsAppService.GetListAsync(Filter);
            FurnishingLevelList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetFurnishingLevelsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await FurnishingLevelsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/furnishing-levels/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<FurnishingLevelDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetFurnishingLevelsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateFurnishingLevelModalAsync()
        {
            NewFurnishingLevel = new FurnishingLevelCreateDto{
                
                
            };

            SelectedCreateTab = "furnishingLevel-create-tab";
            
            
            await NewFurnishingLevelValidations.ClearAll();
            await CreateFurnishingLevelModal.Show();
        }

        private async Task CloseCreateFurnishingLevelModalAsync()
        {
            NewFurnishingLevel = new FurnishingLevelCreateDto{
                
                
            };
            await CreateFurnishingLevelModal.Hide();
        }

        private async Task OpenEditFurnishingLevelModalAsync(FurnishingLevelDto input)
        {
            SelectedEditTab = "furnishingLevel-edit-tab";
            
            
            var furnishingLevel = await FurnishingLevelsAppService.GetAsync(input.Id);
            
            EditingFurnishingLevelId = furnishingLevel.Id;
            EditingFurnishingLevel = ObjectMapper.Map<FurnishingLevelDto, FurnishingLevelUpdateDto>(furnishingLevel);
            
            await EditingFurnishingLevelValidations.ClearAll();
            await EditFurnishingLevelModal.Show();
        }

        private async Task DeleteFurnishingLevelAsync(FurnishingLevelDto input)
        {
            await FurnishingLevelsAppService.DeleteAsync(input.Id);
            await GetFurnishingLevelsAsync();
        }

        private async Task CreateFurnishingLevelAsync()
        {
            try
            {
                if (await NewFurnishingLevelValidations.ValidateAll() == false)
                {
                    return;
                }

                await FurnishingLevelsAppService.CreateAsync(NewFurnishingLevel);
                await GetFurnishingLevelsAsync();
                await CloseCreateFurnishingLevelModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditFurnishingLevelModalAsync()
        {
            await EditFurnishingLevelModal.Hide();
        }

        private async Task UpdateFurnishingLevelAsync()
        {
            try
            {
                if (await EditingFurnishingLevelValidations.ValidateAll() == false)
                {
                    return;
                }

                await FurnishingLevelsAppService.UpdateAsync(EditingFurnishingLevelId, EditingFurnishingLevel);
                await GetFurnishingLevelsAsync();
                await EditFurnishingLevelModal.Hide();                
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
            AllFurnishingLevelsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllFurnishingLevelsSelected = false;
            SelectedFurnishingLevels.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedFurnishingLevelRowsChanged()
        {
            if (SelectedFurnishingLevels.Count != PageSize)
            {
                AllFurnishingLevelsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedFurnishingLevelsAsync()
        {
            var message = AllFurnishingLevelsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedFurnishingLevels.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllFurnishingLevelsSelected)
            {
                await FurnishingLevelsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await FurnishingLevelsAppService.DeleteByIdsAsync(SelectedFurnishingLevels.Select(x => x.Id).ToList());
            }

            SelectedFurnishingLevels.Clear();
            AllFurnishingLevelsSelected = false;

            await GetFurnishingLevelsAsync();
        }


    }
}
