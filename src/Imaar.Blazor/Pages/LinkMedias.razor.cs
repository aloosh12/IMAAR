using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Guids;
using Volo.Abp.Application.Dtos;
using Imaar.Medias;
using Microsoft.Extensions.Logging;
using Imaar.Permissions;
using Imaar.Helper;


namespace Imaar.Blazor.Pages
{
    public partial class LinkMedias
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string Name { get; set; }
        [Parameter]
        public MediaEntityType MediaEntityType { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private bool CanEditZoneParagraph { get; set; }

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }

        private Guid EntityId { get; set; }
   
        [Inject]
        public IMediasAppService MediasAppService { get; set; }
        
        private List<MediaDto> MediaDtos { get; set; }
        private string SelectedTab { get; set; }
        public LinkMedias()
        {
           
        }

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();

            if (!Id.IsNullOrEmpty())
            {
                try
                {
                    EntityId = Guid.Parse(Id);

                    GetMediasInput getMediasInput = new GetMediasInput();
                    getMediasInput.SkipCount= 0;
                    getMediasInput.MaxResultCount = 1000;
                    getMediasInput.SourceEntityType = MediaEntityType;
                    getMediasInput.SourceEntityId = Id;

                    PagedResultDto<MediaDto> mediaDtos = await MediasAppService.GetListAsync(getMediasInput);
                    if (mediaDtos != null && mediaDtos.TotalCount > 0)
                    {
                        MediaDtos = mediaDtos.Items.ToList();
                        SelectedTab = "1";
                    }

                   // await SetNewAsync();
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ZoneParagraphs"],
                url: $"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}"));
 
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["LinkMedias"]));
            return ValueTask.CompletedTask;
        }
        private async Task SetPermissionsAsync()
        {
            CanEditZoneParagraph = await AuthorizationService
                            .IsGrantedAsync(ImaarPermissions.Medias.Edit);

            if (!CanEditZoneParagraph)
                throw new UnauthorizedAccessException();

        }

        protected async Task addNewMediaAsync()
        {
            var data = new MediaDto();
            data.Id = GuidGenerator.Create();
            data.SourceEntityType = MediaEntityType;
            

            if (!Id.IsNullOrEmpty())
                data.SourceEntityId = Id;
            data.IsActive = true;
            if (MediaDtos.IsNullOrEmpty())
                MediaDtos = new List<MediaDto>();
            data.Order = MediaDtos.Count + 1;
            data.Title = $"Media {MediaDtos.Count+1}" ;

            SelectedTab = (MediaDtos.Count + 1).ToString();
            MediaDtos.Add(data);
        }
        protected void MediaDeleted(MediaDto input)
        {
            MediaDtos.Remove(input);
            if(!MediaDtos.IsNullOrEmpty() && SelectedTab != null && SelectedTab.Trim() != "1")
                SelectedTab =  (Int32.Parse(SelectedTab) - 1).ToString();
            else
                if(SelectedTab != null && SelectedTab.Trim() == "1")
                    SelectedTab = "1";
        }

        private async Task UpdatePageSectionMediasAsync()
        {
            try
            {
                if (await ValidateMedia() == false)
                {
                    return;
                }

                MediaBulkUpdateDto mediaBulkUpdateDto = new MediaBulkUpdateDto();
                mediaBulkUpdateDto.SourceEntityId = Id;
                mediaBulkUpdateDto.SourceEntityType = MediaEntityType;
                mediaBulkUpdateDto.Medias = MediaDtos;
                await MediasAppService.BulkUpdateMediasAsync(mediaBulkUpdateDto);
                await uiMessageService.Success(L["Message:SuccessfullyUpdated"]);
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task<bool> ValidateMedia()
        {
            foreach (var (item, index) in MediaDtos.WithIndex())
            {
                if(item.File.IsNullOrEmpty())
                {
                    if(!item.Title.IsNullOrEmpty())
                        await uiMessageService.Error( $"{L["MediaFileValidation"]} ({item.Title})");
                    else
                        await uiMessageService.Error($"{L["MediaFileValidation"]} (Media {index + 1})");
                    return false;
                }
            }
            return true;
        }
        private async Task Cancel()
        {
            var confirm = await uiMessageService.Confirm(L["ReturnBackConfirmationMessage"]);

            if (confirm)
            {
                NavigationManager.NavigateTo($"{String.Join("/", NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('/'), 0, 2) ?? ""}");
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedTab = name;
        }
    }
}
