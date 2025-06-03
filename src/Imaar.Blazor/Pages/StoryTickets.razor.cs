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
using Imaar.StoryTickets;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class StoryTickets
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<StoryTicketWithNavigationPropertiesDto> StoryTicketList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateStoryTicket { get; set; }
        private bool CanEditStoryTicket { get; set; }
        private bool CanDeleteStoryTicket { get; set; }
        private StoryTicketCreateDto NewStoryTicket { get; set; }
        private Validations NewStoryTicketValidations { get; set; } = new();
        private StoryTicketUpdateDto EditingStoryTicket { get; set; }
        private Validations EditingStoryTicketValidations { get; set; } = new();
        private Guid EditingStoryTicketId { get; set; }
        private Modal CreateStoryTicketModal { get; set; } = new();
        private Modal EditStoryTicketModal { get; set; } = new();
        private GetStoryTicketsInput Filter { get; set; }
        private DataGridEntityActionsColumn<StoryTicketWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "storyTicket-create-tab";
        protected string SelectedEditTab = "storyTicket-edit-tab";
        private StoryTicketWithNavigationPropertiesDto? SelectedStoryTicket;
        private IReadOnlyList<LookupDto<Guid>> StoryTicketTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> StoriesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<StoryTicketWithNavigationPropertiesDto> SelectedStoryTickets { get; set; } = new();
        private bool AllStoryTicketsSelected { get; set; }
        
        public StoryTickets()
        {
            NewStoryTicket = new StoryTicketCreateDto();
            EditingStoryTicket = new StoryTicketUpdateDto();
            Filter = new GetStoryTicketsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            StoryTicketList = new List<StoryTicketWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetStoryTicketTypeCollectionLookupAsync();


            await GetUserProfileCollectionLookupAsync();


            await GetStoryCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["StoryTickets"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewStoryTicket"], async () =>
            {
                await OpenCreateStoryTicketModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.StoryTickets.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateStoryTicket = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.StoryTickets.Create);
            CanEditStoryTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryTickets.Edit);
            CanDeleteStoryTicket = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.StoryTickets.Delete);
                            
                            
        }

        private async Task GetStoryTicketsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await StoryTicketsAppService.GetListAsync(Filter);
            StoryTicketList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetStoryTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await StoryTicketsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/story-tickets/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Description={HttpUtility.UrlEncode(Filter.Description)}&StoryTicketTypeId={Filter.StoryTicketTypeId}&TicketCreatorId={Filter.TicketCreatorId}&StoryAgainstId={Filter.StoryAgainstId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StoryTicketWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetStoryTicketsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateStoryTicketModalAsync()
        {
            NewStoryTicket = new StoryTicketCreateDto{
                
                StoryTicketTypeId = StoryTicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
StoryAgainstId = StoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "storyTicket-create-tab";
            
            
            await NewStoryTicketValidations.ClearAll();
            await CreateStoryTicketModal.Show();
        }

        private async Task CloseCreateStoryTicketModalAsync()
        {
            NewStoryTicket = new StoryTicketCreateDto{
                
                StoryTicketTypeId = StoryTicketTypesCollection.Select(i=>i.Id).FirstOrDefault(),
TicketCreatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
StoryAgainstId = StoriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateStoryTicketModal.Hide();
        }

        private async Task OpenEditStoryTicketModalAsync(StoryTicketWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "storyTicket-edit-tab";
            
            
            var storyTicket = await StoryTicketsAppService.GetWithNavigationPropertiesAsync(input.StoryTicket.Id);
            
            EditingStoryTicketId = storyTicket.StoryTicket.Id;
            EditingStoryTicket = ObjectMapper.Map<StoryTicketDto, StoryTicketUpdateDto>(storyTicket.StoryTicket);
            
            await EditingStoryTicketValidations.ClearAll();
            await EditStoryTicketModal.Show();
        }

        private async Task DeleteStoryTicketAsync(StoryTicketWithNavigationPropertiesDto input)
        {
            await StoryTicketsAppService.DeleteAsync(input.StoryTicket.Id);
            await GetStoryTicketsAsync();
        }

        private async Task CreateStoryTicketAsync()
        {
            try
            {
                if (await NewStoryTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryTicketsAppService.CreateAsync(NewStoryTicket);
                await GetStoryTicketsAsync();
                await CloseCreateStoryTicketModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditStoryTicketModalAsync()
        {
            await EditStoryTicketModal.Hide();
        }

        private async Task UpdateStoryTicketAsync()
        {
            try
            {
                if (await EditingStoryTicketValidations.ValidateAll() == false)
                {
                    return;
                }

                await StoryTicketsAppService.UpdateAsync(EditingStoryTicketId, EditingStoryTicket);
                await GetStoryTicketsAsync();
                await EditStoryTicketModal.Hide();                
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
        protected virtual async Task OnStoryTicketTypeIdChangedAsync(Guid? storyTicketTypeId)
        {
            Filter.StoryTicketTypeId = storyTicketTypeId;
            await SearchAsync();
        }
        protected virtual async Task OnTicketCreatorIdChangedAsync(Guid? ticketCreatorId)
        {
            Filter.TicketCreatorId = ticketCreatorId;
            await SearchAsync();
        }
        protected virtual async Task OnStoryAgainstIdChangedAsync(Guid? storyAgainstId)
        {
            Filter.StoryAgainstId = storyAgainstId;
            await SearchAsync();
        }
        

        private async Task GetStoryTicketTypeCollectionLookupAsync(string? newValue = null)
        {
            StoryTicketTypesCollection = (await StoryTicketsAppService.GetStoryTicketTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await StoryTicketsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetStoryCollectionLookupAsync(string? newValue = null)
        {
            StoriesCollection = (await StoryTicketsAppService.GetStoryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllStoryTicketsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllStoryTicketsSelected = false;
            SelectedStoryTickets.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedStoryTicketRowsChanged()
        {
            if (SelectedStoryTickets.Count != PageSize)
            {
                AllStoryTicketsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedStoryTicketsAsync()
        {
            var message = AllStoryTicketsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedStoryTickets.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllStoryTicketsSelected)
            {
                await StoryTicketsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await StoryTicketsAppService.DeleteByIdsAsync(SelectedStoryTickets.Select(x => x.StoryTicket.Id).ToList());
            }

            SelectedStoryTickets.Clear();
            AllStoryTicketsSelected = false;

            await GetStoryTicketsAsync();
        }


    }
}
