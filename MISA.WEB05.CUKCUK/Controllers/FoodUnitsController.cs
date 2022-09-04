using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;

namespace MISA.WEB05.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FoodUnitsController : BaseController<FoodUnit>
    {
        IFoodUnitService _Service;
        IFoodUnitRepository _Repository;

        public FoodUnitsController(IFoodUnitService service, IFoodUnitRepository repository) : base(service, repository)
        {
            _Service = service;
            _Repository = repository;
        }
    }
}
