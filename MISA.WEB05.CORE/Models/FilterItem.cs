using MISA.WEB05.CORE.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Models
{
    /// <summary>
    /// Thông tin filter
    /// </summary>
    /// Created by: NHANH (15/8/2022)
    public class FilterItem
    {
        /// <summary>
        /// Thuộc tính lọc
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Kiểu filer
        /// </summary>
        public Operator Operator { get; set; }

        /// <summary>
        /// Giá trị lọc
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Kiểu dữ liệu
        /// </summary>
        public string Type { get; set; }
    }
}
