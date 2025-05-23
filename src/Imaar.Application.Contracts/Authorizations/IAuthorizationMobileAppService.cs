﻿using Imaar.MobileResponses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Imaar.Authorizations
{
    public interface IAuthorizationMobileAppService : IApplicationService
    {
        Task<MobileResponseDto> GetAbpLogInAsync(TokenRequest request);
    }
}
