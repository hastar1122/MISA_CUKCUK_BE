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
    public class FoodRepository : BaseRepository<Food>, IFoodRepository
    {
        public FoodRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Tìm kiếm và phân trang các thực đơn
        /// </summary>
        /// <param name="m_where">Câu điều kiện where</param>
        /// <param name="m_sort">Câu sắp sắp theo cột</param>
        /// <param name="m_paging">Câu phân trang</param>
        /// <returns>Danh sách các thực đơn tìm được</returns>
        /// Created by: NHANH (15/8/2022)
        public dynamic Filter(string m_where, string? m_sort, string m_paging)
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = $"Proc_FilterFood";
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@m_Where", m_where);
                dynamicParameters.Add("@m_Sort", m_sort);
                dynamicParameters.Add("@m_Paging", m_paging);

                var res = MySqlConnection.QueryMultiple(sqlString, param: dynamicParameters, commandType: System.Data.CommandType.StoredProcedure);

                var listFood = res.Read<Food>().ToList();
                var totalRecord = res.Read<int>().Single();

                return new
                {
                    TotalRecord = totalRecord,
                    Data = listFood
                };
            }
        }

        /// <summary>
        /// Override hàm lấy thông tin món ăn theo khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns></returns>
        /// Create by: NHANH (5/8/2022)
        public override Food Get(Guid id)
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = $"Proc_Get{TableName}ByID";
                DynamicParameters dynamicParams = new DynamicParameters();
                dynamicParams.Add($"{TableName}ID", id);

                var foods = MySqlConnection.Query<Food, FoodAddition, Food>(sqlString, (food, foodAddition) =>
                {
                    if (foodAddition != null)
                    {
                        food.FoodAdditions.Add(foodAddition);
                    }
                    return food;
                }, splitOn: "FoodAdditionID", param: dynamicParams,commandType: System.Data.CommandType.StoredProcedure);

                var result = foods.GroupBy(m => m.FoodID).Select(m =>
                {
                    var food = m.First();
                    food.FoodAdditions = m.Where(fa => fa.FoodAdditions.Count > 0).Select(fa => fa.FoodAdditions.First()).ToList();
                    return food;
                });

                return result.FirstOrDefault();
            }
        }

        /// <summary>
        /// Kiểm tra mã thực đơn đã tồn tại chưa
        /// </summary>
        /// <param name="foodeCode">Mã thực đơn</param>
        /// <returns>
        /// true => đã tồn tại
        /// false => chưa tồn tại
        /// </returns>
        public bool CheckFoodCodeExsits(string foodeCode)
        {
            using (MySqlConnection = new MySqlConnection(ConnectionString))
            {
                var sqlString = "SELECT FoodID FROM Food WHERE FoodCode = @FoodCode";
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("@FoodCode", foodeCode);
                var res = MySqlConnection.QueryFirstOrDefault(sqlString, param: dynamicParams);
                if (res == null)
                    return false;
                return true;
            }
        }
    }
}
