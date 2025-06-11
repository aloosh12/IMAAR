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
using Imaar.Buildings;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class Buildings
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<BuildingWithNavigationPropertiesDto> BuildingList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBuilding { get; set; }
        private bool CanEditBuilding { get; set; }
        private bool CanDeleteBuilding { get; set; }
        private BuildingCreateDto NewBuilding { get; set; }
        private Validations NewBuildingValidations { get; set; } = new();
        private BuildingUpdateDto EditingBuilding { get; set; }
        private Validations EditingBuildingValidations { get; set; } = new();
        private Guid EditingBuildingId { get; set; }
        private Modal CreateBuildingModal { get; set; } = new();
        private Modal EditBuildingModal { get; set; } = new();
        private GetBuildingsInput Filter { get; set; }
        private DataGridEntityActionsColumn<BuildingWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "building-create-tab";
        protected string SelectedEditTab = "building-edit-tab";
        private BuildingWithNavigationPropertiesDto? SelectedBuilding;
        private IReadOnlyList<LookupDto<Guid>> RegionsCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> FurnishingLevelsCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> BuildingFacadesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> ServiceTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> MainAmenities { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedMainAmenityId { get; set; }
        
        private string SelectedMainAmenityText { get; set; }

        private Blazorise.Components.Autocomplete<LookupDto<Guid>, string> SelectedMainAmenityAutoCompleteRef { get; set; } = new();

        private List<LookupDto<Guid>> SelectedMainAmenities { get; set; } = new List<LookupDto<Guid>>();private IReadOnlyList<LookupDto<Guid>> SecondaryAmenities { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedSecondaryAmenityId { get; set; }
        
        private string SelectedSecondaryAmenityText { get; set; }

        private Blazorise.Components.Autocomplete<LookupDto<Guid>, string> SelectedSecondaryAmenityAutoCompleteRef { get; set; } = new();

        private List<LookupDto<Guid>> SelectedSecondaryAmenities { get; set; } = new List<LookupDto<Guid>>();
        
        
        
        
        private List<BuildingWithNavigationPropertiesDto> SelectedBuildings { get; set; } = new();
        private bool AllBuildingsSelected { get; set; }
        
        public Buildings()
        {
            NewBuilding = new BuildingCreateDto();
            EditingBuilding = new BuildingUpdateDto();
            Filter = new GetBuildingsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BuildingList = new List<BuildingWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetRegionCollectionLookupAsync();


            await GetFurnishingLevelCollectionLookupAsync();


            await GetBuildingFacadeCollectionLookupAsync();


            await GetServiceTypeCollectionLookupAsync();


            await GetUserProfileCollectionLookupAsync();


            await GetMainAmenityLookupAsync();


            await GetSecondaryAmenityLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Buildings"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewBuilding"], async () =>
            {
                await OpenCreateBuildingModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Buildings.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateBuilding = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Buildings.Create);
            CanEditBuilding = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Buildings.Edit);
            CanDeleteBuilding = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Buildings.Delete);
                            
                            
        }

        private async Task GetBuildingsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BuildingsAppService.GetListAsync(Filter);
            BuildingList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetBuildingsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BuildingsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/buildings/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&MainTitle={HttpUtility.UrlEncode(Filter.MainTitle)}&Description={HttpUtility.UrlEncode(Filter.Description)}&Price={HttpUtility.UrlEncode(Filter.Price)}&BuildingArea={HttpUtility.UrlEncode(Filter.BuildingArea)}&NumberOfRooms={HttpUtility.UrlEncode(Filter.NumberOfRooms)}&NumberOfBaths={HttpUtility.UrlEncode(Filter.NumberOfBaths)}&FloorNo={HttpUtility.UrlEncode(Filter.FloorNo)}&Latitude={HttpUtility.UrlEncode(Filter.Latitude)}&Longitude={HttpUtility.UrlEncode(Filter.Longitude)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&ViewCounterMin={Filter.ViewCounterMin}&ViewCounterMax={Filter.ViewCounterMax}&OrderCounterMin={Filter.OrderCounterMin}&OrderCounterMax={Filter.OrderCounterMax}&RegionId={Filter.RegionId}&FurnishingLevelId={Filter.FurnishingLevelId}&BuildingFacadeId={Filter.BuildingFacadeId}&ServiceTypeId={Filter.ServiceTypeId}&UserProfileId={Filter.UserProfileId}&MainAmenityId={Filter.MainAmenityId}&SecondaryAmenityId={Filter.SecondaryAmenityId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BuildingWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetBuildingsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateBuildingModalAsync()
        {
            SelectedMainAmenities = new List<LookupDto<Guid>>();
            SelectedMainAmenityId = string.Empty;
            SelectedMainAmenityText = string.Empty;

            await SelectedMainAmenityAutoCompleteRef.Clear();

            SelectedSecondaryAmenities = new List<LookupDto<Guid>>();
            SelectedSecondaryAmenityId = string.Empty;
            SelectedSecondaryAmenityText = string.Empty;

            await SelectedSecondaryAmenityAutoCompleteRef.Clear();

            NewBuilding = new BuildingCreateDto{
                
                RegionId = RegionsCollection.Select(i=>i.Id).FirstOrDefault(),
FurnishingLevelId = FurnishingLevelsCollection.Select(i=>i.Id).FirstOrDefault(),
BuildingFacadeId = BuildingFacadesCollection.Select(i=>i.Id).FirstOrDefault(),
ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "building-create-tab";
            
            
            await NewBuildingValidations.ClearAll();
            await CreateBuildingModal.Show();
        }

        private async Task CloseCreateBuildingModalAsync()
        {
            NewBuilding = new BuildingCreateDto{
                
                RegionId = RegionsCollection.Select(i=>i.Id).FirstOrDefault(),
FurnishingLevelId = FurnishingLevelsCollection.Select(i=>i.Id).FirstOrDefault(),
BuildingFacadeId = BuildingFacadesCollection.Select(i=>i.Id).FirstOrDefault(),
ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateBuildingModal.Hide();
        }

        private async Task OpenEditBuildingModalAsync(BuildingWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "building-edit-tab";
            
            
            var building = await BuildingsAppService.GetWithNavigationPropertiesAsync(input.Building.Id);
            
            EditingBuildingId = building.Building.Id;
            EditingBuilding = ObjectMapper.Map<BuildingDto, BuildingUpdateDto>(building.Building);
            SelectedMainAmenities = building.MainAmenities.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Name}).ToList();

            SelectedSecondaryAmenities = building.SecondaryAmenities.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Name}).ToList();

            
            await EditingBuildingValidations.ClearAll();
            await EditBuildingModal.Show();
        }

        private async Task DeleteBuildingAsync(BuildingWithNavigationPropertiesDto input)
        {
            await BuildingsAppService.DeleteAsync(input.Building.Id);
            await GetBuildingsAsync();
        }

        private async Task CreateBuildingAsync()
        {
            try
            {
                if (await NewBuildingValidations.ValidateAll() == false)
                {
                    return;
                }
                NewBuilding.MainAmenityIds = SelectedMainAmenities.Select(x => x.Id).ToList();

                NewBuilding.SecondaryAmenityIds = SelectedSecondaryAmenities.Select(x => x.Id).ToList();


                await BuildingsAppService.CreateAsync(NewBuilding);
                await GetBuildingsAsync();
                await CloseCreateBuildingModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditBuildingModalAsync()
        {
            await EditBuildingModal.Hide();
        }

        private async Task UpdateBuildingAsync()
        {
            try
            {
                if (await EditingBuildingValidations.ValidateAll() == false)
                {
                    return;
                }
                EditingBuilding.MainAmenityIds = SelectedMainAmenities.Select(x => x.Id).ToList();

                EditingBuilding.SecondaryAmenityIds = SelectedSecondaryAmenities.Select(x => x.Id).ToList();


                await BuildingsAppService.UpdateAsync(EditingBuildingId, EditingBuilding);
                await GetBuildingsAsync();
                await EditBuildingModal.Hide();                
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









        protected virtual async Task OnMainTitleChangedAsync(string? mainTitle)
        {
            Filter.MainTitle = mainTitle;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnPriceChangedAsync(string? price)
        {
            Filter.Price = price;
            await SearchAsync();
        }
        protected virtual async Task OnBuildingAreaChangedAsync(string? buildingArea)
        {
            Filter.BuildingArea = buildingArea;
            await SearchAsync();
        }
        protected virtual async Task OnNumberOfRoomsChangedAsync(string? numberOfRooms)
        {
            Filter.NumberOfRooms = numberOfRooms;
            await SearchAsync();
        }
        protected virtual async Task OnNumberOfBathsChangedAsync(string? numberOfBaths)
        {
            Filter.NumberOfBaths = numberOfBaths;
            await SearchAsync();
        }
        protected virtual async Task OnFloorNoChangedAsync(string? floorNo)
        {
            Filter.FloorNo = floorNo;
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
        protected virtual async Task OnRegionIdChangedAsync(Guid? regionId)
        {
            Filter.RegionId = regionId;
            await SearchAsync();
        }
        protected virtual async Task OnFurnishingLevelIdChangedAsync(Guid? furnishingLevelId)
        {
            Filter.FurnishingLevelId = furnishingLevelId;
            await SearchAsync();
        }
        protected virtual async Task OnBuildingFacadeIdChangedAsync(Guid? buildingFacadeId)
        {
            Filter.BuildingFacadeId = buildingFacadeId;
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
        protected virtual async Task OnMainAmenityIdChangedAsync(Guid? mainAmenityId)
        {
            Filter.MainAmenityId = mainAmenityId;
            await SearchAsync();
        }
        protected virtual async Task OnSecondaryAmenityIdChangedAsync(Guid? secondaryAmenityId)
        {
            Filter.SecondaryAmenityId = secondaryAmenityId;
            await SearchAsync();
        }
        

        private async Task GetRegionCollectionLookupAsync(string? newValue = null)
        {
            RegionsCollection = (await BuildingsAppService.GetRegionLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetFurnishingLevelCollectionLookupAsync(string? newValue = null)
        {
            FurnishingLevelsCollection = (await BuildingsAppService.GetFurnishingLevelLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetBuildingFacadeCollectionLookupAsync(string? newValue = null)
        {
            BuildingFacadesCollection = (await BuildingsAppService.GetBuildingFacadeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetServiceTypeCollectionLookupAsync(string? newValue = null)
        {
            ServiceTypesCollection = (await BuildingsAppService.GetServiceTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await BuildingsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetMainAmenityLookupAsync(string? newValue = null)
        {
            MainAmenities = (await BuildingsAppService.GetMainAmenityLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddMainAmenity()
        {
            if (SelectedMainAmenityId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedMainAmenities.Any(p => p.Id.ToString() == SelectedMainAmenityId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedMainAmenities.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedMainAmenityId),
                DisplayName = SelectedMainAmenityText
            });
        }

        private async Task GetSecondaryAmenityLookupAsync(string? newValue = null)
        {
            SecondaryAmenities = (await BuildingsAppService.GetSecondaryAmenityLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddSecondaryAmenity()
        {
            if (SelectedSecondaryAmenityId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedSecondaryAmenities.Any(p => p.Id.ToString() == SelectedSecondaryAmenityId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedSecondaryAmenities.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedSecondaryAmenityId),
                DisplayName = SelectedSecondaryAmenityText
            });
        }





        private Task SelectAllItems()
        {
            AllBuildingsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllBuildingsSelected = false;
            SelectedBuildings.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedBuildingRowsChanged()
        {
            if (SelectedBuildings.Count != PageSize)
            {
                AllBuildingsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedBuildingsAsync()
        {
            var message = AllBuildingsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedBuildings.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllBuildingsSelected)
            {
                await BuildingsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await BuildingsAppService.DeleteByIdsAsync(SelectedBuildings.Select(x => x.Building.Id).ToList());
            }

            SelectedBuildings.Clear();
            AllBuildingsSelected = false;

            await GetBuildingsAsync();
        }


    }
}
