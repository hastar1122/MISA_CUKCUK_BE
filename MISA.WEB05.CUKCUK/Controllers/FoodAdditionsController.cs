using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;

namespace MISA.WEB05.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FoodAdditionsController : BaseController<FoodAddition>
    {
        #region Properties
        IFoodAdditionService _Service;
        IFoodAdditionRepository _Repository;
        #endregion

        #region Constructor
        public FoodAdditionsController(IFoodAdditionService service, IFoodAdditionRepository repository):base(service, repository)
        {
            _Service = service;
            _Repository = repository;
        }
        #endregion

        #region Methods

        #endregion
    }
}
