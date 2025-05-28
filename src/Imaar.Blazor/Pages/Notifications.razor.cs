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
using Imaar.Notifications;
using Imaar.Permissions;
using Imaar.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

using Imaar.Notifications;



namespace Imaar.Blazor.Pages
{
    public partial class Notifications
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<NotificationWithNavigationPropertiesDto> NotificationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateNotification { get; set; }
        private bool CanEditNotification { get; set; }
        private bool CanDeleteNotification { get; set; }
        private NotificationCreateDto NewNotification { get; set; }
        private Validations NewNotificationValidations { get; set; } = new();
        private NotificationUpdateDto EditingNotification { get; set; }
        private Validations EditingNotificationValidations { get; set; } = new();
        private Guid EditingNotificationId { get; set; }
        private Modal CreateNotificationModal { get; set; } = new();
        private Modal EditNotificationModal { get; set; } = new();
        private GetNotificationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<NotificationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "notification-create-tab";
        protected string SelectedEditTab = "notification-edit-tab";
        private NotificationWithNavigationPropertiesDto? SelectedNotification;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> NotificationTypesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<NotificationWithNavigationPropertiesDto> SelectedNotifications { get; set; } = new();
        private bool AllNotificationsSelected { get; set; }
        
        public Notifications()
        {
            NewNotification = new NotificationCreateDto();
            EditingNotification = new NotificationUpdateDto();
            Filter = new GetNotificationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            NotificationList = new List<NotificationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            await GetNotificationTypeCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Notifications"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewNotification"], async () =>
            {
                await OpenCreateNotificationModalAsync();
            }, IconName.Add, requiredPolicyName: ImaarPermissions.Notifications.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateNotification = await AuthorizationService
                .IsGrantedAsync(ImaarPermissions.Notifications.Create);
            CanEditNotification = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Notifications.Edit);
            CanDeleteNotification = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Notifications.Delete);
                            
                            
        }

        private async Task GetNotificationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await NotificationsAppService.GetListAsync(Filter);
            NotificationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetNotificationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await NotificationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Imaar") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/notifications/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Message={HttpUtility.UrlEncode(Filter.Message)}&IsRead={Filter.IsRead}&ReadDateMin={Filter.ReadDateMin?.ToString("O")}&ReadDateMax={Filter.ReadDateMax?.ToString("O")}&PriorityMin={Filter.PriorityMin}&PriorityMax={Filter.PriorityMax}&SourceEntityType={Filter.SourceEntityType}&SourceEntityId={HttpUtility.UrlEncode(Filter.SourceEntityId)}&SenderUserId={HttpUtility.UrlEncode(Filter.SenderUserId)}&UserProfileId={Filter.UserProfileId}&NotificationTypeId={Filter.NotificationTypeId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<NotificationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetNotificationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateNotificationModalAsync()
        {
            NewNotification = new NotificationCreateDto{
                ReadDate = DateTime.Now,

                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
NotificationTypeId = NotificationTypesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "notification-create-tab";
            
            
            await NewNotificationValidations.ClearAll();
            await CreateNotificationModal.Show();
        }

        private async Task CloseCreateNotificationModalAsync()
        {
            NewNotification = new NotificationCreateDto{
                ReadDate = DateTime.Now,

                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
NotificationTypeId = NotificationTypesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateNotificationModal.Hide();
        }

        private async Task OpenEditNotificationModalAsync(NotificationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "notification-edit-tab";
            
            
            var notification = await NotificationsAppService.GetWithNavigationPropertiesAsync(input.Notification.Id);
            
            EditingNotificationId = notification.Notification.Id;
            EditingNotification = ObjectMapper.Map<NotificationDto, NotificationUpdateDto>(notification.Notification);
            
            await EditingNotificationValidations.ClearAll();
            await EditNotificationModal.Show();
        }

        private async Task DeleteNotificationAsync(NotificationWithNavigationPropertiesDto input)
        {
            await NotificationsAppService.DeleteAsync(input.Notification.Id);
            await GetNotificationsAsync();
        }

        private async Task CreateNotificationAsync()
        {
            try
            {
                if (await NewNotificationValidations.ValidateAll() == false)
                {
                    return;
                }

                await NotificationsAppService.CreateAsync(NewNotification);
                await GetNotificationsAsync();
                await CloseCreateNotificationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditNotificationModalAsync()
        {
            await EditNotificationModal.Hide();
        }

        private async Task UpdateNotificationAsync()
        {
            try
            {
                if (await EditingNotificationValidations.ValidateAll() == false)
                {
                    return;
                }

                await NotificationsAppService.UpdateAsync(EditingNotificationId, EditingNotification);
                await GetNotificationsAsync();
                await EditNotificationModal.Hide();                
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
        protected virtual async Task OnMessageChangedAsync(string? message)
        {
            Filter.Message = message;
            await SearchAsync();
        }
        protected virtual async Task OnIsReadChangedAsync(bool? isRead)
        {
            Filter.IsRead = isRead;
            await SearchAsync();
        }
        protected virtual async Task OnReadDateMinChangedAsync(DateTime? readDateMin)
        {
            Filter.ReadDateMin = readDateMin.HasValue ? readDateMin.Value.Date : readDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnReadDateMaxChangedAsync(DateTime? readDateMax)
        {
            Filter.ReadDateMax = readDateMax.HasValue ? readDateMax.Value.Date.AddDays(1).AddSeconds(-1) : readDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnPriorityMinChangedAsync(int? priorityMin)
        {
            Filter.PriorityMin = priorityMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriorityMaxChangedAsync(int? priorityMax)
        {
            Filter.PriorityMax = priorityMax;
            await SearchAsync();
        }
        protected virtual async Task OnSourceEntityTypeChangedAsync(SourceEntityType? sourceEntityType)
        {
            Filter.SourceEntityType = sourceEntityType;
            await SearchAsync();
        }
        protected virtual async Task OnSourceEntityIdChangedAsync(string? sourceEntityId)
        {
            Filter.SourceEntityId = sourceEntityId;
            await SearchAsync();
        }
        protected virtual async Task OnSenderUserIdChangedAsync(string? senderUserId)
        {
            Filter.SenderUserId = senderUserId;
            await SearchAsync();
        }
        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        protected virtual async Task OnNotificationTypeIdChangedAsync(Guid? notificationTypeId)
        {
            Filter.NotificationTypeId = notificationTypeId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await NotificationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetNotificationTypeCollectionLookupAsync(string? newValue = null)
        {
            NotificationTypesCollection = (await NotificationsAppService.GetNotificationTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllNotificationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllNotificationsSelected = false;
            SelectedNotifications.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedNotificationRowsChanged()
        {
            if (SelectedNotifications.Count != PageSize)
            {
                AllNotificationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedNotificationsAsync()
        {
            var message = AllNotificationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedNotifications.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllNotificationsSelected)
            {
                await NotificationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await NotificationsAppService.DeleteByIdsAsync(SelectedNotifications.Select(x => x.Notification.Id).ToList());
            }

            SelectedNotifications.Clear();
            AllNotificationsSelected = false;

            await GetNotificationsAsync();
        }


    }
}
