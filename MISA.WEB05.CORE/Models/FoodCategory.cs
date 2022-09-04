using MISA.WEB05.CORE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Nhóm thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class FoodCategory: BaseEntity
    {
        #region Constructor
        public FoodCategory()
        {
            this.FoodCategoryID = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid FoodCategoryID { get; set; }

        /// <summary>
        /// Mã nhóm thực đơn
        /// </summary>
        [NotAllowedNull, NotAllowedDuplicate, PropsName("Mã nhóm thực đơn")]
        public string FoodCategoryCode { get; set; }

        /// <summary>
        /// Tên nhóm thực đơn
        /// </summary>
        [NotAllowedNull, PropsName("Tên nhóm thực đơn")]
        public string FoodCategoryName { get; set; }

        /// <summary>
        /// Mô tả nhóm thực đơn
        /// </summary>
        public string? FoodCategoryDescription { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
