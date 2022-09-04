using MISA.WEB05.CORE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class Food: BaseEntity
    {
        #region Constructor
        public Food()
        {
            this.FoodID = Guid.NewGuid();
            this.FoodAdditions = new List<FoodAddition>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid FoodID { get; set; }

        /// <summary>
        /// Mã thực đơn
        /// </summary>
        [NotAllowedNull, NotAllowedDuplicate, PropsName("Mã món ăn")]
        public string FoodCode { get; set; }

        /// <summary>
        /// Tên thực đơn
        /// </summary>
        [NotAllowedNull, PropsName("Tên món ăn")]
        public string FoodName { get; set; }

        /// <summary>
        /// Khóa ngoại nhóm thực đơn
        /// </summary>
        public Guid? FoodCategoryID { get; set; }

        /// <summary>
        /// Tên nhóm thực đơn
        /// </summary>
        public string? FoodCategoryName { get; set; }

        /// <summary>
        /// Khóa ngoại đơn vị tính
        /// </summary>
        [NotAllowedNull, PropsName("Đơn vị tính")]
        public Guid FoodUnitID { get; set; }

        /// <summary>
        /// Tên đơn vị tính
        /// </summary>
        public string? FoodUnitName { get; set; }

        /// <summary>
        /// Giá bán
        /// </summary>
        [NotAllowedNull, PropsName("Giá bán")]
        public double FoodPrice { get; set; }

        /// <summary>
        /// Giá vốn
        /// </summary>
        public double? FoodOutwardPrice { get; set; }

        /// <summary>
        /// Miêu tả thực đơn
        /// </summary>
        public string? FoodDescription { get; set; }

        /// <summary>
        /// Khóa ngoại nơi chế biến
        /// </summary>
        public Guid? KitchenID { get; set; }

        /// <summary>
        /// Tên nơi chế biến
        /// </summary>
        public string? KitchenName { get; set; }

        /// <summary>
        /// Hình ảnh thực đơn
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Thay đổi theo thời giá
        /// </summary>
        public bool? ChangeByTime { get; set; }

        /// <summary>
        /// Điều chỉnh giá tự do
        /// </summary>
        public bool? AllowedChange { get; set; }

        /// <summary>
        /// Hiển thị trên menu
        /// </summary>
        public bool? HideInMenu { get; set; }

        /// <summary>
        /// Ngừng bán
        /// </summary>
        public bool? InActive { get; set; }

        /// <summary>
        /// Danh sách sở thích phục vụ
        /// </summary>
        [NotParameter]
        public List<FoodAddition> FoodAdditions { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
