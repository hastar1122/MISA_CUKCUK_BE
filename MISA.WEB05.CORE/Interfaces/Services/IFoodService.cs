using Microsoft.AspNetCore.Http;
using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Services
{
    /// <summary>
    /// Interface service thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public interface IFoodService: IBaseService<Food>
    {
        /// <summary>
        /// Hàm tạo command Text để lọc và phân trang thực đơn
        /// </summary>
        /// <param name="filter">Thông tin lọc</param>
        /// <returns></returns>
        /// Created by: NHANH (15/8/2022)
        public object FilterFood(Filter filter);

        /// <summary>
        /// Hàm tạo mã thực đơn theo đơn thực đơn
        /// </summary>
        /// <param name="foodName">tên thực đơn</param>
        /// <returns>Mã thực đơn</returns>
        /// Created by: NHANH (23/8/2022)
        public string GetFoodCode(string foodName);

        /// <summary>
        /// Service upload ảnh
        /// </summary>
        /// <param name="fileImage">file ảnh</param>
        /// <returns>Đường dẫn file ảnh</returns>
        /// Created by: NHANH (28/8/2022)
        public string UploadImage(IFormFile fileImage);

        /// <summary>
        /// Service trả lại luồng dữ liệu để suất ra file excel
        /// </summary>
        /// <param name="filter">Thông tin lọc</param>
        /// <returns></returns>
        /// Created by: NHANH (29/8/2022)
        public Stream Export(Filter filter);
    }
}
