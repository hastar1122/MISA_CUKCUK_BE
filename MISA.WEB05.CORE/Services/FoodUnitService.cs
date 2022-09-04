using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Services
{
    public class FoodUnitService : BaseService<FoodUnit>, IFoodUnitService
    {
        IFoodUnitRepository _Repository;

        public FoodUnitService(IFoodUnitRepository repository) : base(repository)
        {
            _Repository = repository;
        }
    }
}
