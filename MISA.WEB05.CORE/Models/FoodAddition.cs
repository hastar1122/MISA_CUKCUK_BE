using MISA.WEB05.CORE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Sở thích phục vụ
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class FoodAddition: BaseEntity
    {
        #region Constructor
        public FoodAddition()
        {
            this.FoodAdditionID = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid? FoodAdditionID { get; set; }

        /// <summary>
        /// Mô tả sở thích phục vụ
        /// </summary>
        [NotAllowedNull, PropsName("Mô tả sở thích phục vụ")]
        public string FoodAdditionDescription { get; set; }

        /// <summary>
        /// Giá thu thêm
        /// </summary>
        public double? FoodAdditionPrice { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
