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
using Imaar.Evalauations;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Evalauations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<EvalauationWithNavigationPropertiesDto> EvalauationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateEvalauation { get; set; }
        private bool CanEditEvalauation { get; set; }
        private bool CanDeleteEvalauation { get; set; }
        private EvalauationCreateDto NewEvalauation { get; set; }
        private Validations NewEvalauationValidations { get; set; } = new();
        private EvalauationUpdateDto EditingEvalauation { get; set; }
        private Validations EditingEvalauationValidations { get; set; } = new();
        private Guid EditingEvalauationId { get; set; }
        private Modal CreateEvalauationModal { get; set; } = new();
        private Modal EditEvalauationModal { get; set; } = new();
        private GetEvalauationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<EvalauationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "evalauation-create-tab";
        protected string SelectedEditTab = "evalauation-edit-tab";
        private EvalauationWithNavigationPropertiesDto? SelectedEvalauation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<EvalauationWithNavigationPropertiesDto> SelectedEvalauations { get; set; } = new();
        private bool AllEvalauationsSelected { get; set; }
        
        public Evalauations()
        {
            NewEvalauation = new EvalauationCreateDto();
            EditingEvalauation = new EvalauationUpdateDto();
            Filter = new GetEvalauationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            EvalauationList = new List<EvalauationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Evalauations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewEvalauation"], async () =>
            {
                await OpenCreateEvalauationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Evalauations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateEvalauation = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Evalauations.Create);
            CanEditEvalauation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Evalauations.Edit);
            CanDeleteEvalauation = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Evalauations.Delete);
                            
                            
        }

        private async Task GetEvalauationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EvalauationsAppService.GetListAsync(Filter);
            EvalauationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetEvalauationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await EvalauationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/evalauations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&SpeedOfCompletionMin={Filter.SpeedOfCompletionMin}&SpeedOfCompletionMax={Filter.SpeedOfCompletionMax}&DealingMin={Filter.DealingMin}&DealingMax={Filter.DealingMax}&CleanlinessMin={Filter.CleanlinessMin}&CleanlinessMax={Filter.CleanlinessMax}&PerfectionMin={Filter.PerfectionMin}&PerfectionMax={Filter.PerfectionMax}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&Evaluatord={Filter.Evaluatord}&EvaluatedPersonId={Filter.EvaluatedPersonId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EvalauationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetEvalauationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateEvalauationModalAsync()
        {
            NewEvalauation = new EvalauationCreateDto{
                
                Evaluatord = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "evalauation-create-tab";
            
            
            await NewEvalauationValidations.ClearAll();
            await CreateEvalauationModal.Show();
        }

        private async Task CloseCreateEvalauationModalAsync()
        {
            NewEvalauation = new EvalauationCreateDto{
                
                Evaluatord = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateEvalauationModal.Hide();
        }

        private async Task OpenEditEvalauationModalAsync(EvalauationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "evalauation-edit-tab";
            
            
            var evalauation = await EvalauationsAppService.GetWithNavigationPropertiesAsync(input.Evalauation.Id);
            
            EditingEvalauationId = evalauation.Evalauation.Id;
            EditingEvalauation = ObjectMapper.Map<EvalauationDto, EvalauationUpdateDto>(evalauation.Evalauation);
            
            await EditingEvalauationValidations.ClearAll();
            await EditEvalauationModal.Show();
        }

        private async Task DeleteEvalauationAsync(EvalauationWithNavigationPropertiesDto input)
        {
            await EvalauationsAppService.DeleteAsync(input.Evalauation.Id);
            await GetEvalauationsAsync();
        }

        private async Task CreateEvalauationAsync()
        {
            try
            {
                if (await NewEvalauationValidations.ValidateAll() == false)
                {
                    return;
                }

                await EvalauationsAppService.CreateAsync(NewEvalauation);
                await GetEvalauationsAsync();
                await CloseCreateEvalauationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditEvalauationModalAsync()
        {
            await EditEvalauationModal.Hide();
        }

        private async Task UpdateEvalauationAsync()
        {
            try
            {
                if (await EditingEvalauationValidations.ValidateAll() == false)
                {
                    return;
                }

                await EvalauationsAppService.UpdateAsync(EditingEvalauationId, EditingEvalauation);
                await GetEvalauationsAsync();
                await EditEvalauationModal.Hide();                
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









        protected virtual async Task OnSpeedOfCompletionMinChangedAsync(int? speedOfCompletionMin)
        {
            Filter.SpeedOfCompletionMin = speedOfCompletionMin;
            await SearchAsync();
        }
        protected virtual async Task OnSpeedOfCompletionMaxChangedAsync(int? speedOfCompletionMax)
        {
            Filter.SpeedOfCompletionMax = speedOfCompletionMax;
            await SearchAsync();
        }
        protected virtual async Task OnDealingMinChangedAsync(int? dealingMin)
        {
            Filter.DealingMin = dealingMin;
            await SearchAsync();
        }
        protected virtual async Task OnDealingMaxChangedAsync(int? dealingMax)
        {
            Filter.DealingMax = dealingMax;
            await SearchAsync();
        }
        protected virtual async Task OnCleanlinessMinChangedAsync(int? cleanlinessMin)
        {
            Filter.CleanlinessMin = cleanlinessMin;
            await SearchAsync();
        }
        protected virtual async Task OnCleanlinessMaxChangedAsync(int? cleanlinessMax)
        {
            Filter.CleanlinessMax = cleanlinessMax;
            await SearchAsync();
        }
        protected virtual async Task OnPerfectionMinChangedAsync(int? perfectionMin)
        {
            Filter.PerfectionMin = perfectionMin;
            await SearchAsync();
        }
        protected virtual async Task OnPerfectionMaxChangedAsync(int? perfectionMax)
        {
            Filter.PerfectionMax = perfectionMax;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMinChangedAsync(int? priceMin)
        {
            Filter.PriceMin = priceMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMaxChangedAsync(int? priceMax)
        {
            Filter.PriceMax = priceMax;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatordChangedAsync(Guid? evaluatord)
        {
            Filter.Evaluatord = evaluatord;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatedPersonIdChangedAsync(Guid? evaluatedPersonId)
        {
            Filter.EvaluatedPersonId = evaluatedPersonId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await EvalauationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllEvalauationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllEvalauationsSelected = false;
            SelectedEvalauations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedEvalauationRowsChanged()
        {
            if (SelectedEvalauations.Count != PageSize)
            {
                AllEvalauationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedEvalauationsAsync()
        {
            var message = AllEvalauationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedEvalauations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllEvalauationsSelected)
            {
                await EvalauationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await EvalauationsAppService.DeleteByIdsAsync(SelectedEvalauations.Select(x => x.Evalauation.Id).ToList());
            }

            SelectedEvalauations.Clear();
            AllEvalauationsSelected = false;

            await GetEvalauationsAsync();
        }


    }
}
