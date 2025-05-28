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
using Imaar.ServiceTickets;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

using Imaar.ServiceTickets;



namespace Imaar.Blazor.Pages
{
    public partial class ServiceTickets
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ServiceTicketWithNavigationPropertiesDto> ServiceTicketList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateServiceTicket { get; set; }
        private bool CanEditServiceTicket { get; set; }
        private bool CanDeleteServiceTicket { get; set; }
        private ServiceTicketCreateDto NewServiceTicket { get; set; }
        private Validations NewServiceTicketValidations { get; set; } = new();
        private ServiceTicketUpdateDto EditingServiceTicket { get; set; }
        private Validations EditingServiceTicketValidations { get; set; } = new();
        private Guid EditingServiceTicketId { get; set; }
        private Modal CreateServiceTicketModal { get; set; } = new();
        private Modal EditServiceTicketModal { get; set; } = new();
        private GetServiceTicketsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ServiceTicketWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "serviceTicket-create-tab";
        protected string SelectedEditTab = "serviceTicket-edit-tab";
        private ServiceTicketWithNavigationPropertiesDto? SelectedServiceTicket;
        private IReadOnlyList<LookupDto<Guid>> ServiceTicketTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<ServiceTicketWithNavigationPropertiesDto> SelectedServiceTickets { get; set; } = new();
        private bool AllServiceTicketsSelected { get; set; }
        
        public ServiceTickets()
        {
            NewServiceTicket = new ServiceTicketCreateDto();
            EditingServiceTicket = new ServiceTicketUpdateDto();
            Filter = new GetServiceTicketsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ServiceTicketList = new List<ServiceTicketWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetServiceTicketTypeCollectionLookupAsync();


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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ServiceTickets"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewServiceTicket"], async () =>
            {
                await OpenCreateServiceTicketModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.ServiceTickets.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateServiceTicket = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTickets.Create);
            CanEditServiceTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceTickets.Edit);
            CanDeleteServiceTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ServiceTickets.Delete);
                            
                            
        }

        private async Task GetServiceTicketsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ServiceTicketsAppService.GetListAsync(Filter);
            ServiceTicketList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetServiceTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ServiceTicketsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/service-tickets/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Description ={HttpUtility.UrlEncode(Filter.Description )}&TicketEntityType={Filter.TicketEntityType}&TicketEntityId={HttpUtility.UrlEncode(Filter.TicketEntityId)}&ServiceTicketTypeId={Filter.ServiceTicketTypeId}&TicketCreatorId={Filter.TicketCreatorId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ServiceTicketWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetServiceTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateServiceTicketModalAsync()
        {
            NewServiceTicket = new ServiceTicketCreateDto{
                
                ServiceTicketTypeId = ServiceTicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "serviceTicket-create-tab";
            
            
            await NewServiceTicketValidations.ClearAll();
            await CreateServiceTicketModal.Show();
        }

        private async Task CloseCreateServiceTicketModalAsync()
        {
            NewServiceTicket = new ServiceTicketCreateDto{
                
                ServiceTicketTypeId = ServiceTicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateServiceTicketModal.Hide();
        }

        private async Task OpenEditServiceTicketModalAsync(ServiceTicketWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "serviceTicket-edit-tab";
            
            
            var serviceTicket = await ServiceTicketsAppService.GetWithNavigationPropertiesAsync(input.ServiceTicket.Id);
            
            EditingServiceTicketId = serviceTicket.ServiceTicket.Id;
            EditingServiceTicket = ObjectMapper.Map<ServiceTicketDto, ServiceTicketUpdateDto>(serviceTicket.ServiceTicket);
            
            await EditingServiceTicketValidations.ClearAll();
            await EditServiceTicketModal.Show();
        }

        private async Task DeleteServiceTicketAsync(ServiceTicketWithNavigationPropertiesDto input)
        {
            await ServiceTicketsAppService.DeleteAsync(input.ServiceTicket.Id);
            await GetServiceTicketsAsync();
        }

        private async Task CreateServiceTicketAsync()
        {
            try
            {
                if (await NewServiceTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTicketsAppService.CreateAsync(NewServiceTicket);
                await GetServiceTicketsAsync();
                await CloseCreateServiceTicketModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditServiceTicketModalAsync()
        {
            await EditServiceTicketModal.Hide();
        }

        private async Task UpdateServiceTicketAsync()
        {
            try
            {
                if (await EditingServiceTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTicketsAppService.UpdateAsync(EditingServiceTicketId, EditingServiceTicket);
                await GetServiceTicketsAsync();
                await EditServiceTicketModal.Hide();                
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









        //protected virtual async Task OnDescription ChangedAsync(string? description )
        //{
        //    Filter.Description  = description ;
        //    await SearchAsync();
        //}
        protected virtual async Task OnTicketEntityTypeChangedAsync(TicketEntityType? ticketEntityType)
        {
            Filter.TicketEntityType = ticketEntityType;
            await SearchAsync();
        }
        protected virtual async Task OnTicketEntityIdChangedAsync(string? ticketEntityId)
        {
            Filter.TicketEntityId = ticketEntityId;
            await SearchAsync();
        }
        protected virtual async Task OnServiceTicketTypeIdChangedAsync(Guid? serviceTicketTypeId)
        {
            Filter.ServiceTicketTypeId = serviceTicketTypeId;
            await SearchAsync();
        }
        protected virtual async Task OnTicketCreatorIdChangedAsync(Guid? ticketCreatorId)
        {
            Filter.TicketCreatorId = ticketCreatorId;
            await SearchAsync();
        }
        

        private async Task GetServiceTicketTypeCollectionLookupAsync(string? newValue = null)
        {
            ServiceTicketTypesCollection = (await ServiceTicketsAppService.GetServiceTicketTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await ServiceTicketsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllServiceTicketsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllServiceTicketsSelected = false;
            SelectedServiceTickets.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedServiceTicketRowsChanged()
        {
            if (SelectedServiceTickets.Count != PageSize)
            {
                AllServiceTicketsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedServiceTicketsAsync()
        {
            var message = AllServiceTicketsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedServiceTickets.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllServiceTicketsSelected)
            {
                await ServiceTicketsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ServiceTicketsAppService.DeleteByIdsAsync(SelectedServiceTickets.Select(x => x.ServiceTicket.Id).ToList());
            }

            SelectedServiceTickets.Clear();
            AllServiceTicketsSelected = false;

            await GetServiceTicketsAsync();
        }


    }
}
