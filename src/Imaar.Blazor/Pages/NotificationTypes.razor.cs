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
using Imaar.NotificationTypes;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace Imaar.Blazor.Pages
{
    public partial class NotificationTypes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<NotificationTypeDto> NotificationTypeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateNotificationType { get; set; }
        private bool CanEditNotificationType { get; set; }
        private bool CanDeleteNotificationType { get; set; }
        private NotificationTypeCreateDto NewNotificationType { get; set; }
        private Validations NewNotificationTypeValidations { get; set; } = new();
        private NotificationTypeUpdateDto EditingNotificationType { get; set; }
        private Validations EditingNotificationTypeValidations { get; set; } = new();
        private Guid EditingNotificationTypeId { get; set; }
        private Modal CreateNotificationTypeModal { get; set; } = new();
        private Modal EditNotificationTypeModal { get; set; } = new();
        private GetNotificationTypesInput Filter { get; set; }
        private DataGridEntityActionsColumn<NotificationTypeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "notificationType-create-tab";
        protected string SelectedEditTab = "notificationType-edit-tab";
        private NotificationTypeDto? SelectedNotificationType;
        
        
        
        
        
        private List<NotificationTypeDto> SelectedNotificationTypes { get; set; } = new();
        private bool AllNotificationTypesSelected { get; set; }
        
        public NotificationTypes()
        {
            NewNotificationType = new NotificationTypeCreateDto();
            EditingNotificationType = new NotificationTypeUpdateDto();
            Filter = new GetNotificationTypesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            NotificationTypeList = new List<NotificationTypeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["NotificationTypes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewNotificationType"], async () =>
            {
                await OpenCreateNotificationTypeModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.NotificationTypes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateNotificationType = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.NotificationTypes.Create);
            CanEditNotificationType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.NotificationTypes.Edit);
            CanDeleteNotificationType = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.NotificationTypes.Delete);
                            
                            
        }

        private async Task GetNotificationTypesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await NotificationTypesAppService.GetListAsync(Filter);
            NotificationTypeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetNotificationTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await NotificationTypesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/notification-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<NotificationTypeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetNotificationTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateNotificationTypeModalAsync()
        {
            NewNotificationType = new NotificationTypeCreateDto{
                
                
            };

            SelectedCreateTab = "notificationType-create-tab";
            
            
            await NewNotificationTypeValidations.ClearAll();
            await CreateNotificationTypeModal.Show();
        }

        private async Task CloseCreateNotificationTypeModalAsync()
        {
            NewNotificationType = new NotificationTypeCreateDto{
                
                
            };
            await CreateNotificationTypeModal.Hide();
        }

        private async Task OpenEditNotificationTypeModalAsync(NotificationTypeDto input)
        {
            SelectedEditTab = "notificationType-edit-tab";
            
            
            var notificationType = await NotificationTypesAppService.GetAsync(input.Id);
            
            EditingNotificationTypeId = notificationType.Id;
            EditingNotificationType = ObjectMapper.Map<NotificationTypeDto, NotificationTypeUpdateDto>(notificationType);
            
            await EditingNotificationTypeValidations.ClearAll();
            await EditNotificationTypeModal.Show();
        }

        private async Task DeleteNotificationTypeAsync(NotificationTypeDto input)
        {
            await NotificationTypesAppService.DeleteAsync(input.Id);
            await GetNotificationTypesAsync();
        }

        private async Task CreateNotificationTypeAsync()
        {
            try
            {
                if (await NewNotificationTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await NotificationTypesAppService.CreateAsync(NewNotificationType);
                await GetNotificationTypesAsync();
                await CloseCreateNotificationTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditNotificationTypeModalAsync()
        {
            await EditNotificationTypeModal.Hide();
        }

        private async Task UpdateNotificationTypeAsync()
        {
            try
            {
                if (await EditingNotificationTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await NotificationTypesAppService.UpdateAsync(EditingNotificationTypeId, EditingNotificationType);
                await GetNotificationTypesAsync();
                await EditNotificationTypeModal.Hide();                
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
        





        private Task SelectAllItems()
        {
            AllNotificationTypesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllNotificationTypesSelected = false;
            SelectedNotificationTypes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedNotificationTypeRowsChanged()
        {
            if (SelectedNotificationTypes.Count != PageSize)
            {
                AllNotificationTypesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedNotificationTypesAsync()
        {
            var message = AllNotificationTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedNotificationTypes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllNotificationTypesSelected)
            {
                await NotificationTypesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await NotificationTypesAppService.DeleteByIdsAsync(SelectedNotificationTypes.Select(x => x.Id).ToList());
            }

            SelectedNotificationTypes.Clear();
            AllNotificationTypesSelected = false;

            await GetNotificationTypesAsync();
        }


    }
}
