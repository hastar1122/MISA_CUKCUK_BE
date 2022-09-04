using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Services
{
    /// <summary>
    /// Interface service sở thích phục vụ
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public interface IFoodAdditionService: IBaseService<FoodAddition>
    {
        /// <summary>
        /// Hàm kiểm tra những sở thích phục vụ bị trùng lặp
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được kiểm tra</param>
        /// <returns>Danh sách sở thích phục vụ bị lặp dạng list chuỗi</returns>
        /// Created by: NHANH (22/8/2022)
        public List<string> GetFoodAdditionDuplicate(List<FoodAddition> foodAdditions);

        /// <summary>
        /// Hàm kiểm tra sở thích phục vụ có thu thêm nhưng không có nội dung
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được kiểm tra</param>
        /// <returns>
        /// true => có
        /// faslse => không
        /// </returns>
        /// Created by: NHANH (22/8/2022)
        public bool CheckNoDescriptionFoodAddition(List<FoodAddition> foodAdditions);

        /// <summary>
        /// Hàm loại bỏ những sở thích phục vụ không có nội dung và thu thêm <=0
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được lọc</param>
        /// <returns>Danh sách sở thích phục vụ mới</returns>
        /// Created by: NHANH (22/8/2022)
        public List<FoodAddition> FilterEmptyFoodAddition(List<FoodAddition> foodAdditions);

        /// <summary>
        /// Hàm gán ID cho sở thích phục vụ đã có trong đã database và gắn null cho những sở thích phục vụ chưa có trong database
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục cần xử lý</param>
        /// Created by: NHANH (22/8/2022)
        public void UpdateFoodAdditionID(List<FoodAddition> foodAdditions);

        /// <summary>
        /// Hàm tạo ra danh sách các sở thích phục vụ cần xóa khi cập nhật 1 thực đơn
        /// </summary>
        /// <param name="newFoodAddition">Danh sách sở thích phục vụ mới</param>
        /// <param name="foodID">Khóa chính của thực đơn</param>
        /// <returns>Danh sách khóa chính của các sở thích phục vụ cần xóa</returns>
        /// Created by: NHANH (22/8/2022)
        public List<Guid> CreateDeleteFoodAdditions(List<FoodAddition> newFoodAddition, Guid foodID);

        /// <summary>
        /// Hàm lấy danh sách sở thích phục vụ mới không có trong danh sách sở thích phục cũ theo thực đơn
        /// </summary>
        /// <param name="newFoodAdditions">Danh sách sở thích phục vụ mới</param>
        /// <param name="foodId">Khóa chính của thực đơn</param>
        /// <returns>Danh sách phục vụ để thêm mới</returns>
        /// Created by: NHANH (23/8/2022)
        public List<FoodAddition> GetNewFoodAddtionForUpdate(List<FoodAddition> newFoodAdditions, Guid foodId);
    }
}
