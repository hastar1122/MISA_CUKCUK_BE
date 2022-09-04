using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.INFRASTRUCTURE.Repository
{
    public class FoodDetailRepository : BaseRepository<FoodDetail>, IFoodDetailRepository
    {
        public FoodDetailRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Thêm mới danh sách chi tiết thực đơn
        /// </summary>
        /// <param name="foodId">Khóa chính của thực đơn được thêm mới</param>
        /// <param name="foodAdditionIDs">Danh sách khóa chính của sở thích phục vụ</param>
        /// <param name="mySqlTransaction"></param>
        /// <returns>Số cột được thêm mới</returns>
        public int InsertMultiple(IEnumerable<FoodDetail> foodDetails, MySqlTransaction mySqlTransaction)
        {
            var sqlQuery = $"INSERT INTO FoodDetail(FoodDetailID, FoodID, FoodAdditionID) VALUES (@FoodDetailID, @FoodID, @FoodAdditionID)";

            var mySqlConnection = mySqlTransaction.Connection;
            var rowsEffect = mySqlConnection.Execute(sqlQuery, param: foodDetails, transaction: mySqlTransaction);
            return rowsEffect;
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="id">Danh sách khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (3/8/2022)
        public string DeleteMultiple(string IDs, MySqlTransaction? transaction, string foodID)
        {
            var sqlString = $"DELETE FROM FoodDetail WHERE FIND_IN_SET(FoodAdditionId, @IDs) AND FoodID = @FoodID";

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("@IDs", IDs);
            dynamicParams.Add("@FoodID", foodID);

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
    }
}
