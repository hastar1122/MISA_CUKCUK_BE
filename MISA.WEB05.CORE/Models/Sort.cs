using MISA.WEB05.CORE.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Sắp xếp theo cột
    /// </summary>
    /// Created by: NHANH (15/8/2022)
    public class Sort
    {
        /// <summary>
        /// Tên thuộc tính
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Sắp xếp theo thứ tự
        /// </summary>
        public Direction Direction { get; set; }
    }
}
