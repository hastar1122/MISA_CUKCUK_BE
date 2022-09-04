using MISA.WEB05.CORE.Exceptions;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Services
{
    public class FoodAdditionService: BaseService<FoodAddition>, IFoodAdditionService
    {
        IFoodAdditionRepository _Repository;

        public FoodAdditionService(IFoodAdditionRepository repository):base(repository)
        {
            _Repository = repository;
        }

        /// <summary>
        /// Hàm loại bỏ những sở thích phục vụ không có nội dung và thu thêm <=0
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được lọc</param>
        /// <returns>Danh sách sở thích phục vụ mới</returns>
        /// Created by: NHANH (22/8/2022)
        public List<FoodAddition> FilterEmptyFoodAddition(List<FoodAddition> foodAdditions)
        {
            List<FoodAddition> res = new List<FoodAddition>(); 

            foreach (var item in foodAdditions)
            {
                if ((String.IsNullOrWhiteSpace(item.FoodAdditionDescription)) && (item.FoodAdditionPrice <= 0 || item.FoodAdditionPrice == null))
                {
                    continue;
                }
                else if (!String.IsNullOrWhiteSpace(item.FoodAdditionDescription) && (item.FoodAdditionPrice <= 0 || item.FoodAdditionPrice == null)) {
                    item.FoodAdditionPrice = 0;
                    res.Add(item);
                }
                else {
                    res.Add(item);
                }
            }

            return res;
        }

        /// <summary>
        /// Hàm kiểm tra sở thích phục vụ có thu thêm nhưng không có nội dung
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được kiểm tra</param>
        /// <returns>
        /// true => có
        /// faslse => không
        /// </returns>
        /// Created by: NHANH (22/8/2022)
        public bool CheckNoDescriptionFoodAddition(List<FoodAddition> foodAdditions)
        {
            foreach (var item in foodAdditions)
            {
                if ((item.FoodAdditionDescription == null || item.FoodAdditionDescription.Trim() == "") && item.FoodAdditionPrice > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Hàm kiểm tra những sở thích phục vụ bị trùng lặp
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục vụ được kiểm tra</param>
        /// <returns>Danh sách sở thích phục vụ bị lặp dạng list chuỗi</returns>
        /// Created by: NHANH (22/8/2022)
        public List<string> GetFoodAdditionDuplicate(List<FoodAddition> foodAdditions)
        {
            var res = new List<string>();

            var tempString = String.Empty;

            for (int i = 0; i< foodAdditions.Count() -1; i++ )
            {
                for ( int y = i + 1; y < foodAdditions.Count(); y++)
                {
                    if (foodAdditions[i].FoodAdditionDescription == foodAdditions[y].FoodAdditionDescription && foodAdditions[i].FoodAdditionPrice == foodAdditions[y].FoodAdditionPrice)
                    {
                        tempString = $"{foodAdditions[i].FoodAdditionDescription} - {foodAdditions[i].FoodAdditionPrice}";
                        if (!res.Contains(tempString))
                        {
                            res.Add(tempString);
                        }
                    }
                }
            } 

            return res;
        }

        /// <summary>
        /// Hàm gán ID cho sở thích phục vụ đã có trong đã database và gắn null cho những sở thích phục vụ chưa có trong database
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục cần xử lý</param>
        /// Created by: NHANH (22/8/2022)
        public void UpdateFoodAdditionID(List<FoodAddition> foodAdditions)
        {
            var allFoodAddition = _Repository.Get().ToList();

            foreach (var fa in foodAdditions)
            {
                var foodAddition = allFoodAddition.Where(p => p.FoodAdditionDescription == fa.FoodAdditionDescription && p.FoodAdditionPrice == fa.FoodAdditionPrice).FirstOrDefault();

                if (foodAddition != null)
                {
                    fa.FoodAdditionID = foodAddition.FoodAdditionID;
                }
                else
                {
                    fa.FoodAdditionID = Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Hàm tạo ra danh sách các sở thích phục vụ cần xóa khi cập nhật 1 thực đơn
        /// </summary>
        /// <param name="newFoodAddition">Danh sách sở thích phục vụ mới</param>
        /// <param name="foodID">Khóa chính của thực đơn</param>
        /// <returns>Danh sách khóa chính của các sở thích phục vụ cần xóa</returns>
        /// Created by: NHANH (22/8/2022)
        public List<Guid> CreateDeleteFoodAdditions(List<FoodAddition> newFoodAddition, Guid foodID)
        {
            List<Guid> res = new List<Guid>();

            var oldFoodAdditions = _Repository.GetDataByFoodId(foodID);

            foreach (var item in oldFoodAdditions)
            {
                if (newFoodAddition.Where(m => m.FoodAdditionDescription == item.FoodAdditionDescription && m.FoodAdditionPrice == item.FoodAdditionPrice).ToList().Any() == false)
                {
                    if (item.FoodAdditionID != null)
                    {
                        res.Add((Guid)item.FoodAdditionID);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Hàm lấy danh sách sở thích phục vụ mới không có trong danh sách sở thích phục cũ theo thực đơn
        /// </summary>
        /// <param name="newFoodAdditions">Danh sách sở thích phục vụ mới</param>
        /// <param name="foodId">Khóa chính của thực đơn</param>
        /// <returns>Danh sách phục vụ để thêm mới</returns>
        /// Created by: NHANH (23/8/2022)
        public List<FoodAddition> GetNewFoodAddtionForUpdate(List<FoodAddition> newFoodAdditions, Guid foodId)
        {
            List<FoodAddition> res = new();

            var foodAdditionsByFoodID = _Repository.GetDataByFoodId(foodId).ToList();

            foreach (var item in newFoodAdditions)
            {
                if (!foodAdditionsByFoodID.Where(p => p.FoodAdditionDescription == item.FoodAdditionDescription && p.FoodAdditionPrice == item.FoodAdditionPrice).ToList().Any())
                {
                    res.Add(item);
                }
            }

            return res;
        }

        /// <summary>
        /// Override hàm thêm mới sở thích phục vụ
        /// </summary>
        /// <param name="foodAddition">Thông tin sở thích phục vụ</param>
        /// <returns>ID của bản ghi được thêm mới</returns>
        /// Created by: NHANH (27/8/2022)
        public override Guid InsertService(FoodAddition foodAddition)
        {
            /// Validate dữ liệu
            CheckProPertiesNotAllowedNull(foodAddition);
            
            if (_Repository.CheckDuplicate(foodAddition, null))
            {
                IsValid = false;
                ErrorValidateMsgs.Add(String.Format(Resources.Validate.DuplicateFoodAddition, foodAddition.FoodAdditionDescription, foodAddition.FoodAdditionPrice));
            }

            /// Thực hiện thêm mới
            if (IsValid == true)
            {
                // Build lại dữ liệu
                if (foodAddition.FoodAdditionPrice == null || foodAddition.FoodAdditionPrice < 0)
                {
                    foodAddition.FoodAdditionPrice = 0;
                }

                return _Repository.Insert(foodAddition, null);
            }
            else
            {
                throw new MISAValidateException(ErrorValidateMsgs[0], ErrorValidateMsgs);
            }
        }
    }
}
