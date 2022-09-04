using MISA.WEB05.CORE.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Repository
{
    /// <summary>
    /// Interface repository chi tiết thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public interface IFoodDetailRepository: IBaseRepository<FoodDetail>
    {
        /// <summary>
        /// Thêm mới danh sách chi tiết thực đơn
        /// </summary>
        /// <param name="foodId">Khóa chính của thực đơn được thêm mới</param>
        /// <param name="foodAdditionIDs">Danh sách khóa chính của sở thích phục vụ</param>
        /// <param name="mySqlTransaction"></param>
        /// <returns>Số cột được thêm mới</returns>
        public int InsertMultiple(IEnumerable<FoodDetail> foodDetails, MySqlTransaction mySqlTransaction);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="id">Danh sách khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (3/8/2022)
        public string DeleteMultiple(string IDs, MySqlTransaction? transaction, string FoodId);
    }
}
