using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;

namespace MISA.WEB05.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FoodCategorysController : BaseController<FoodCategory>
    {
        IFoodCategoryService _Service;
        IFoodCategoryRepository _Repository;

        public FoodCategorysController(IFoodCategoryService service, IFoodCategoryRepository repository):base(service, repository)
        {
            _Service = service;
            _Repository = repository;
        }
    }
}
