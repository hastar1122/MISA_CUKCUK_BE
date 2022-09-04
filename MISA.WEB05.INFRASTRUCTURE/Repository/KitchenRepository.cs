using Microsoft.Extensions.Configuration;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.INFRASTRUCTURE.Repository
{
    public class KitchenRepository : BaseRepository<Kitchen>, IKitchenRepository
    {
        public KitchenRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
