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
using Imaar.Tickets;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Tickets
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<TicketWithNavigationPropertiesDto> TicketList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateTicket { get; set; }
        private bool CanEditTicket { get; set; }
        private bool CanDeleteTicket { get; set; }
        private TicketCreateDto NewTicket { get; set; }
        private Validations NewTicketValidations { get; set; } = new();
        private TicketUpdateDto EditingTicket { get; set; }
        private Validations EditingTicketValidations { get; set; } = new();
        private Guid EditingTicketId { get; set; }
        private Modal CreateTicketModal { get; set; } = new();
        private Modal EditTicketModal { get; set; } = new();
        private GetTicketsInput Filter { get; set; }
        private DataGridEntityActionsColumn<TicketWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "ticket-create-tab";
        protected string SelectedEditTab = "ticket-edit-tab";
        private TicketWithNavigationPropertiesDto? SelectedTicket;
        private IReadOnlyList<LookupDto<Guid>> TicketTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<TicketWithNavigationPropertiesDto> SelectedTickets { get; set; } = new();
        private bool AllTicketsSelected { get; set; }
        
        public Tickets()
        {
            NewTicket = new TicketCreateDto();
            EditingTicket = new TicketUpdateDto();
            Filter = new GetTicketsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            TicketList = new List<TicketWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetTicketTypeCollectionLookupAsync();


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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Tickets"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewTicket"], async () =>
            {
                await OpenCreateTicketModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Tickets.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTicket = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Tickets.Create);
            CanEditTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Tickets.Edit);
            CanDeleteTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Tickets.Delete);
                            
                            
        }

        private async Task GetTicketsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await TicketsAppService.GetListAsync(Filter);
            TicketList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await TicketsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/tickets/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Description={HttpUtility.UrlEncode(Filter.Description)}&TicketTypeId={Filter.TicketTypeId}&TicketCreatorId={Filter.TicketCreatorId}&TicketAgainstId={Filter.TicketAgainstId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TicketWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateTicketModalAsync()
        {
            NewTicket = new TicketCreateDto{
                
                TicketTypeId = TicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketAgainstId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "ticket-create-tab";
            
            
            await NewTicketValidations.ClearAll();
            await CreateTicketModal.Show();
        }

        private async Task CloseCreateTicketModalAsync()
        {
            NewTicket = new TicketCreateDto{
                
                TicketTypeId = TicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketAgainstId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateTicketModal.Hide();
        }

        private async Task OpenEditTicketModalAsync(TicketWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "ticket-edit-tab";
            
            
            var ticket = await TicketsAppService.GetWithNavigationPropertiesAsync(input.Ticket.Id);
            
            EditingTicketId = ticket.Ticket.Id;
            EditingTicket = ObjectMapper.Map<TicketDto, TicketUpdateDto>(ticket.Ticket);
            
            await EditingTicketValidations.ClearAll();
            await EditTicketModal.Show();
        }

        private async Task DeleteTicketAsync(TicketWithNavigationPropertiesDto input)
        {
            await TicketsAppService.DeleteAsync(input.Ticket.Id);
            await GetTicketsAsync();
        }

        private async Task CreateTicketAsync()
        {
            try
            {
                if (await NewTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await TicketsAppService.CreateAsync(NewTicket);
                await GetTicketsAsync();
                await CloseCreateTicketModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditTicketModalAsync()
        {
            await EditTicketModal.Hide();
        }

        private async Task UpdateTicketAsync()
        {
            try
            {
                if (await EditingTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await TicketsAppService.UpdateAsync(EditingTicketId, EditingTicket);
                await GetTicketsAsync();
                await EditTicketModal.Hide();                
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









        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnTicketTypeIdChangedAsync(Guid? ticketTypeId)
        {
            Filter.TicketTypeId = ticketTypeId;
            await SearchAsync();
        }
        protected virtual async Task OnTicketCreatorIdChangedAsync(Guid? ticketCreatorId)
        {
            Filter.TicketCreatorId = ticketCreatorId;
            await SearchAsync();
        }
        protected virtual async Task OnTicketAgainstIdChangedAsync(Guid? ticketAgainstId)
        {
            Filter.TicketAgainstId = ticketAgainstId;
            await SearchAsync();
        }
        

        private async Task GetTicketTypeCollectionLookupAsync(string? newValue = null)
        {
            TicketTypesCollection = (await TicketsAppService.GetTicketTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await TicketsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllTicketsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllTicketsSelected = false;
            SelectedTickets.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedTicketRowsChanged()
        {
            if (SelectedTickets.Count != PageSize)
            {
                AllTicketsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedTicketsAsync()
        {
            var message = AllTicketsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedTickets.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllTicketsSelected)
            {
                await TicketsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await TicketsAppService.DeleteByIdsAsync(SelectedTickets.Select(x => x.Ticket.Id).ToList());
            }

            SelectedTickets.Clear();
            AllTicketsSelected = false;

            await GetTicketsAsync();
        }


    }
}
