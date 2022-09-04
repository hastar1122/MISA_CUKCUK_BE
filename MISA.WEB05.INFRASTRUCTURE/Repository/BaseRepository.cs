using MySqlConnector;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Enum;
using System.Reflection;
using MISA.WEB05.CORE.Attributes;

namespace MISA.WEB05.INFRASTRUCTURE.Repository
{
    public class BaseRepository<MISAEntity>: IBaseRepository<MISAEntity>
    {
        #region Properties
        protected string ConnectionString;
        protected MySqlConnection MySqlConnection;
        protected string TableName;
        protected IConfiguration _configuration;
        #endregion

        #region Constructor
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            // Khai báo thông tin csdl
            ConnectionString = _configuration.GetValue<string>("ConnectionStrings");
            TableName = typeof(MISAEntity).Name;    
        }
        #endregion

        #region Methods
        /// <summary>
        /// Hàm mở connection
        /// </summary>
        /// Created by: NHANH (4/8/2022)
        public MySqlConnection OpenConnection()
        {
            MySqlConnection = new MySqlConnection(ConnectionString);
            return MySqlConnection;
        }

        /// <summary>
        /// Hàm đóng connection
        /// </summary>
        /// Created by: NHANH (4/8/2022)
        public void CloseConnection ()
        {
            if (MySqlConnection.State != System.Data.ConnectionState.Closed)
            {
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
        }

        /// <summary>
        /// @author: VQPhong (02/08/2022)
        /// @desc: Add dynamic parameters for an entity
        /// </summary>
        /// <param name="entity">The entity need to be added dynamic params</param>
        /// <param name="props">The list of props of the entity</param>
        /// <param name="parameters">The dynamic parameters object</param>
        protected DynamicParameters AddEntityToDynamicParams(Object entity)
        {
            DynamicParameters parameters = new DynamicParameters();
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(NotParameter), true).Length <= 0)
                {
                    var propName = prop.Name;
                    var propValue = prop.GetValue(entity);

                    parameters.Add($"@{propName}", propValue);
                }
            }

            return parameters;
        }

        /// <summary>
        /// Lấy toàn bộ bản ghi trong table
        /// </summary>
        /// <returns></returns>
        /// Created by: NHANH (3/8/2022)
        public IEnumerable<MISAEntity> Get()
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = $"Proc_Get{TableName}";
                var data = MySqlConnection.Query<MISAEntity>(sqlString,commandType: System.Data.CommandType.StoredProcedure);
                return data;
            }
        }

        /// <summary>
        /// Lấy thông tin bản ghi theo khóa chinh
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (3/8/2022)
        public virtual MISAEntity Get(Guid id)
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = $"Proc_Get{TableName}ByID";
                DynamicParameters dynamicParams = new DynamicParameters();
                dynamicParams.Add($"{TableName}ID", id);
                var data = MySqlConnection.QueryFirstOrDefault<MISAEntity>(sqlString, param: dynamicParams, commandType: System.Data.CommandType.StoredProcedure);
                return data;
            }
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>ID của bản ghi vừa được thêm mới</returns>
        /// Created by: NHANH (3/8/2022)
        public virtual Guid Insert(MISAEntity entity, MySqlTransaction? mySqlTransaction)
        {
            var sqlString = $"Proc_Insert{TableName}";

            DynamicParameters parameters = AddEntityToDynamicParams(entity);

            parameters.Add($"@New{TableName}ID", direction: System.Data.ParameterDirection.Output);

            if (mySqlTransaction != null)
            {
                var mySqlConnection = mySqlTransaction.Connection;
                
                mySqlConnection.QueryFirstOrDefault(sqlString, param: parameters, commandType: System.Data.CommandType.StoredProcedure,transaction: mySqlTransaction);

                Guid newID = parameters.Get<Guid>($"@New{TableName}ID");

                return newID;
            }
            else
            {
                using (MySqlConnection = new MySqlConnection(ConnectionString))
                {
                    MySqlConnection.QueryFirstOrDefault(sqlString, param: parameters, commandType: System.Data.CommandType.StoredProcedure);

                    Guid newID = parameters.Get<Guid>($"@New{TableName}ID");

                    return newID;
                }
            }
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Số dòng được cập nhật</returns>
        /// Created by: NHANH (3/8/2022)
        public int Update(MISAEntity entity, MySqlTransaction? mySqlTransaction)
        {
            var sqlString = $"Proc_Update{TableName}";

            DynamicParameters parameters = AddEntityToDynamicParams(entity);

            if (mySqlTransaction != null)
            {
                var mySqlConnection = mySqlTransaction.Connection;

                var res = mySqlConnection.Execute(sqlString, param: parameters, commandType: System.Data.CommandType.StoredProcedure,transaction: mySqlTransaction);

                return res; 
            }
            else
            {
                using (MySqlConnection = new MySqlConnection(ConnectionString))
                {
                    var res = MySqlConnection.Execute(sqlString, param: parameters, commandType: System.Data.CommandType.StoredProcedure);

                    return res;
                }
            }
        }

        /// <summary>
        /// Xóa bản ghi theo khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns>Số dòng bị xóa</returns>
        /// Created by: NHANH 3/8/2022)
        public int Delete(Guid id)
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = $"Proc_Delete{TableName}";
                DynamicParameters dynamicParams = new DynamicParameters();
                dynamicParams.Add($"{TableName}ID", id);
                var res = MySqlConnection.Execute(sqlString, param: dynamicParams, commandType: System.Data.CommandType.StoredProcedure);
                return res;
            }
        }

        /// <summary>
        /// Kiểm tra trùng dữ liệu trong database
        /// </summary>
        /// <param name="entity">Thực thể cần kiểm tra</param>
        /// <param name="propName">Thuộc tính không được trùng dữ liệu</param>
        /// <param name="ID">Nếu khác null nghĩa là check trùng cho update</param>
        /// <returns></returns>
        /// Created by: NHANH 4/8/2022)
        public virtual bool CheckDuplicate(MISAEntity entity, string propName,Guid? ID)
        {
            var sqlString = "";
            DynamicParameters dynamicParams = new DynamicParameters();
            if (ID == null)
            {
                sqlString = $"SELECT {propName} FROM {TableName} WHERE {TableName}.{propName} = @{propName}";
                dynamicParams.Add($"@{propName}", entity.GetType().GetProperty(propName).GetValue(entity, null).ToString().Trim());
            }
            else if (ID != null)
            {
                sqlString = $"SELECT {propName} FROM {TableName} WHERE {TableName}.{propName} = @{propName} AND {TableName}.{TableName}ID != @{TableName}ID";
                dynamicParams.Add($"@{propName}", entity.GetType().GetProperty(propName).GetValue(entity, null).ToString().Trim());
                dynamicParams.Add($"{TableName}ID", entity.GetType().GetProperty($"{TableName}ID").GetValue(entity, null));
            }
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var res = MySqlConnection.QueryFirstOrDefault(sqlString, param: dynamicParams);
                if (res == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="id">Danh sách khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (3/8/2022)
        public virtual string DeleteMultiple(string IDs, MySqlTransaction? transaction)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE FIND_IN_SET({TableName}Id, @IDs)";

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("@IDs", IDs);

            if (transaction != null)
            {
                var mySqlConnection = transaction.Connection;

                var res = mySqlConnection.QueryFirstOrDefault(sqlString, param: dynamicParams, transaction);

                return IDs;
            }
            else
            {
                using (MySqlConnection = new MySqlConnection(ConnectionString))
                {
                    var res = MySqlConnection.Execute(sqlString, param: dynamicParams);

                    return IDs;
                }
            }
        }
        #endregion
    }
}
