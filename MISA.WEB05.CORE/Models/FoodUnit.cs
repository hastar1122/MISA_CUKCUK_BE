using MISA.WEB05.CORE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Đơn vị tính
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class FoodUnit: BaseEntity
    {
        #region Constructor
        public FoodUnit()
        {
            this.FoodUnitID = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chinh
        /// </summary>
        public Guid FoodUnitID { get; set; }

        /// <summary>
        /// Tên đơn vị
        /// </summary>
        [NotAllowedNull, NotAllowedDuplicate, PropsName("Đơn vị tính")]
        public string FoodUnitName { get; set; }

        /// <summary>
        /// Mô tả đơn vị tính
        /// </summary>
        public string? FoodUnitDescription { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
