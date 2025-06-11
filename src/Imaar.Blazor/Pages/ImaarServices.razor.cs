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
using Imaar.ImaarServices;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class ImaarServices
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ImaarServiceWithNavigationPropertiesDto> ImaarServiceList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateImaarService { get; set; }
        private bool CanEditImaarService { get; set; }
        private bool CanDeleteImaarService { get; set; }
        private ImaarServiceCreateDto NewImaarService { get; set; }
        private Validations NewImaarServiceValidations { get; set; } = new();
        private ImaarServiceUpdateDto EditingImaarService { get; set; }
        private Validations EditingImaarServiceValidations { get; set; } = new();
        private Guid EditingImaarServiceId { get; set; }
        private Modal CreateImaarServiceModal { get; set; } = new();
        private Modal EditImaarServiceModal { get; set; } = new();
        private GetImaarServicesInput Filter { get; set; }
        private DataGridEntityActionsColumn<ImaarServiceWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "imaarService-create-tab";
        protected string SelectedEditTab = "imaarService-edit-tab";
        private ImaarServiceWithNavigationPropertiesDto? SelectedImaarService;
        private IReadOnlyList<LookupDto<Guid>> ServiceTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<ImaarServiceWithNavigationPropertiesDto> SelectedImaarServices { get; set; } = new();
        private bool AllImaarServicesSelected { get; set; }
        
        public ImaarServices()
        {
            NewImaarService = new ImaarServiceCreateDto();
            EditingImaarService = new ImaarServiceUpdateDto();
            Filter = new GetImaarServicesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ImaarServiceList = new List<ImaarServiceWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetServiceTypeCollectionLookupAsync();


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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ImaarServices"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewImaarService"], async () =>
            {
                await OpenCreateImaarServiceModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.ImaarServices.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateImaarService = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ImaarServices.Create);
            CanEditImaarService = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ImaarServices.Edit);
            CanDeleteImaarService = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.ImaarServices.Delete);
                            
                            
        }

        private async Task GetImaarServicesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ImaarServicesAppService.GetListAsync(Filter);
            ImaarServiceList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetImaarServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ImaarServicesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/imaar-services/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Description={HttpUtility.UrlEncode(Filter.Description)}&ServiceLocation={HttpUtility.UrlEncode(Filter.ServiceLocation)}&ServiceNumber={HttpUtility.UrlEncode(Filter.ServiceNumber)}&DateOfPublishMin={Filter.DateOfPublishMin}&DateOfPublishMax={Filter.DateOfPublishMax}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&Latitude={HttpUtility.UrlEncode(Filter.Latitude)}&Longitude={HttpUtility.UrlEncode(Filter.Longitude)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&ViewCounterMin={Filter.ViewCounterMin}&ViewCounterMax={Filter.ViewCounterMax}&OrderCounterMin={Filter.OrderCounterMin}&OrderCounterMax={Filter.OrderCounterMax}&ServiceTypeId={Filter.ServiceTypeId}&UserProfileId={Filter.UserProfileId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ImaarServiceWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetImaarServicesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateImaarServiceModalAsync()
        {
            NewImaarService = new ImaarServiceCreateDto{
                DateOfPublish = DateOnly.FromDateTime(DateTime.Now),

                ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "imaarService-create-tab";
            
            
            await NewImaarServiceValidations.ClearAll();
            await CreateImaarServiceModal.Show();
        }

        private async Task CloseCreateImaarServiceModalAsync()
        {
            NewImaarService = new ImaarServiceCreateDto{
                DateOfPublish = DateOnly.FromDateTime(DateTime.Now),

                ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateImaarServiceModal.Hide();
        }

        private async Task OpenEditImaarServiceModalAsync(ImaarServiceWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "imaarService-edit-tab";
            
            
            var imaarService = await ImaarServicesAppService.GetWithNavigationPropertiesAsync(input.ImaarService.Id);
            
            EditingImaarServiceId = imaarService.ImaarService.Id;
            EditingImaarService = ObjectMapper.Map<ImaarServiceDto, ImaarServiceUpdateDto>(imaarService.ImaarService);
            
            await EditingImaarServiceValidations.ClearAll();
            await EditImaarServiceModal.Show();
        }

        private async Task DeleteImaarServiceAsync(ImaarServiceWithNavigationPropertiesDto input)
        {
            await ImaarServicesAppService.DeleteAsync(input.ImaarService.Id);
            await GetImaarServicesAsync();
        }

        private async Task CreateImaarServiceAsync()
        {
            try
            {
                if (await NewImaarServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await ImaarServicesAppService.CreateAsync(NewImaarService);
                await GetImaarServicesAsync();
                await CloseCreateImaarServiceModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditImaarServiceModalAsync()
        {
            await EditImaarServiceModal.Hide();
        }

        private async Task UpdateImaarServiceAsync()
        {
            try
            {
                if (await EditingImaarServiceValidations.ValidateAll() == false)
                {
                    return;
                }

                await ImaarServicesAppService.UpdateAsync(EditingImaarServiceId, EditingImaarService);
                await GetImaarServicesAsync();
                await EditImaarServiceModal.Hide();                
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
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnServiceLocationChangedAsync(string? serviceLocation)
        {
            Filter.ServiceLocation = serviceLocation;
            await SearchAsync();
        }
        protected virtual async Task OnServiceNumberChangedAsync(string? serviceNumber)
        {
            Filter.ServiceNumber = serviceNumber;
            await SearchAsync();
        }
        protected virtual async Task OnDateOfPublishMinChangedAsync(DateOnly? dateOfPublishMin)
        {
            Filter.DateOfPublishMin = dateOfPublishMin;
            await SearchAsync();
        }
        protected virtual async Task OnDateOfPublishMaxChangedAsync(DateOnly? dateOfPublishMax)
        {
            Filter.DateOfPublishMax = dateOfPublishMax;
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
        protected virtual async Task OnLatitudeChangedAsync(string? latitude)
        {
            Filter.Latitude = latitude;
            await SearchAsync();
        }
        protected virtual async Task OnLongitudeChangedAsync(string? longitude)
        {
            Filter.Longitude = longitude;
            await SearchAsync();
        }
        protected virtual async Task OnPhoneNumberChangedAsync(string? phoneNumber)
        {
            Filter.PhoneNumber = phoneNumber;
            await SearchAsync();
        }
        protected virtual async Task OnViewCounterMinChangedAsync(int? viewCounterMin)
        {
            Filter.ViewCounterMin = viewCounterMin;
            await SearchAsync();
        }
        protected virtual async Task OnViewCounterMaxChangedAsync(int? viewCounterMax)
        {
            Filter.ViewCounterMax = viewCounterMax;
            await SearchAsync();
        }
        protected virtual async Task OnOrderCounterMinChangedAsync(int? orderCounterMin)
        {
            Filter.OrderCounterMin = orderCounterMin;
            await SearchAsync();
        }
        protected virtual async Task OnOrderCounterMaxChangedAsync(int? orderCounterMax)
        {
            Filter.OrderCounterMax = orderCounterMax;
            await SearchAsync();
        }
        protected virtual async Task OnServiceTypeIdChangedAsync(Guid? serviceTypeId)
        {
            Filter.ServiceTypeId = serviceTypeId;
            await SearchAsync();
        }
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        

        private async Task GetServiceTypeCollectionLookupAsync(string? newValue = null)
        {
            ServiceTypesCollection = (await ImaarServicesAppService.GetServiceTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await ImaarServicesAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllImaarServicesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllImaarServicesSelected = false;
            SelectedImaarServices.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedImaarServiceRowsChanged()
        {
            if (SelectedImaarServices.Count != PageSize)
            {
                AllImaarServicesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedImaarServicesAsync()
        {
            var message = AllImaarServicesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedImaarServices.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllImaarServicesSelected)
            {
                await ImaarServicesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ImaarServicesAppService.DeleteByIdsAsync(SelectedImaarServices.Select(x => x.ImaarService.Id).ToList());
            }

            SelectedImaarServices.Clear();
            AllImaarServicesSelected = false;

            await GetImaarServicesAsync();
        }


    }
}
