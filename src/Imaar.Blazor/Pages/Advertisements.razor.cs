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
using Imaar.Advertisements;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Advertisements
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<AdvertisementWithNavigationPropertiesDto> AdvertisementList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateAdvertisement { get; set; }
        private bool CanEditAdvertisement { get; set; }
        private bool CanDeleteAdvertisement { get; set; }
        private AdvertisementCreateDto NewAdvertisement { get; set; }
        private Validations NewAdvertisementValidations { get; set; } = new();
        private AdvertisementUpdateDto EditingAdvertisement { get; set; }
        private Validations EditingAdvertisementValidations { get; set; } = new();
        private Guid EditingAdvertisementId { get; set; }
        private Modal CreateAdvertisementModal { get; set; } = new();
        private Modal EditAdvertisementModal { get; set; } = new();
        private GetAdvertisementsInput Filter { get; set; }
        private DataGridEntityActionsColumn<AdvertisementWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "advertisement-create-tab";
        protected string SelectedEditTab = "advertisement-edit-tab";
        private AdvertisementWithNavigationPropertiesDto? SelectedAdvertisement;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<AdvertisementWithNavigationPropertiesDto> SelectedAdvertisements { get; set; } = new();
        private bool AllAdvertisementsSelected { get; set; }
        
        public Advertisements()
        {
            NewAdvertisement = new AdvertisementCreateDto();
            EditingAdvertisement = new AdvertisementUpdateDto();
            Filter = new GetAdvertisementsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            AdvertisementList = new List<AdvertisementWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Advertisements"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewAdvertisement"], async () =>
            {
                await OpenCreateAdvertisementModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Advertisements.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAdvertisement = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Advertisements.Create);
            CanEditAdvertisement = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Advertisements.Edit);
            CanDeleteAdvertisement = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Advertisements.Delete);
                            
                            
        }

        private async Task GetAdvertisementsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AdvertisementsAppService.GetListAsync(Filter);
            AdvertisementList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAdvertisementsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await AdvertisementsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/advertisements/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&SubTitle={HttpUtility.UrlEncode(Filter.SubTitle)}&File={HttpUtility.UrlEncode(Filter.File)}&FromDateTimeMin={Filter.FromDateTimeMin?.ToString("O")}&FromDateTimeMax={Filter.FromDateTimeMax?.ToString("O")}&ToDateTimeMin={Filter.ToDateTimeMin?.ToString("O")}&ToDateTimeMax={Filter.ToDateTimeMax?.ToString("O")}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}&UserProfileId={Filter.UserProfileId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AdvertisementWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetAdvertisementsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateAdvertisementModalAsync()
        {
            NewAdvertisement = new AdvertisementCreateDto{
                FromDateTime = DateTime.Now,
ToDateTime = DateTime.Now,

                
            };

            SelectedCreateTab = "advertisement-create-tab";
            
            
            await NewAdvertisementValidations.ClearAll();
            await CreateAdvertisementModal.Show();
        }

        private async Task CloseCreateAdvertisementModalAsync()
        {
            NewAdvertisement = new AdvertisementCreateDto{
                FromDateTime = DateTime.Now,
ToDateTime = DateTime.Now,

                
            };
            await CreateAdvertisementModal.Hide();
        }

        private async Task OpenEditAdvertisementModalAsync(AdvertisementWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "advertisement-edit-tab";
            
            
            var advertisement = await AdvertisementsAppService.GetWithNavigationPropertiesAsync(input.Advertisement.Id);
            
            EditingAdvertisementId = advertisement.Advertisement.Id;
            EditingAdvertisement = ObjectMapper.Map<AdvertisementDto, AdvertisementUpdateDto>(advertisement.Advertisement);
            
            await EditingAdvertisementValidations.ClearAll();
            await EditAdvertisementModal.Show();
        }

        private async Task DeleteAdvertisementAsync(AdvertisementWithNavigationPropertiesDto input)
        {
            await AdvertisementsAppService.DeleteAsync(input.Advertisement.Id);
            await GetAdvertisementsAsync();
        }

        private async Task CreateAdvertisementAsync()
        {
            try
            {
                if (await NewAdvertisementValidations.ValidateAll() == false)
                {
                    return;
                }

                await AdvertisementsAppService.CreateAsync(NewAdvertisement);
                await GetAdvertisementsAsync();
                await CloseCreateAdvertisementModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditAdvertisementModalAsync()
        {
            await EditAdvertisementModal.Hide();
        }

        private async Task UpdateAdvertisementAsync()
        {
            try
            {
                if (await EditingAdvertisementValidations.ValidateAll() == false)
                {
                    return;
                }

                await AdvertisementsAppService.UpdateAsync(EditingAdvertisementId, EditingAdvertisement);
                await GetAdvertisementsAsync();
                await EditAdvertisementModal.Hide();                
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
        protected virtual async Task OnSubTitleChangedAsync(string? subTitle)
        {
            Filter.SubTitle = subTitle;
            await SearchAsync();
        }
        protected virtual async Task OnFileChangedAsync(string? file)
        {
            Filter.File = file;
            await SearchAsync();
        }
        protected virtual async Task OnFromDateTimeMinChangedAsync(DateTime? fromDateTimeMin)
        {
            Filter.FromDateTimeMin = fromDateTimeMin.HasValue ? fromDateTimeMin.Value.Date : fromDateTimeMin;
            await SearchAsync();
        }
        protected virtual async Task OnFromDateTimeMaxChangedAsync(DateTime? fromDateTimeMax)
        {
            Filter.FromDateTimeMax = fromDateTimeMax.HasValue ? fromDateTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : fromDateTimeMax;
            await SearchAsync();
        }
        protected virtual async Task OnToDateTimeMinChangedAsync(DateTime? toDateTimeMin)
        {
            Filter.ToDateTimeMin = toDateTimeMin.HasValue ? toDateTimeMin.Value.Date : toDateTimeMin;
            await SearchAsync();
        }
        protected virtual async Task OnToDateTimeMaxChangedAsync(DateTime? toDateTimeMax)
        {
            Filter.ToDateTimeMax = toDateTimeMax.HasValue ? toDateTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : toDateTimeMax;
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
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await AdvertisementsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllAdvertisementsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllAdvertisementsSelected = false;
            SelectedAdvertisements.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedAdvertisementRowsChanged()
        {
            if (SelectedAdvertisements.Count != PageSize)
            {
                AllAdvertisementsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedAdvertisementsAsync()
        {
            var message = AllAdvertisementsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedAdvertisements.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllAdvertisementsSelected)
            {
                await AdvertisementsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await AdvertisementsAppService.DeleteByIdsAsync(SelectedAdvertisements.Select(x => x.Advertisement.Id).ToList());
            }

            SelectedAdvertisements.Clear();
            AllAdvertisementsSelected = false;

            await GetAdvertisementsAsync();
        }


    }
}
