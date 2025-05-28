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
using Imaar.ServiceTicketTypes;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class ServiceTicketTypes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ServiceTicketTypeDto> ServiceTicketTypeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateServiceTicketType { get; set; }
        private bool CanEditServiceTicketType { get; set; }
        private bool CanDeleteServiceTicketType { get; set; }
        private ServiceTicketTypeCreateDto NewServiceTicketType { get; set; }
        private Validations NewServiceTicketTypeValidations { get; set; } = new();
        private ServiceTicketTypeUpdateDto EditingServiceTicketType { get; set; }
        private Validations EditingServiceTicketTypeValidations { get; set; } = new();
        private Guid EditingServiceTicketTypeId { get; set; }
        private Modal CreateServiceTicketTypeModal { get; set; } = new();
        private Modal EditServiceTicketTypeModal { get; set; } = new();
        private GetServiceTicketTypesInput Filter { get; set; }
        private DataGridEntityActionsColumn<ServiceTicketTypeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "serviceTicketType-create-tab";
        protected string SelectedEditTab = "serviceTicketType-edit-tab";
        private ServiceTicketTypeDto? SelectedServiceTicketType;
        
        
        
        
        
        private List<ServiceTicketTypeDto> SelectedServiceTicketTypes { get; set; } = new();
        private bool AllServiceTicketTypesSelected { get; set; }
        
        public ServiceTicketTypes()
        {
            NewServiceTicketType = new ServiceTicketTypeCreateDto();
            EditingServiceTicketType = new ServiceTicketTypeUpdateDto();
            Filter = new GetServiceTicketTypesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ServiceTicketTypeList = new List<ServiceTicketTypeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ServiceTicketTypes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewServiceTicketType"], async () =>
            {
                await OpenCreateServiceTicketTypeModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.ServiceTicketTypes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateServiceTicketType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTicketTypes.Create);
            CanEditServiceTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceTicketTypes.Edit);
            CanDeleteServiceTicketType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceTicketTypes.Delete);
                            
                            
        }

        private async Task GetServiceTicketTypesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ServiceTicketTypesAppService.GetListAsync(Filter);
            ServiceTicketTypeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetServiceTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ServiceTicketTypesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/service-ticket-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ServiceTicketTypeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetServiceTicketTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateServiceTicketTypeModalAsync()
        {
            NewServiceTicketType = new ServiceTicketTypeCreateDto{
                
                
            };

            SelectedCreateTab = "serviceTicketType-create-tab";
            
            
            await NewServiceTicketTypeValidations.ClearAll();
            await CreateServiceTicketTypeModal.Show();
        }

        private async Task CloseCreateServiceTicketTypeModalAsync()
        {
            NewServiceTicketType = new ServiceTicketTypeCreateDto{
                
                
            };
            await CreateServiceTicketTypeModal.Hide();
        }

        private async Task OpenEditServiceTicketTypeModalAsync(ServiceTicketTypeDto input)
        {
            SelectedEditTab = "serviceTicketType-edit-tab";
            
            
            var serviceTicketType = await ServiceTicketTypesAppService.GetAsync(input.Id);
            
            EditingServiceTicketTypeId = serviceTicketType.Id;
            EditingServiceTicketType = ObjectMapper.Map<ServiceTicketTypeDto, ServiceTicketTypeUpdateDto>(serviceTicketType);
            
            await EditingServiceTicketTypeValidations.ClearAll();
            await EditServiceTicketTypeModal.Show();
        }

        private async Task DeleteServiceTicketTypeAsync(ServiceTicketTypeDto input)
        {
            await ServiceTicketTypesAppService.DeleteAsync(input.Id);
            await GetServiceTicketTypesAsync();
        }

        private async Task CreateServiceTicketTypeAsync()
        {
            try
            {
                if (await NewServiceTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTicketTypesAppService.CreateAsync(NewServiceTicketType);
                await GetServiceTicketTypesAsync();
                await CloseCreateServiceTicketTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditServiceTicketTypeModalAsync()
        {
            await EditServiceTicketTypeModal.Hide();
        }

        private async Task UpdateServiceTicketTypeAsync()
        {
            try
            {
                if (await EditingServiceTicketTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTicketTypesAppService.UpdateAsync(EditingServiceTicketTypeId, EditingServiceTicketType);
                await GetServiceTicketTypesAsync();
                await EditServiceTicketTypeModal.Hide();                
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
            AllServiceTicketTypesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllServiceTicketTypesSelected = false;
            SelectedServiceTicketTypes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedServiceTicketTypeRowsChanged()
        {
            if (SelectedServiceTicketTypes.Count != PageSize)
            {
                AllServiceTicketTypesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedServiceTicketTypesAsync()
        {
            var message = AllServiceTicketTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedServiceTicketTypes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllServiceTicketTypesSelected)
            {
                await ServiceTicketTypesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ServiceTicketTypesAppService.DeleteByIdsAsync(SelectedServiceTicketTypes.Select(x => x.Id).ToList());
            }

            SelectedServiceTicketTypes.Clear();
            AllServiceTicketTypesSelected = false;

            await GetServiceTicketTypesAsync();
        }


    }
}
