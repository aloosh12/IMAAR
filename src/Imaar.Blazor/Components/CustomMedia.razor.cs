using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using System.IO;
using Volo.Abp;
using Volo.Abp.Guids;
using Volo.Abp.BlobStoring;
using Imaar.Medias;

namespace Imaar.Blazor.Components
{
    public partial class CustomMedia
    {
        [Parameter]
        public MediaDto Media { get; set; }

        [Parameter]
        public EventCallback<MediaDto> MediaDeleted { get; set; }

        [Inject]
        public IUiMessageService uiMessageService { get; set; }

        [Inject]
        public IBlobContainer<MediaContainer> MediaContainer { get; set; }

        [Inject]
        protected IGuidGenerator GuidGenerator { get; set; }
        private Validations MediaValidations { get; set; } = new();


        public CustomMedia()
        {

        }

        protected override async Task OnInitializedAsync()
        {

        }

        private async Task deleteMediaAsync(MediaDto input)
        {
            var confirm = await uiMessageService.Confirm(L["DeleteConfirmationMessage"]);

            if (confirm)
            {
                await MediaDeleted.InvokeAsync(input);
            }
        }
        private async Task RemoveMedia()
        {
            Media.File = null;
        }
        public async Task FileChanged(FileChangedEventArgs e)
        {
            try
            {
                if (e.Files.Count() == 0)
                {
                    Media.File = null;
                }

            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        public async Task OnFileUpload(FileUploadEventArgs e)
        {
            try
            {
                using (MemoryStream result = new MemoryStream())
                {
                    await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
                    Media.FileContent = await result.GetAllBytesAsync();
                   // Media.File = $"{Path.GetFileNameWithoutExtension(e.File.Name)}_{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";
                    Media.File = $"{GuidGenerator.Create().ToString("N")}{Path.GetExtension(e.File.Name)}";

                    Media.FileNewUpload = true;
                }
            }
            catch (UserFriendlyException ex)
            {
                await HandleErrorAsync(ex);
            }
        }

    }
}
