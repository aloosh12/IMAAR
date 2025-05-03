using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Imaar.Categories;

namespace Imaar.Controllers.Categories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Category")]
    [Route("api/mobile/categories")]

    public class CategoryController : CategoryControllerBase, ICategoriesAppService
    {
        public CategoryController(ICategoriesAppService categoriesAppService) : base(categoriesAppService)
        {
        }
    }
}