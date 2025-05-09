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
using Imaar.Categories;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using Imaar.ServiceTypes; 



namespace Imaar.Blazor.Pages
{
    public partial class Categories
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<CategoryDto> CategoryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCategory { get; set; }
        private bool CanEditCategory { get; set; }
        private bool CanDeleteCategory { get; set; }
        private CategoryCreateDto NewCategory { get; set; }
        private Validations NewCategoryValidations { get; set; } = new();
        private CategoryUpdateDto EditingCategory { get; set; }
        private Validations EditingCategoryValidations { get; set; } = new();
        private Guid EditingCategoryId { get; set; }
        private Modal CreateCategoryModal { get; set; } = new();
        private Modal EditCategoryModal { get; set; } = new();
        private GetCategoriesInput Filter { get; set; }
        private DataGridEntityActionsColumn<CategoryDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "category-create-tab";
        protected string SelectedEditTab = "category-edit-tab";
        private CategoryDto? SelectedCategory;
        
        
                #region Child Entities
        
                #region ServiceTypes

                private bool CanListServiceType { get; set; }
                private bool CanCreateServiceType { get; set; }
                private bool CanEditServiceType { get; set; }
                private bool CanDeleteServiceType { get; set; }
                private ServiceTypeCreateDto NewServiceType { get; set; }
                private Dictionary<Guid, DataGrid<ServiceTypeDto>> ServiceTypeDataGrids { get; set; } = new();
                private int ServiceTypePageSize { get; } = 5;
                private DataGridEntityActionsColumn<ServiceTypeDto> ServiceTypeEntityActionsColumns { get; set; } = new();
                private Validations NewServiceTypeValidations { get; set; } = new();
                private Modal CreateServiceTypeModal { get; set; } = new();
                private Guid EditingServiceTypeId { get; set; }
                private ServiceTypeUpdateDto EditingServiceType { get; set; }
                private Validations EditingServiceTypeValidations { get; set; } = new();
                private Modal EditServiceTypeModal { get; set; } = new();
    
            
                #endregion
        
        #endregion
        
        
        private List<CategoryDto> SelectedCategories { get; set; } = new();
        private bool AllCategoriesSelected { get; set; }
        
        public Categories()
        {
            NewCategory = new CategoryCreateDto();
            EditingCategory = new CategoryUpdateDto();
            Filter = new GetCategoriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CategoryList = new List<CategoryDto>();
            
            NewServiceType = new ServiceTypeCreateDto();
EditingServiceType = new ServiceTypeUpdateDto();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Categories"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCategory"], async () =>
            {
                await OpenCreateCategoryModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Categories.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCategory = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Categories.Create);
            CanEditCategory = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Categories.Edit);
            CanDeleteCategory = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Categories.Delete);
                            
            
            #region ServiceTypes
            CanListServiceType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTypes.Default);
            CanCreateServiceType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTypes.Create);
            CanEditServiceType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTypes.Edit);
            CanDeleteServiceType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.ServiceTypes.Delete);
            #endregion                
        }

        private async Task GetCategoriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CategoriesAppService.GetListAsync(Filter);
            CategoryList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await CategoriesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/categories/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CategoryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCategoriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCategoryModalAsync()
        {
            NewCategory = new CategoryCreateDto{
                
                
            };

            SelectedCreateTab = "category-create-tab";
            
            
            await NewCategoryValidations.ClearAll();
            await CreateCategoryModal.Show();
        }

        private async Task CloseCreateCategoryModalAsync()
        {
            NewCategory = new CategoryCreateDto{
                
                
            };
            await CreateCategoryModal.Hide();
        }

        private async Task OpenEditCategoryModalAsync(CategoryDto input)
        {
            SelectedEditTab = "category-edit-tab";
            
            
            var category = await CategoriesAppService.GetAsync(input.Id);
            
            EditingCategoryId = category.Id;
            EditingCategory = ObjectMapper.Map<CategoryDto, CategoryUpdateDto>(category);
            
            await EditingCategoryValidations.ClearAll();
            await EditCategoryModal.Show();
        }

        private async Task DeleteCategoryAsync(CategoryDto input)
        {
            await CategoriesAppService.DeleteAsync(input.Id);
            await GetCategoriesAsync();
        }

        private async Task CreateCategoryAsync()
        {
            try
            {
                if (await NewCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.CreateAsync(NewCategory);
                await GetCategoriesAsync();
                await CloseCreateCategoryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCategoryModalAsync()
        {
            await EditCategoryModal.Hide();
        }

        private async Task UpdateCategoryAsync()
        {
            try
            {
                if (await EditingCategoryValidations.ValidateAll() == false)
                {
                    return;
                }

                await CategoriesAppService.UpdateAsync(EditingCategoryId, EditingCategory);
                await GetCategoriesAsync();
                await EditCategoryModal.Hide();                
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
        


    private bool ShouldShowDetailRow()
    {
        return CanListServiceType;
    }
    
    public string SelectedChildTab { get; set; } = "servicetype-tab";
        
    private Task OnSelectedChildTabChanged(string name)
    {
        SelectedChildTab = name;
    
        return Task.CompletedTask;
    }


        private Task SelectAllItems()
        {
            AllCategoriesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllCategoriesSelected = false;
            SelectedCategories.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedCategoryRowsChanged()
        {
            if (SelectedCategories.Count != PageSize)
            {
                AllCategoriesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedCategoriesAsync()
        {
            var message = AllCategoriesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedCategories.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllCategoriesSelected)
            {
                await CategoriesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await CategoriesAppService.DeleteByIdsAsync(SelectedCategories.Select(x => x.Id).ToList());
            }

            SelectedCategories.Clear();
            AllCategoriesSelected = false;

            await GetCategoriesAsync();
        }


        #region ServiceTypes
        
        private async Task OnServiceTypeDataGridReadAsync(DataGridReadDataEventArgs<ServiceTypeDto> e, Guid categoryId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetServiceTypesAsync(categoryId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SetServiceTypesAsync(Guid categoryId, int currentPage = 1, string? sorting = null)
        {
            var category = CategoryList.FirstOrDefault(x => x.Id == categoryId);
            if(category == null)
            {
                return;
            }

            var serviceTypes = await ServiceTypesAppService.GetListByCategoryIdAsync(new GetServiceTypeListInput 
            {
                CategoryId = categoryId,
                MaxResultCount = ServiceTypePageSize,
                SkipCount = (currentPage - 1) * ServiceTypePageSize,
                Sorting = sorting
            });

            category.ServiceTypes = serviceTypes.Items.ToList();

            var serviceTypeDataGrid = ServiceTypeDataGrids[categoryId];
            
            serviceTypeDataGrid.CurrentPage = currentPage;
            serviceTypeDataGrid.TotalItems = (int)serviceTypes.TotalCount;
        }
        
        private async Task OpenEditServiceTypeModalAsync(ServiceTypeDto input)
        {
            
            
            var serviceType = await ServiceTypesAppService.GetAsync(input.Id);

            EditingServiceTypeId = serviceType.Id;
            EditingServiceType = ObjectMapper.Map<ServiceTypeDto, ServiceTypeUpdateDto>(serviceType);
            
            
            await EditingServiceTypeValidations.ClearAll();
            await EditServiceTypeModal.Show();
        }
        
        private async Task CloseEditServiceTypeModalAsync()
        {
            await EditServiceTypeModal.Hide();
        }
        
        private async Task UpdateServiceTypeAsync()
        {
            try
            {
                if (await EditingServiceTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTypesAppService.UpdateAsync(EditingServiceTypeId, EditingServiceType);
                await SetServiceTypesAsync(EditingServiceType.CategoryId);
                await EditServiceTypeModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task DeleteServiceTypeAsync(ServiceTypeDto input)
        {
            await ServiceTypesAppService.DeleteAsync(input.Id);
            await SetServiceTypesAsync(input.CategoryId);
        }
        
        private async Task OpenCreateServiceTypeModalAsync(Guid categoryId)
        {
            NewServiceType = new ServiceTypeCreateDto
            {
                CategoryId = categoryId
            };
            
            
            await NewServiceTypeValidations.ClearAll();
            await CreateServiceTypeModal.Show();
        }
        
        private async Task CloseCreateServiceTypeModalAsync()
        {
            NewServiceType = new ServiceTypeCreateDto();

            await CreateServiceTypeModal.Hide();
        }
        
        private async Task CreateServiceTypeAsync()
        {
            try
            {
                if (await NewServiceTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await ServiceTypesAppService.CreateAsync(NewServiceType);
                await SetServiceTypesAsync(NewServiceType.CategoryId);
                await CloseCreateServiceTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        
        
        
        
        #endregion
    }
}
