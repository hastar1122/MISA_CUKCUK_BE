using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Repository
{
    /// <summary>
    /// Interface sở thích phục vụ
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public interface IFoodAdditionRepository: IBaseRepository<FoodAddition>
    {
        /// <summary>
        /// Hàm lấy danh sách sở thích phục vụ theo thực đơn
        /// </summary>
        /// <param name="foodID">Khóa chính thực đơn</param>
        /// <returns>Danh sách sở thích phục vụ</returns>
        /// Created by: NHANH (23/8/2022)
        public IEnumerable<FoodAddition> GetDataByFoodId(Guid foodID);

        /// <summary>
        /// Hàm kiểm tra sở thích phục vụ đã tồn tại chưa
        /// </summary>
        /// <param name="foodAddition">Sở thích phục vụ</param>
        /// <param name="ID">Khóa ID </param>
        /// <returns>
        /// true => đã tồn tại
        /// false => chưa có
        /// </returns>
        /// Created by: NHANH (27/8/2022)
        public bool CheckDuplicate(FoodAddition foodAddition, Guid? ID);
    }
}
