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
using Imaar.Vacancies;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

using Imaar.Vacancies;



namespace Imaar.Blazor.Pages
{
    public partial class Vacancies
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<VacancyWithNavigationPropertiesDto> VacancyList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateVacancy { get; set; }
        private bool CanEditVacancy { get; set; }
        private bool CanDeleteVacancy { get; set; }
        private VacancyCreateDto NewVacancy { get; set; }
        private Validations NewVacancyValidations { get; set; } = new();
        private VacancyUpdateDto EditingVacancy { get; set; }
        private Validations EditingVacancyValidations { get; set; } = new();
        private Guid EditingVacancyId { get; set; }
        private Modal CreateVacancyModal { get; set; } = new();
        private Modal EditVacancyModal { get; set; } = new();
        private GetVacanciesInput Filter { get; set; }
        private DataGridEntityActionsColumn<VacancyWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "vacancy-create-tab";
        protected string SelectedEditTab = "vacancy-edit-tab";
        private VacancyWithNavigationPropertiesDto? SelectedVacancy;
        private IReadOnlyList<LookupDto<Guid>> ServiceTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> VacancyAdditionalFeatures { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedVacancyAdditionalFeatureId { get; set; }
        
        private string SelectedVacancyAdditionalFeatureText { get; set; }

        private Blazorise.Components.Autocomplete<LookupDto<Guid>, string> SelectedVacancyAdditionalFeatureAutoCompleteRef { get; set; } = new();

        private List<LookupDto<Guid>> SelectedVacancyAdditionalFeatures { get; set; } = new List<LookupDto<Guid>>();
        
        
        
        
        private List<VacancyWithNavigationPropertiesDto> SelectedVacancies { get; set; } = new();
        private bool AllVacanciesSelected { get; set; }
        
        public Vacancies()
        {
            NewVacancy = new VacancyCreateDto();
            EditingVacancy = new VacancyUpdateDto();
            Filter = new GetVacanciesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            VacancyList = new List<VacancyWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetServiceTypeCollectionLookupAsync();


            await GetUserProfileCollectionLookupAsync();


            await GetVacancyAdditionalFeatureLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Vacancies"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewVacancy"], async () =>
            {
                await OpenCreateVacancyModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Vacancies.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateVacancy = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Vacancies.Create);
            CanEditVacancy = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Vacancies.Edit);
            CanDeleteVacancy = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Vacancies.Delete);
                            
                            
        }

        private async Task GetVacanciesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await VacanciesAppService.GetListAsync(Filter);
            VacancyList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetVacanciesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await VacanciesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/vacancies/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Description={HttpUtility.UrlEncode(Filter.Description)}&Location={HttpUtility.UrlEncode(Filter.Location)}&Number={HttpUtility.UrlEncode(Filter.Number)}&Latitude={HttpUtility.UrlEncode(Filter.Latitude)}&Longitude={HttpUtility.UrlEncode(Filter.Longitude)}&DateOfPublishMin={Filter.DateOfPublishMin}&DateOfPublishMax={Filter.DateOfPublishMax}&ExpectedExperience={HttpUtility.UrlEncode(Filter.ExpectedExperience)}&EducationLevel={HttpUtility.UrlEncode(Filter.EducationLevel)}&WorkSchedule={HttpUtility.UrlEncode(Filter.WorkSchedule)}&EmploymentType={HttpUtility.UrlEncode(Filter.EmploymentType)}&BiologicalSex={Filter.BiologicalSex}&Languages={HttpUtility.UrlEncode(Filter.Languages)}&DriveLicense={HttpUtility.UrlEncode(Filter.DriveLicense)}&Salary={HttpUtility.UrlEncode(Filter.Salary)}&ViewCounterMin={Filter.ViewCounterMin}&ViewCounterMax={Filter.ViewCounterMax}&OrderCounterMin={Filter.OrderCounterMin}&OrderCounterMax={Filter.OrderCounterMax}&ServiceTypeId={Filter.ServiceTypeId}&UserProfileId={Filter.UserProfileId}&VacancyAdditionalFeatureId={Filter.VacancyAdditionalFeatureId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<VacancyWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetVacanciesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateVacancyModalAsync()
        {
            SelectedVacancyAdditionalFeatures = new List<LookupDto<Guid>>();
            SelectedVacancyAdditionalFeatureId = string.Empty;
            SelectedVacancyAdditionalFeatureText = string.Empty;

            await SelectedVacancyAdditionalFeatureAutoCompleteRef.Clear();

            NewVacancy = new VacancyCreateDto{
                DateOfPublish = DateOnly.FromDateTime(DateTime.Now),

                ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "vacancy-create-tab";
            
            
            await NewVacancyValidations.ClearAll();
            await CreateVacancyModal.Show();
        }

        private async Task CloseCreateVacancyModalAsync()
        {
            NewVacancy = new VacancyCreateDto{
                DateOfPublish = DateOnly.FromDateTime(DateTime.Now),

                ServiceTypeId = ServiceTypesCollection.Select(i=>i.Id).FirstOrDefault(),
UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateVacancyModal.Hide();
        }

        private async Task OpenEditVacancyModalAsync(VacancyWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "vacancy-edit-tab";
            
            
            var vacancy = await VacanciesAppService.GetWithNavigationPropertiesAsync(input.Vacancy.Id);
            
            EditingVacancyId = vacancy.Vacancy.Id;
            EditingVacancy = ObjectMapper.Map<VacancyDto, VacancyUpdateDto>(vacancy.Vacancy);
            SelectedVacancyAdditionalFeatures = vacancy.VacancyAdditionalFeatures.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Name}).ToList();

            
            await EditingVacancyValidations.ClearAll();
            await EditVacancyModal.Show();
        }

        private async Task DeleteVacancyAsync(VacancyWithNavigationPropertiesDto input)
        {
            await VacanciesAppService.DeleteAsync(input.Vacancy.Id);
            await GetVacanciesAsync();
        }

        private async Task CreateVacancyAsync()
        {
            try
            {
                if (await NewVacancyValidations.ValidateAll() == false)
                {
                    return;
                }
                NewVacancy.VacancyAdditionalFeatureIds = SelectedVacancyAdditionalFeatures.Select(x => x.Id).ToList();


                await VacanciesAppService.CreateAsync(NewVacancy);
                await GetVacanciesAsync();
                await CloseCreateVacancyModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditVacancyModalAsync()
        {
            await EditVacancyModal.Hide();
        }

        private async Task UpdateVacancyAsync()
        {
            try
            {
                if (await EditingVacancyValidations.ValidateAll() == false)
                {
                    return;
                }
                EditingVacancy.VacancyAdditionalFeatureIds = SelectedVacancyAdditionalFeatures.Select(x => x.Id).ToList();


                await VacanciesAppService.UpdateAsync(EditingVacancyId, EditingVacancy);
                await GetVacanciesAsync();
                await EditVacancyModal.Hide();                
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
        protected virtual async Task OnLocationChangedAsync(string? location)
        {
            Filter.Location = location;
            await SearchAsync();
        }
        protected virtual async Task OnNumberChangedAsync(string? number)
        {
            Filter.Number = number;
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
        protected virtual async Task OnExpectedExperienceChangedAsync(string? expectedExperience)
        {
            Filter.ExpectedExperience = expectedExperience;
            await SearchAsync();
        }
        protected virtual async Task OnEducationLevelChangedAsync(string? educationLevel)
        {
            Filter.EducationLevel = educationLevel;
            await SearchAsync();
        }
        protected virtual async Task OnWorkScheduleChangedAsync(string? workSchedule)
        {
            Filter.WorkSchedule = workSchedule;
            await SearchAsync();
        }
        protected virtual async Task OnEmploymentTypeChangedAsync(string? employmentType)
        {
            Filter.EmploymentType = employmentType;
            await SearchAsync();
        }
        protected virtual async Task OnBiologicalSexChangedAsync(BiologicalSex? biologicalSex)
        {
            Filter.BiologicalSex = biologicalSex;
            await SearchAsync();
        }
        protected virtual async Task OnLanguagesChangedAsync(string? languages)
        {
            Filter.Languages = languages;
            await SearchAsync();
        }
        protected virtual async Task OnDriveLicenseChangedAsync(string? driveLicense)
        {
            Filter.DriveLicense = driveLicense;
            await SearchAsync();
        }
        protected virtual async Task OnSalaryChangedAsync(string? salary)
        {
            Filter.Salary = salary;
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
        protected virtual async Task OnVacancyAdditionalFeatureIdChangedAsync(Guid? vacancyAdditionalFeatureId)
        {
            Filter.VacancyAdditionalFeatureId = vacancyAdditionalFeatureId;
            await SearchAsync();
        }
        

        private async Task GetServiceTypeCollectionLookupAsync(string? newValue = null)
        {
            ServiceTypesCollection = (await VacanciesAppService.GetServiceTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await VacanciesAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetVacancyAdditionalFeatureLookupAsync(string? newValue = null)
        {
            VacancyAdditionalFeatures = (await VacanciesAppService.GetVacancyAdditionalFeatureLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddVacancyAdditionalFeature()
        {
            if (SelectedVacancyAdditionalFeatureId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedVacancyAdditionalFeatures.Any(p => p.Id.ToString() == SelectedVacancyAdditionalFeatureId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedVacancyAdditionalFeatures.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedVacancyAdditionalFeatureId),
                DisplayName = SelectedVacancyAdditionalFeatureText
            });
        }





        private Task SelectAllItems()
        {
            AllVacanciesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllVacanciesSelected = false;
            SelectedVacancies.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedVacancyRowsChanged()
        {
            if (SelectedVacancies.Count != PageSize)
            {
                AllVacanciesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedVacanciesAsync()
        {
            var message = AllVacanciesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedVacancies.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllVacanciesSelected)
            {
                await VacanciesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await VacanciesAppService.DeleteByIdsAsync(SelectedVacancies.Select(x => x.Vacancy.Id).ToList());
            }

            SelectedVacancies.Clear();
            AllVacanciesSelected = false;

            await GetVacanciesAsync();
        }


    }
}
