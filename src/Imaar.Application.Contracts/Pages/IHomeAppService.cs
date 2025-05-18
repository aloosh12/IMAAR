using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Imaar.Shared;
using Imaar.Pages;

namespace Imaar.Categories
{
    public interface IHomeAppService : IApplicationService
    {

        Task<HomePageDto> GetAsync();
    }
}