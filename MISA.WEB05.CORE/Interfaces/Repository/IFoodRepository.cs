using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Repository
{
    /// <summary>
    /// Interface repository thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public interface IFoodRepository: IBaseRepository<Food>
    {
        /// <summary>
        /// Tìm kiếm và phân trang các thực đơn
        /// </summary>
        /// <param name="m_where">Câu điều kiện where</param>
        /// <param name="m_sort">Câu sắp sắp theo cột</param>
        /// <param name="m_paging">Câu phân trang</param>
        /// <returns>Danh sách các thực đơn tìm được</returns>
        /// Created by: NHANH (15/8/2022)
        dynamic Filter(string m_where, string? m_sort, string m_paging);

        /// <summary>
        /// Kiểm tra mã thực đơn đã tồn tại chưa
        /// </summary>
        /// <param name="foodeCode">Mã thực đơn</param>
        /// <returns>
        /// true => đã tồn tại
        /// false => chưa tồn tại
        /// </returns>
        public bool CheckFoodCodeExsits(string foodeCode);
    }
}
