using MISA.WEB05.CORE.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Nơi chế biến
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class Kitchen: BaseEntity
    {
        #region Constructor
        public Kitchen()
        {
            this.KitchenID = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid KitchenID { get; set; }

        /// <summary>
        /// Tên nơi chế biến
        /// </summary>
        [NotAllowedNull, NotAllowedDuplicate, PropsName("Nơi chế biến")]
        public string KitchenName { get; set; }

        /// <summary>
        /// Mô tả nơi chế biến
        /// </summary>
        public string? KitchenDescription { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
