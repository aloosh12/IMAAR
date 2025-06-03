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
using Imaar.StoryTicketTypes;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class StoryTicketTypes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<StoryTicketTypeDto> StoryTicketTypeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateStoryTicketType { get; set; }
        private bool CanEditStoryTicketType { get; set; }
        private bool CanDeleteStoryTicketType { get; set; }
        private StoryTicketTypeCreateDto NewStoryTicketType { get; set; }
        private Validations NewStoryTicketTypeValidations { get; set; } = new();
        private StoryTicketTypeUpdateDto EditingStoryTicketType { get; set; }
        private Validations EditingStoryTicketTypeValidations { get; set; } = new();
        private Guid EditingStoryTicketTypeId { get; set; }
        private Modal CreateStoryTicketTypeModal { get; set; } = new();
        private Modal EditStoryTicketTypeModal { get; set; } = new();
        private GetStoryTicketTypesInput Filter { get; set; }
        private DataGridEntityActionsColumn<StoryTicketTypeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "storyTicketType-create-tab";
        protected string SelectedEditTab = "storyTicketType-edit-tab";
        private StoryTicketTypeDto? SelectedStoryTicketType;
        
        
        
        
        
        private List<StoryTicketTypeDto> SelectedStoryTicketTypes { get; set; } = new();
        private bool AllStoryTicketTypesSelected { get; set; }
        
        public StoryTicketTypes()
        {
            NewStoryTicketType = new StoryTicketTypeCreateDto();
            EditingStoryTicketType = new StoryTicketTypeUpdateDto();
            Filter = new GetStoryTicketTypesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            StoryTicketTypeList = new List<StoryTicketTypeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["StoryTicketTypes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewStoryTicketType"], async () =>
            {
                await OpenCreateStoryTicketTypeModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.StoryTicketTypes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateStoryTicketType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.StoryTicketTypes.Create);
            CanEditStoryTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryTicketTypes.Edit);
            CanDeleteStoryTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryTicketTypes.Delete);
                            
                            
        }

        private async Task GetStoryTicketTypesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await StoryTicketTypesAppService.GetListAsync(Filter);
            StoryTicketTypeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetStoryTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await StoryTicketTypesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/story-ticket-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StoryTicketTypeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetStoryTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateStoryTicketTypeModalAsync()
        {
            NewStoryTicketType = new StoryTicketTypeCreateDto{
                
                
            };

            SelectedCreateTab = "storyTicketType-create-tab";
            
            
            await NewStoryTicketTypeValidations.ClearAll();
            await CreateStoryTicketTypeModal.Show();
        }

        private async Task CloseCreateStoryTicketTypeModalAsync()
        {
            NewStoryTicketType = new StoryTicketTypeCreateDto{
                
                
            };
            await CreateStoryTicketTypeModal.Hide();
        }

        private async Task OpenEditStoryTicketTypeModalAsync(StoryTicketTypeDto input)
        {
            SelectedEditTab = "storyTicketType-edit-tab";
            
            
            var storyTicketType = await StoryTicketTypesAppService.GetAsync(input.Id);
            
            EditingStoryTicketTypeId = storyTicketType.Id;
            EditingStoryTicketType = ObjectMapper.Map<StoryTicketTypeDto, StoryTicketTypeUpdateDto>(storyTicketType);
            
            await EditingStoryTicketTypeValidations.ClearAll();
            await EditStoryTicketTypeModal.Show();
        }

        private async Task DeleteStoryTicketTypeAsync(StoryTicketTypeDto input)
        {
            await StoryTicketTypesAppService.DeleteAsync(input.Id);
            await GetStoryTicketTypesAsync();
        }

        private async Task CreateStoryTicketTypeAsync()
        {
            try
            {
                if (await NewStoryTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryTicketTypesAppService.CreateAsync(NewStoryTicketType);
                await GetStoryTicketTypesAsync();
                await CloseCreateStoryTicketTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditStoryTicketTypeModalAsync()
        {
            await EditStoryTicketTypeModal.Hide();
        }

        private async Task UpdateStoryTicketTypeAsync()
        {
            try
            {
                if (await EditingStoryTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryTicketTypesAppService.UpdateAsync(EditingStoryTicketTypeId, EditingStoryTicketType);
                await GetStoryTicketTypesAsync();
                await EditStoryTicketTypeModal.Hide();                
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









        protected virtual async Task OnTitleChangedAsync(string? title)
        {
            Filter.Title = title;
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
            AllStoryTicketTypesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllStoryTicketTypesSelected = false;
            SelectedStoryTicketTypes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedStoryTicketTypeRowsChanged()
        {
            if (SelectedStoryTicketTypes.Count != PageSize)
            {
                AllStoryTicketTypesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedStoryTicketTypesAsync()
        {
            var message = AllStoryTicketTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedStoryTicketTypes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllStoryTicketTypesSelected)
            {
                await StoryTicketTypesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await StoryTicketTypesAppService.DeleteByIdsAsync(SelectedStoryTicketTypes.Select(x => x.Id).ToList());
            }

            SelectedStoryTicketTypes.Clear();
            AllStoryTicketTypesSelected = false;

            await GetStoryTicketTypesAsync();
        }


    }
}
