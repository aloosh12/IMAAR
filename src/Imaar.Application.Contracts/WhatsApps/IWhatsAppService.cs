using Imaar.MobileResponses;
using Imaar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Imaar.WhatsApps
{
    public partial interface IWhatsAppService : IApplicationService
    {
        Task<MobileResponseDto> SendSecurityCodeAsync(string input);
        Task<MobileResponseDto> SendMessageAsync(WhatsAppMessageDto input);

    }
}
