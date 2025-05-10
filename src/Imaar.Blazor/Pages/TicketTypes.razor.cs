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
using Imaar.TicketTypes;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class TicketTypes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<TicketTypeDto> TicketTypeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateTicketType { get; set; }
        private bool CanEditTicketType { get; set; }
        private bool CanDeleteTicketType { get; set; }
        private TicketTypeCreateDto NewTicketType { get; set; }
        private Validations NewTicketTypeValidations { get; set; } = new();
        private TicketTypeUpdateDto EditingTicketType { get; set; }
        private Validations EditingTicketTypeValidations { get; set; } = new();
        private Guid EditingTicketTypeId { get; set; }
        private Modal CreateTicketTypeModal { get; set; } = new();
        private Modal EditTicketTypeModal { get; set; } = new();
        private GetTicketTypesInput Filter { get; set; }
        private DataGridEntityActionsColumn<TicketTypeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "ticketType-create-tab";
        protected string SelectedEditTab = "ticketType-edit-tab";
        private TicketTypeDto? SelectedTicketType;
        
        
        
        
        
        private List<TicketTypeDto> SelectedTicketTypes { get; set; } = new();
        private bool AllTicketTypesSelected { get; set; }
        
        public TicketTypes()
        {
            NewTicketType = new TicketTypeCreateDto();
            EditingTicketType = new TicketTypeUpdateDto();
            Filter = new GetTicketTypesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            TicketTypeList = new List<TicketTypeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["TicketTypes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewTicketType"], async () =>
            {
                await OpenCreateTicketTypeModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.TicketTypes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTicketType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.TicketTypes.Create);
            CanEditTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.TicketTypes.Edit);
            CanDeleteTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.TicketTypes.Delete);
                            
                            
        }

        private async Task GetTicketTypesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await TicketTypesAppService.GetListAsync(Filter);
            TicketTypeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await TicketTypesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/ticket-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TicketTypeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateTicketTypeModalAsync()
        {
            NewTicketType = new TicketTypeCreateDto{
                
                
            };

            SelectedCreateTab = "ticketType-create-tab";
            
            
            await NewTicketTypeValidations.ClearAll();
            await CreateTicketTypeModal.Show();
        }

        private async Task CloseCreateTicketTypeModalAsync()
        {
            NewTicketType = new TicketTypeCreateDto{
                
                
            };
            await CreateTicketTypeModal.Hide();
        }

        private async Task OpenEditTicketTypeModalAsync(TicketTypeDto input)
        {
            SelectedEditTab = "ticketType-edit-tab";
            
            
            var ticketType = await TicketTypesAppService.GetAsync(input.Id);
            
            EditingTicketTypeId = ticketType.Id;
            EditingTicketType = ObjectMapper.Map<TicketTypeDto, TicketTypeUpdateDto>(ticketType);
            
            await EditingTicketTypeValidations.ClearAll();
            await EditTicketTypeModal.Show();
        }

        private async Task DeleteTicketTypeAsync(TicketTypeDto input)
        {
            await TicketTypesAppService.DeleteAsync(input.Id);
            await GetTicketTypesAsync();
        }

        private async Task CreateTicketTypeAsync()
        {
            try
            {
                if (await NewTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await TicketTypesAppService.CreateAsync(NewTicketType);
                await GetTicketTypesAsync();
                await CloseCreateTicketTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditTicketTypeModalAsync()
        {
            await EditTicketTypeModal.Hide();
        }

        private async Task UpdateTicketTypeAsync()
        {
            try
            {
                if (await EditingTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await TicketTypesAppService.UpdateAsync(EditingTicketTypeId, EditingTicketType);
                await GetTicketTypesAsync();
                await EditTicketTypeModal.Hide();                
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
            AllTicketTypesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllTicketTypesSelected = false;
            SelectedTicketTypes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedTicketTypeRowsChanged()
        {
            if (SelectedTicketTypes.Count != PageSize)
            {
                AllTicketTypesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedTicketTypesAsync()
        {
            var message = AllTicketTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedTicketTypes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllTicketTypesSelected)
            {
                await TicketTypesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await TicketTypesAppService.DeleteByIdsAsync(SelectedTicketTypes.Select(x => x.Id).ToList());
            }

            SelectedTicketTypes.Clear();
            AllTicketTypesSelected = false;

            await GetTicketTypesAsync();
        }


    }
}
