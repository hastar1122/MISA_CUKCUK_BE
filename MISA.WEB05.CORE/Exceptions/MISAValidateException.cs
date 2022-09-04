using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Exceptions
{
    public class MISAValidateException: Exception
    {
        public string? ValidateErrorMsg { get; set; }

        public IDictionary Errors { get; set; }

        /// <summary>
        /// Khởi tạo Exception với 1 lỗi
        /// </summary>
        /// <param name="errorMsg"></param>
        /// Created by: NHANH (4/7/2022)
        public MISAValidateException(string errorMsg)
        {
            ValidateErrorMsg = errorMsg;
        }

        /// <summary>
        /// Khởi tạo Exception với nhiều lỗi
        /// </summary>
        /// <param name="errorMsg">Lỗi đầu tiên trong danh sách lỗi được tạo</param>
        /// <param name="errorMsgs">Danh sách lỗi</param>
        /// Created by: NHANH (5/7/2022)
        public MISAValidateException(string errorMsg, List<string> errorMsgs)
        {
            ValidateErrorMsg = errorMsg;
            Errors = new Dictionary<string, object>();  
            Errors.Add("errors", errorMsgs);
        }

        public override string Message => this.ValidateErrorMsg;

        public override IDictionary Data => this.Errors;
    }
}
