using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Services
{
    public interface IBaseService<MISAEntity>
    {
        /// <summary>
        /// Service thêm mới 1 bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Khóa chính của bản ghi được thêm mới</returns>
        /// Created by: NHANH (4/7/2022)
        Guid InsertService(MISAEntity entity);

        /// <summary>
        /// Service cập nhật 1 bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Số dòng được cập nhật</returns>
        /// Created by: NHANH (4/7/2022)
        int UpdateService(MISAEntity entity);
    }
}
