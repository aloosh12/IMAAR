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
using Imaar.BuildingFacades;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class BuildingFacades
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<BuildingFacadeDto> BuildingFacadeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBuildingFacade { get; set; }
        private bool CanEditBuildingFacade { get; set; }
        private bool CanDeleteBuildingFacade { get; set; }
        private BuildingFacadeCreateDto NewBuildingFacade { get; set; }
        private Validations NewBuildingFacadeValidations { get; set; } = new();
        private BuildingFacadeUpdateDto EditingBuildingFacade { get; set; }
        private Validations EditingBuildingFacadeValidations { get; set; } = new();
        private Guid EditingBuildingFacadeId { get; set; }
        private Modal CreateBuildingFacadeModal { get; set; } = new();
        private Modal EditBuildingFacadeModal { get; set; } = new();
        private GetBuildingFacadesInput Filter { get; set; }
        private DataGridEntityActionsColumn<BuildingFacadeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "buildingFacade-create-tab";
        protected string SelectedEditTab = "buildingFacade-edit-tab";
        private BuildingFacadeDto? SelectedBuildingFacade;
        
        
        
        
        
        private List<BuildingFacadeDto> SelectedBuildingFacades { get; set; } = new();
        private bool AllBuildingFacadesSelected { get; set; }
        
        public BuildingFacades()
        {
            NewBuildingFacade = new BuildingFacadeCreateDto();
            EditingBuildingFacade = new BuildingFacadeUpdateDto();
            Filter = new GetBuildingFacadesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BuildingFacadeList = new List<BuildingFacadeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["BuildingFacades"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewBuildingFacade"], async () =>
            {
                await OpenCreateBuildingFacadeModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.BuildingFacades.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateBuildingFacade = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.BuildingFacades.Create);
            CanEditBuildingFacade = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.BuildingFacades.Edit);
            CanDeleteBuildingFacade = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.BuildingFacades.Delete);
                            
                            
        }

        private async Task GetBuildingFacadesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BuildingFacadesAppService.GetListAsync(Filter);
            BuildingFacadeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetBuildingFacadesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BuildingFacadesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/building-facades/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BuildingFacadeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetBuildingFacadesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateBuildingFacadeModalAsync()
        {
            NewBuildingFacade = new BuildingFacadeCreateDto{
                
                
            };

            SelectedCreateTab = "buildingFacade-create-tab";
            
            
            await NewBuildingFacadeValidations.ClearAll();
            await CreateBuildingFacadeModal.Show();
        }

        private async Task CloseCreateBuildingFacadeModalAsync()
        {
            NewBuildingFacade = new BuildingFacadeCreateDto{
                
                
            };
            await CreateBuildingFacadeModal.Hide();
        }

        private async Task OpenEditBuildingFacadeModalAsync(BuildingFacadeDto input)
        {
            SelectedEditTab = "buildingFacade-edit-tab";
            
            
            var buildingFacade = await BuildingFacadesAppService.GetAsync(input.Id);
            
            EditingBuildingFacadeId = buildingFacade.Id;
            EditingBuildingFacade = ObjectMapper.Map<BuildingFacadeDto, BuildingFacadeUpdateDto>(buildingFacade);
            
            await EditingBuildingFacadeValidations.ClearAll();
            await EditBuildingFacadeModal.Show();
        }

        private async Task DeleteBuildingFacadeAsync(BuildingFacadeDto input)
        {
            await BuildingFacadesAppService.DeleteAsync(input.Id);
            await GetBuildingFacadesAsync();
        }

        private async Task CreateBuildingFacadeAsync()
        {
            try
            {
                if (await NewBuildingFacadeValidations.ValidateAll() == false)
                {
                    return;
                }

                await BuildingFacadesAppService.CreateAsync(NewBuildingFacade);
                await GetBuildingFacadesAsync();
                await CloseCreateBuildingFacadeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditBuildingFacadeModalAsync()
        {
            await EditBuildingFacadeModal.Hide();
        }

        private async Task UpdateBuildingFacadeAsync()
        {
            try
            {
                if (await EditingBuildingFacadeValidations.ValidateAll() == false)
                {
                    return;
                }

                await BuildingFacadesAppService.UpdateAsync(EditingBuildingFacadeId, EditingBuildingFacade);
                await GetBuildingFacadesAsync();
                await EditBuildingFacadeModal.Hide();                
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
            AllBuildingFacadesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllBuildingFacadesSelected = false;
            SelectedBuildingFacades.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedBuildingFacadeRowsChanged()
        {
            if (SelectedBuildingFacades.Count != PageSize)
            {
                AllBuildingFacadesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedBuildingFacadesAsync()
        {
            var message = AllBuildingFacadesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedBuildingFacades.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllBuildingFacadesSelected)
            {
                await BuildingFacadesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await BuildingFacadesAppService.DeleteByIdsAsync(SelectedBuildingFacades.Select(x => x.Id).ToList());
            }

            SelectedBuildingFacades.Clear();
            AllBuildingFacadesSelected = false;

            await GetBuildingFacadesAsync();
        }


    }
}
