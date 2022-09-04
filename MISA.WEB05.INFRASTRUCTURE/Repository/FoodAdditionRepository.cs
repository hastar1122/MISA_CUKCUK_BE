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
    public class FoodAdditionRepository : BaseRepository<FoodAddition>, IFoodAdditionRepository
    {
        public FoodAdditionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Hàm lấy danh sách sở thích phục vụ theo thực đơn
        /// </summary>
        /// <param name="foodID">Khóa chính thực đơn</param>
        /// <returns>Danh sách sở thích phục vụ</returns>
        /// Created by: NHANH (23/8/2022)
        public IEnumerable<FoodAddition> GetDataByFoodId(Guid foodID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FoodID", foodID);

            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlQuery = "Proc_GetFoodAdditionByFoodID";

                var foodAdditions = MySqlConnection.Query<FoodAddition>(
                    sqlQuery,
                    param: parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return foodAdditions;
            }
        }

        /// <summary>
        /// Hàm kiểm tra sở thích phục vụ đã tồn tại chưa
        /// </summary>
        /// <param name="foodAddition">Sở thích phục vụ</param>
        /// <param name="ID">Khóa ID </param>
        /// <returns>
        /// true => đã tồn tại
        /// false => chưa có
        /// </returns>
        /// Created by: NHANH (27/8/2022)
        public bool CheckDuplicate(FoodAddition foodAddition, Guid? ID)
        {
            var sqlString = "";
            DynamicParameters dynamicParams = new DynamicParameters();
            if (ID == null)
            {
                sqlString = $"SELECT FoodAdditionID FROM FoodAddition WHERE FoodAdditionDescription = @FoodAdditionDescription And FoodAdditionPrice = @FoodAdditionPrice";
                dynamicParams.Add("@FoodAdditionDescription", foodAddition.FoodAdditionDescription.Trim());
                dynamicParams.Add("@FoodAdditionPrice", foodAddition.FoodAdditionPrice);
            }
            else if (ID != null)
            {
                sqlString = $"SELECT FoodAdditionID FROM FoodAddtion WHERE FoodAdditionDescription = @FoodAdditionDescription And FoodAdditionPrice = @FoodAdditionPrice And FoodAdditionID != @FoodAdditionID";
                dynamicParams.Add("@FoodAdditionDescription", foodAddition.FoodAdditionDescription.Trim());
                dynamicParams.Add("@FoodAdditionPrice", foodAddition.FoodAdditionPrice);
                dynamicParams.Add("@FoodAdditionID", foodAddition.FoodAdditionID);
            }
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var res = MySqlConnection.QueryFirstOrDefault(sqlString, param: dynamicParams);
                if (res == null)
                    return false;
                return true;
            }
        }
    }
}
