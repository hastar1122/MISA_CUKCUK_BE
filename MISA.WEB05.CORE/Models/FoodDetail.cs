using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Chi tiết thực đơn
    /// </summary>
    /// Created by: NHANH (3/8/2022)
    public class FoodDetail
    {
        #region Constructor
        public FoodDetail()
        {
            this.FoodDetailID = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid FoodDetailID { get; set; }

        /// <summary>
        /// Khóa ngoại thực đơn
        /// </summary>
        public Guid FoodID { get; set; }

        /// <summary>
        /// Khóa ngoại sở thích phục vụ
        /// </summary>
        public Guid FoodAdditionID { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
