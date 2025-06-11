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
using Imaar.BuildingEvaluations;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class BuildingEvaluations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<BuildingEvaluationWithNavigationPropertiesDto> BuildingEvaluationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBuildingEvaluation { get; set; }
        private bool CanEditBuildingEvaluation { get; set; }
        private bool CanDeleteBuildingEvaluation { get; set; }
        private BuildingEvaluationCreateDto NewBuildingEvaluation { get; set; }
        private Validations NewBuildingEvaluationValidations { get; set; } = new();
        private BuildingEvaluationUpdateDto EditingBuildingEvaluation { get; set; }
        private Validations EditingBuildingEvaluationValidations { get; set; } = new();
        private Guid EditingBuildingEvaluationId { get; set; }
        private Modal CreateBuildingEvaluationModal { get; set; } = new();
        private Modal EditBuildingEvaluationModal { get; set; } = new();
        private GetBuildingEvaluationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<BuildingEvaluationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "buildingEvaluation-create-tab";
        protected string SelectedEditTab = "buildingEvaluation-edit-tab";
        private BuildingEvaluationWithNavigationPropertiesDto? SelectedBuildingEvaluation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> BuildingsCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<BuildingEvaluationWithNavigationPropertiesDto> SelectedBuildingEvaluations { get; set; } = new();
        private bool AllBuildingEvaluationsSelected { get; set; }
        
        public BuildingEvaluations()
        {
            NewBuildingEvaluation = new BuildingEvaluationCreateDto();
            EditingBuildingEvaluation = new BuildingEvaluationUpdateDto();
            Filter = new GetBuildingEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BuildingEvaluationList = new List<BuildingEvaluationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            await GetBuildingCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["BuildingEvaluations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewBuildingEvaluation"], async () =>
            {
                await OpenCreateBuildingEvaluationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.BuildingEvaluations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateBuildingEvaluation = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.BuildingEvaluations.Create);
            CanEditBuildingEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.BuildingEvaluations.Edit);
            CanDeleteBuildingEvaluation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.BuildingEvaluations.Delete);
                            
                            
        }

        private async Task GetBuildingEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BuildingEvaluationsAppService.GetListAsync(Filter);
            BuildingEvaluationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetBuildingEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BuildingEvaluationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/building-evaluations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&RateMin={Filter.RateMin}&RateMax={Filter.RateMax}&EvaluatorId={Filter.EvaluatorId}&BuildingId={Filter.BuildingId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BuildingEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetBuildingEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateBuildingEvaluationModalAsync()
        {
            NewBuildingEvaluation = new BuildingEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
BuildingId = BuildingsCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "buildingEvaluation-create-tab";
            
            
            await NewBuildingEvaluationValidations.ClearAll();
            await CreateBuildingEvaluationModal.Show();
        }

        private async Task CloseCreateBuildingEvaluationModalAsync()
        {
            NewBuildingEvaluation = new BuildingEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
BuildingId = BuildingsCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateBuildingEvaluationModal.Hide();
        }

        private async Task OpenEditBuildingEvaluationModalAsync(BuildingEvaluationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "buildingEvaluation-edit-tab";
            
            
            var buildingEvaluation = await BuildingEvaluationsAppService.GetWithNavigationPropertiesAsync(input.BuildingEvaluation.Id);
            
            EditingBuildingEvaluationId = buildingEvaluation.BuildingEvaluation.Id;
            EditingBuildingEvaluation = ObjectMapper.Map<BuildingEvaluationDto, BuildingEvaluationUpdateDto>(buildingEvaluation.BuildingEvaluation);
            
            await EditingBuildingEvaluationValidations.ClearAll();
            await EditBuildingEvaluationModal.Show();
        }

        private async Task DeleteBuildingEvaluationAsync(BuildingEvaluationWithNavigationPropertiesDto input)
        {
            await BuildingEvaluationsAppService.DeleteAsync(input.BuildingEvaluation.Id);
            await GetBuildingEvaluationsAsync();
        }

        private async Task CreateBuildingEvaluationAsync()
        {
            try
            {
                if (await NewBuildingEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await BuildingEvaluationsAppService.CreateAsync(NewBuildingEvaluation);
                await GetBuildingEvaluationsAsync();
                await CloseCreateBuildingEvaluationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditBuildingEvaluationModalAsync()
        {
            await EditBuildingEvaluationModal.Hide();
        }

        private async Task UpdateBuildingEvaluationAsync()
        {
            try
            {
                if (await EditingBuildingEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await BuildingEvaluationsAppService.UpdateAsync(EditingBuildingEvaluationId, EditingBuildingEvaluation);
                await GetBuildingEvaluationsAsync();
                await EditBuildingEvaluationModal.Hide();                
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
        protected virtual async Task OnBuildingIdChangedAsync(Guid? buildingId)
        {
            Filter.BuildingId = buildingId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await BuildingEvaluationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetBuildingCollectionLookupAsync(string? newValue = null)
        {
            BuildingsCollection = (await BuildingEvaluationsAppService.GetBuildingLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllBuildingEvaluationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllBuildingEvaluationsSelected = false;
            SelectedBuildingEvaluations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedBuildingEvaluationRowsChanged()
        {
            if (SelectedBuildingEvaluations.Count != PageSize)
            {
                AllBuildingEvaluationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedBuildingEvaluationsAsync()
        {
            var message = AllBuildingEvaluationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedBuildingEvaluations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllBuildingEvaluationsSelected)
            {
                await BuildingEvaluationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await BuildingEvaluationsAppService.DeleteByIdsAsync(SelectedBuildingEvaluations.Select(x => x.BuildingEvaluation.Id).ToList());
            }

            SelectedBuildingEvaluations.Clear();
            AllBuildingEvaluationsSelected = false;

            await GetBuildingEvaluationsAsync();
        }


    }
}
