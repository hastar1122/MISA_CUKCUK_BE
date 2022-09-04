using MISA.WEB05.CORE.Enum;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Interfaces.Repository
{
    public interface IBaseRepository<MISAEntity>
    {
        /// <summary>
        /// Hàm mở connection
        /// </summary>
        /// <returns>Connection</returns>
        public MySqlConnection OpenConnection();
        /// <summary>
        /// Lấy toàn bộ danh sách dữ liệu trong bảng
        /// </summary>
        /// <returns></returns>
        /// Created by: NHANH (4/7/2022)
        IEnumerable<MISAEntity> Get();

        /// <summary>
        /// Lấy thông tin dữ liệu theo khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (4/7/2022)
        MISAEntity Get(Guid id);

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns></returns>
        /// Created by: NHANH (4/7/2022)
        Guid Insert(MISAEntity entity, MySqlTransaction? mySqlTransaction);

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns></returns>
        /// Created by: NHANH (4/7/2022)
        int Update(MISAEntity entity, MySqlTransaction? mySqlTransaction);

        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (4/7/2022)
        int Delete(Guid id);

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="id">Danh sách khóa chính của bản ghi bị xóa</param>
        /// <returns></returns>
        /// Created by: NHANH (18/7/2022)
        string DeleteMultiple(string id, MySqlTransaction? transaction);

        /// <summary>
        /// Kiểm tra trùng dữ liệu
        /// </summary>
        /// <param name="entity">Thực thể cần kiểm tra</param>
        /// <param name="propName">Thuộc tính cần kiểm tra trùng dữ liệu</param>
        /// <param name="ID">Nếu khác null nghĩa là check trùng cho update</param>
        /// <returns></returns>
        /// Created by: NHANH (4/8/2022)
        bool CheckDuplicate(MISAEntity entity, string propName, Guid? ID);
    }
}
