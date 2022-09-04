using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MISA.WEB05.CORE.Enum;
using MISA.WEB05.CORE.Exceptions;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;
using MySqlConnector;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Services
{
    public class FoodService : BaseService<Food>, IFoodService
    {
        IFoodRepository _FoodRepository;
        IFoodAdditionRepository _FoodAdditionRepository;
        IFoodAdditionService _FoodAdditionService;
        IFoodDetailRepository _FoodDetailRepository;
        IConfiguration _configuration;

        public FoodService(IFoodRepository repository, IFoodAdditionRepository foodAdditionRepository, IFoodAdditionService foodAdditionService, IFoodDetailRepository foodDetailRepository, IConfiguration configuration) : base(repository)
        {
            _FoodRepository = repository;
            _FoodAdditionRepository = foodAdditionRepository;
            _FoodAdditionService = foodAdditionService;
            _FoodDetailRepository = foodDetailRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Service upload ảnh
        /// </summary>
        /// <param name="fileImage">file ảnh</param>
        /// <returns>Đường dẫn file ảnh</returns>
        /// Created by: NHANH (28/8/2022)
        public string UploadImage(IFormFile fileImage)
        {
            var supportTypes = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExt = Path.GetExtension(fileImage.FileName);
            // 1. Validate tệp
            // Check định dạng
            if (!supportTypes.Contains(fileExt))
            {
                throw new MISAValidateException("Tệp không đúng định dạng");
            }

            // Check dung lượng
            // Check dung lượng file ảnh
            if (fileImage.Length > 5 * 1024 * 1024)
            {
                throw new MISAValidateException("Ảnh không tải được do vượt quá dung lượng. Vui lòng chọn ảnh có dung lượng nhỏ hơn 5 MB");
            }

            // 2. Lưu ảnh
            string fileName = "Food_" + DateTime.Now.Ticks.ToString() + fileExt;

            // Tạo đường dẫn
            var pathBuild = Path.Combine(Directory.GetCurrentDirectory(), _configuration["StaticFolder:UploadPath"]);

            if (!Directory.Exists(pathBuild))
            {
                Directory.CreateDirectory(pathBuild);
            }

            // Khởi tạo đường dẫn ảnh
            var path = Path.Combine(Directory.GetCurrentDirectory(), _configuration["StaticFolder:UploadPath"], fileName);

            // Copy ảnh theo đường dẫn
            using (var stream = new FileStream(path, FileMode.Create))
            {
                fileImage.CopyTo(stream);
            }

            // Tạo link ảnh trả về cho client
            var linkImage = _configuration["StaticFolder:UploadLink"] + fileName;

            return linkImage;
        }

        /// <summary>
        /// Override hàm thêm mới thực đơn
        /// </summary>
        /// <param name="food">Thông tin thực đơn</param>
        /// <returns>ID của bản ghi được thêm mới</returns>
        /// Created by: NHANH (8/8/2022)
        public override Guid InsertService(Food food)
        {
            // Validate
            // 1. Validate Food
            ValidateFood(food, null);

            // 2. Validate FoodAddition
            List<FoodAddition> foodAdditions = new List<FoodAddition>(); // Biến lưu danh sách sở thích phục sau khi được lọc

            if (food.FoodAdditions.Count > 0 && food.FoodAdditions != null)
            {
                // Loại bỏ những sở thích phục không có nội dung và thu thêm <= 0
                foodAdditions = _FoodAdditionService.FilterEmptyFoodAddition(food.FoodAdditions);

                ValidateFoodAddition(foodAdditions);
            }

            // Thực hiện thêm mới
            if (IsValid == true)
            {
                if (foodAdditions.Any())
                {
                    // Cập nhập lại khóa chính cho danh sách sở thích phục vụ để thêm mới (nếu có rồi gán ID nếu chưa gán null để tí thêm mới)
                    _FoodAdditionService.UpdateFoodAdditionID(foodAdditions);
                }

                // Tạo connection và transaction
                MySqlConnection mySqlConnection = _FoodRepository.OpenConnection();
                mySqlConnection.Open();
                MySqlTransaction transaction = mySqlConnection.BeginTransaction();

                // Thực hiện quá trình thêm mới
                try
                {
                    // Thêm mới thực đơn
                    Guid newFoodID = _FoodRepository.Insert(food, transaction);

                    if (foodAdditions.Any())
                    {
                        // Biến lưu danh sách chi tiết thực đơn để thêm mới
                        List<FoodDetail> foodDetails = new List<FoodDetail>();

                        foreach (var item in foodAdditions)
                        {
                            // Nếu sở thích phục vụ không có trong databasse
                            if (item.FoodAdditionID == Guid.Empty || item.FoodAdditionID == null)
                            {
                                // Thêm mới sở thích phục vụ
                                var newFoodAdditionID = _FoodAdditionRepository.Insert(item, transaction);

                                foodDetails.Add(new FoodDetail() { FoodID = newFoodID, FoodAdditionID = newFoodAdditionID });
                            }
                            else
                            {
                                foodDetails.Add(new FoodDetail() { FoodID = newFoodID, FoodAdditionID = (Guid)item.FoodAdditionID });
                            }
                        }

                        // Thêm bảng trung gian (Chi tiết thực đơn)
                        _FoodDetailRepository.InsertMultiple(foodDetails, transaction);
                    }

                    transaction.Commit();

                    return newFoodID;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
            else
            {
                throw new MISAValidateException(ErrorValidateMsgs[0], ErrorValidateMsgs);
            }
        }

        /// <summary>
        /// Override hàm cập nhật thực đơn
        /// </summary>
        /// <param name="food">Thông tin thực đơn</param>
        /// <returns>Số dòng được cập nhật</returns>
        /// Created by: NHANH (23/8/2022)
        public override int UpdateService(Food food)
        {
            // Validate
            // 1. Validate Food
            ValidateFood(food, food.FoodID);

            // 2. Validate FoodAddition
            List<FoodAddition> foodAdditions = new List<FoodAddition>(); // Biến lưu danh sách sở thích phục sau khi được lọc

            if (food.FoodAdditions.Count > 0 && food.FoodAdditions != null)
            {
                // Loại bỏ những sở thích phục không có nội dung và thu thêm <= 0
                foodAdditions = _FoodAdditionService.FilterEmptyFoodAddition(food.FoodAdditions);

                ValidateFoodAddition(foodAdditions);
            }

            // Thực hiện thêm mới
            if (IsValid == true)
            {
                // Biến lưu ID của các sở thích phục vụ cần xóa vì không có trong danh sách sở thích phục mới
                //List<Guid> foodAdditionIDNeedDeleted = _FoodAdditionService.CreateDeleteFoodAdditions(foodAdditions, food.FoodID);
                // Lấy ra danh sách những sở thích phục vụ mới không có trong danh sách cũ để thêm mới
                //foodAdditions = _FoodAdditionService.GetNewFoodAddtionForUpdate(foodAdditions, food.FoodID);

                // Lấy danh sách sở thích phục vụ theo thực đơn
                List<FoodAddition> foodAdditionsByFoodID = _FoodAdditionRepository.GetDataByFoodId(food.FoodID).ToList();

                // Lấy ra danh sách những sở thích phục vụ có ở danh sách cũ mà không có ở danh sách mới để xóa
                List<FoodAddition> foodAdditionsNeedDeleted = foodAdditionsByFoodID.Where(m => !foodAdditions.Any(p => p.FoodAdditionDescription == m.FoodAdditionDescription && p.FoodAdditionPrice == m.FoodAdditionPrice)).ToList();

                // Lấy ra danh sách những sở thích phục vụ mới không có trong danh sách cũ để thêm mới
                foodAdditions = foodAdditions.Where(m => !foodAdditionsNeedDeleted.Any(p => m.FoodAdditionDescription == p.FoodAdditionDescription && m.FoodAdditionPrice == p.FoodAdditionPrice) &&
                !foodAdditionsByFoodID.Any(q => q.FoodAdditionDescription == m.FoodAdditionDescription && q.FoodAdditionPrice == m.FoodAdditionPrice)).ToList();

                // Cập nhập lại khóa chính cho danh sách sở thích phục vụ để thêm mới (nếu có rồi gán ID nếu chưa gán null để tí thêm mới)
                _FoodAdditionService.UpdateFoodAdditionID(foodAdditions);

                // Tạo connection và transaction
                MySqlConnection mySqlConnection = _FoodRepository.OpenConnection();
                mySqlConnection.Open();
                MySqlTransaction transaction = mySqlConnection.BeginTransaction();

                // Thực hiện quá trình cập nhật
                try
                {
                    // Cập nhật thực đơn
                    var res = _FoodRepository.Update(food, transaction);

                    if (foodAdditions.Any())
                    {
                        // Biến lưu danh sách chi tiết thực đơn để thêm mới
                        List<FoodDetail> foodDetails = new List<FoodDetail>();

                        foreach (var item in foodAdditions)
                        {
                            // Nếu sở thích phục vụ không có trong databasse
                            if (item.FoodAdditionID == Guid.Empty || item.FoodAdditionID == null)
                            {
                                // Thêm mới sở thích phục vụ
                                var newFoodAdditionID = _FoodAdditionRepository.Insert(item, transaction);

                                foodDetails.Add(new FoodDetail() { FoodID = food.FoodID, FoodAdditionID = newFoodAdditionID });
                            }
                            else
                            {
                                foodDetails.Add(new FoodDetail() { FoodID = food.FoodID, FoodAdditionID = (Guid)item.FoodAdditionID });
                            }
                        }

                        // Thêm bảng trung gian (Chi tiết thực đơn)
                        _FoodDetailRepository.InsertMultiple(foodDetails, transaction);
                    }

                    // Xóa những sở thích phục vụ có ở danh sách cũ mà không có ở danh sách mới

                    if (foodAdditionsNeedDeleted.Any())
                    {
                        StringBuilder listFoodAdditionIds = new StringBuilder();
                        listFoodAdditionIds.Append(String.Join(",", foodAdditionsNeedDeleted.Select(m => m.FoodAdditionID).ToList()));
                        _FoodDetailRepository.DeleteMultiple(listFoodAdditionIds.ToString(), transaction, food.FoodID.ToString());
                    }

                    transaction.Commit();

                    return res;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
            else
            {
                throw new MISAValidateException(ErrorValidateMsgs[0], ErrorValidateMsgs);
            }
        }

        /// <summary>
        /// Hàm validate danh sách sở thích phục vụ
        /// </summary>
        /// <param name="foodAdditions">Danh sách sở thích phục cần validate</param>
        /// Created by: NHANH (22/8/2022)
        private void ValidateFoodAddition(List<FoodAddition> foodAdditions)
        {
            // Kiểm tra sở thích phục vụ có thu thêm nhưng không có nội dung
            if (_FoodAdditionService.CheckNoDescriptionFoodAddition(foodAdditions))
            {
                IsValid = false;
                ErrorValidateMsgs.Add(Resources.Validate.NoDescriptionFoodAddition);
            }

            // Kiểm tra sở thích phục vụ bị trùng
            var listDupicateFoodAdditions = _FoodAdditionService.GetFoodAdditionDuplicate(foodAdditions);
            if (listDupicateFoodAdditions.Count > 0)
            {
                IsValid = false;
                StringBuilder errorMsg = new("Sở thích phục vụ <");
                errorMsg.Append(String.Join(",", listDupicateFoodAdditions));
                errorMsg.Append("> đã bị trùng. Vui lòng kiểm tra lại");
                ErrorValidateMsgs.Add(errorMsg.ToString());
            }
        }

        /// <summary>
        /// Hàm validate thực đơn
        /// </summary>
        /// <param name="food">thực đơn</param>
        /// <param name="foodID">khóa chính để xác định validate cho thêm mới hay update</param>
        /// Created by: NHANH (22/8/2022)
        private void ValidateFood(Food food, Guid? foodID)
        {
            base.CheckProPertiesNotAllowedNull(food);

            base.CheckPropertiesNotAllowedDuplicate(food, foodID);
        }

        /// <summary>
        /// Hàm tạo command Text để lọc và phân trang thực đơn
        /// </summary>
        /// <param name="filter">Thông tin lọc</param>
        /// <returns></returns>
        /// Created by: NHANH (15/8/2022)
        public object FilterFood(Filter filter)
        {
            string m_Paging;
            string m_Where = "WHERE 1 = 1 ";
            string? m_Sort = null;

            if (filter.Filters.Count > 0)
            {
                m_Where += BuildWhereCommandText(filter.Filters);
            }

            if (filter.Sort != null)
            {
                m_Sort = BuildSortCommandText(filter.Sort);
            }

            var offset = (filter.Page - 1) * filter.Limit;

            m_Paging = $"Limit {filter.Limit} OFFSET {offset}";

            var res = _FoodRepository.Filter(m_Where, m_Sort, m_Paging);

            int totalRecord = GetValueObject(res, "TotalRecord");

            var data = GetValueObject(res, "Data");

            var totalPage = Math.Floor(Convert.ToDecimal((totalRecord + filter.Limit - 1) / filter.Limit));

            return new
            {
                TotalPage = totalPage,
                TotalRecord = totalRecord,
                Data = data,
            };
        }

        /// <summary>
        /// Hàm build ra câu điều kiện where
        /// </summary>
        /// <param name="filters">Danh sách các thông tin lọc</param>
        /// <returns>Câu điều kiện where để lọc</returns>
        /// Created by: NHANH (15/8/2022)
        public string BuildWhereCommandText(List<FilterItem> filters)
        {
            string result = String.Empty;

            foreach (FilterItem filterItem in filters)
            {
                if (filterItem.Value != null && !String.IsNullOrWhiteSpace(filterItem.Value.ToString()))
                {
                    switch (filterItem.Operator)
                    {
                        case Operator.Contain:
                            result += $"and {filterItem.Property} LIKE CONCAT('%','{filterItem.Value}','%') ";
                            break;
                        case Operator.Equal:
                            if (filterItem.Type == "string")
                                result += $"and {filterItem.Property} = '{filterItem.Value}' ";
                            else if (filterItem.Type == "boolean" && Convert.ToInt16(filterItem.Value.ToString()) == 0)
                                result += $"and ({filterItem.Property} = {filterItem.Value} OR {filterItem.Property} IS NULL) ";
                            else
                                result += $"and {filterItem.Property} = {filterItem.Value} ";
                            break;
                        case Operator.Start_With:
                            result += $"and {filterItem.Property} LIKE CONCAT('{filterItem.Value}','%') ";
                            break;
                        case Operator.End_With:
                            result += $"and {filterItem.Property} LIKE CONCAT('%','{filterItem.Value}') ";
                            break;
                        case Operator.Not_Contain:
                            result += $"and {filterItem.Property} NOT LIKE CONCAT('%','{filterItem.Value}','%') ";
                            break;
                        case Operator.Less:
                            result += $"and {filterItem.Property} < {filterItem.Value} ";
                            break;
                        case Operator.Less_Or_Equal:
                            result += $"and {filterItem.Property} <= {filterItem.Value} ";
                            break;
                        case Operator.Bigger:
                            result += $"and {filterItem.Property} > {filterItem.Value} ";
                            break;
                        case Operator.Bigger_Or_Equal:
                            result += $"and {filterItem.Property} >= {filterItem.Value} ";
                            break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Hàm build ra câu sếp xếp theo cột
        /// </summary>
        /// <param name="sort">Thông tin cột được sắp xếp</param>
        /// <returns>Câu lệnh sắp xếp</returns>
        /// Created by: NHANH (15/8/2022)
        public string BuildSortCommandText(Sort sort)
        {
            string result = string.Empty;

            if (sort.Direction == Direction.ASC)
                result = $"Order by {sort.Property} ASC";
            else if (sort.Direction == Direction.DESC)
                result = $"Order by {sort.Property} DESC";
            return result;
        }

        /// <summary>
        /// Hàm lấy ra giá trị của thuộc tính trong object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="propertyName">Thuộc tính chứa giá trị cần lấy</param>
        /// <returns></returns>
        /// Created by: NHANH (16/7/2022)
        private object GetValueObject(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        /// <summary>
        /// Hàm tạo mã thực đơn theo đơn thực đơn
        /// </summary>
        /// <param name="foodName">tên thực đơn</param>
        /// <returns>Mã thực đơn</returns>
        /// Created by: NHANH (23/8/2022)
        public string GetFoodCode(string foodName)
        {
            if (foodName != null && foodName.Trim() != "")
            {
                var words = foodName.Trim().Split(' ');

                string foodCode = "";
                if (words.Length == 1)
                {
                    return convertToUnSign(words[0]).ToUpper();
                }

                foreach (var word in words)
                {
                    foodCode += word[0];
                }
                foodCode = convertToUnSign(foodCode).ToUpper();

                if (_FoodRepository.CheckFoodCodeExsits(foodCode) == false)
                {
                    return foodCode;
                }
                else
                {
                    foodCode = "";

                    foreach (var word in words)
                    {
                        foodCode += word[0];
                        if (word.Length > 1)
                            foodCode += word[1];
                    }
                    foodCode = convertToUnSign(foodCode).ToUpper();

                    if (_FoodRepository.CheckFoodCodeExsits(foodCode) == false)
                    {
                        return foodCode;
                    }
                    else
                    {
                        foodCode = "";
                        foreach (var word in words)
                        {
                            foodCode += word;
                        }

                        return convertToUnSign(foodCode).ToUpper();
                    }
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Chuyển chuỗi có dấu thành không dấu
        /// </summary>
        /// <param name="s">Chuỗi được chuyển</param>
        /// <returns></returns>
        /// Created by: NHANH (23/8/2022)
        private string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Service trả lại luồng dữ liệu để suất ra file excel
        /// </summary>
        /// <param name="filter">Thông tin lọc</param>
        /// <returns></returns>
        /// Created by: NHANH (29/8/2022)
        public Stream Export(Filter filter)
        {
            // Biến lưu thông tin lọc
            string m_Paging;
            string m_Where = "WHERE 1 = 1 ";
            string? m_Sort = null;

            if (filter.Filters.Count > 0)
            {
                m_Where += BuildWhereCommandText(filter.Filters);
            }

            if (filter.Sort != null)
            {
                m_Sort = BuildSortCommandText(filter.Sort);
            }

            // Đặt paging bằng rỗng để lấy ra toàn bộ danh sách thực đơn theo thông tin lọc
            m_Paging = " ";

            // Gọi repository lấy ra danh sách thực đơn theo thông tin lọc
            var res = _FoodRepository.Filter(m_Where, m_Sort, m_Paging);

            // Lấy ra danh sách thực đơn từ kết quả của repository
            List<Food> foods = GetValueObject(res, "Data");

            // Duyệt danh sách thực đơn lấy ra danh sách sở thích phục vụ cho mỗi thực đơn
            foreach (var item in foods)
            {
                item.FoodAdditions = (List<FoodAddition>)_FoodAdditionRepository.GetDataByFoodId(item.FoodID);
            }

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                // Tạo sheet export dữ liệu
                var workSheet = package.Workbook.Worksheets.Add("Danh sách thực đơn");

                // Tự động xuống hàng khi text quá dài
                workSheet.Cells.Style.WrapText = true;

                // Biến lưu giá trị hiển thị header
                List<string> listValueHeader = new List<string>()
                {
                    "Mã món ăn", "Tên món ăn", "Nhóm thực đơn", "Đơn vị tính","Giá bán", "Giá vốn",
                    "Sở thích phục vụ", "Thu thêm"
                };

                // Biến lưu chiều rộng của mỗi cột
                List<double> widthColumns = new List<double>
                {
                    25, 40, 25, 17, 20, 20, 30, 20
                };

                // Tạo tiêu đề
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Merge = true;
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Value = "DANH SÁCH THỰC ĐƠN";
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Style.Font.Name = "Arial";
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Style.Font.Bold = true;
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Style.Font.Size = 16;
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells[1, 1, 1, listValueHeader.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Cells[2, 1, 2, listValueHeader.Count].Merge = true;
                workSheet.Cells[2, 1, 2, listValueHeader.Count].Style.Font.Size = 16;
                workSheet.Cells[3, 1, 3, listValueHeader.Count - 2].Merge = true;
                workSheet.Cells[3, 1, 3, listValueHeader.Count - 2].Value = "Thông tin món ăn";
                workSheet.Cells[3, listValueHeader.Count - 1, 3, listValueHeader.Count].Merge = true;
                workSheet.Cells[3, listValueHeader.Count - 1, 3, listValueHeader.Count].Value = "Sở thích phục vụ";
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Font.Name = "Arial";
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Font.Bold = true;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Font.Size = 12;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                workSheet.Cells[3, 1, 3, listValueHeader.Count].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D8D8D8"));

                int rowHeaderStart = 4;
                // Custom style và gán giá trị cho header
                for (int i = 0; i < listValueHeader.Count; i++)
                {
                    // Custom style
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Font.Name = "Arial";
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Font.Bold = true;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Font.Size = 12;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Cells[rowHeaderStart, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D8D8D8"));

                    // Xét giá trị
                    workSheet.Cells[rowHeaderStart, i + 1].Value = listValueHeader[i];
                    workSheet.Cells[rowHeaderStart, i + 1].AutoFitColumns();
                };

                int rowDataStart = 5; // Dòng bắt đầu điền dữ liệu
                // Điền dữ liệu lấy về vào sheet
                for (int i = 0; i < foods.Count(); i++)
                {
                    // Điền dữ liệu
                    workSheet.Cells[rowDataStart, 1].Value = foods[i].FoodCode;
                    workSheet.Cells[rowDataStart, 2].Value = foods[i].FoodName;
                    workSheet.Cells[rowDataStart, 3].Value = foods[i].FoodCategoryName;
                    workSheet.Cells[rowDataStart, 4].Value = foods[i].FoodUnitName;
                    workSheet.Cells[rowDataStart, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowDataStart, 5].Value = foods[i].FoodPrice.ToString("#,#", CultureInfo.InvariantCulture);
                    workSheet.Cells[rowDataStart, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowDataStart, 6].Value = foods[i].FoodOutwardPrice == null ? "" : Convert.ToDouble(foods[i].FoodOutwardPrice).ToString("#,#", CultureInfo.InvariantCulture);
                    for (int y = 0; y < foods[i].FoodAdditions.Count(); y++)
                    {
                        workSheet.Cells[rowDataStart, 7].Value = foods[i].FoodAdditions[y].FoodAdditionDescription;
                        workSheet.Cells[rowDataStart, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowDataStart, 8].Value = foods[i].FoodAdditions[y].FoodAdditionPrice == null ? "" : Convert.ToDouble(foods[i].FoodAdditions[y].FoodAdditionPrice).ToString("#,#", CultureInfo.InvariantCulture);
                        // Custom style cho tất cả các cell được điền dữ liệu
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Font.Name = "Times New Roman";
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Font.Size = 12;
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        rowDataStart++;
                    }

                    // Custom style cho tất cả các cell được điền dữ liệu
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Font.Name = "Times New Roman";
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Font.Size = 12;
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    workSheet.Cells[rowDataStart, 1, rowDataStart, listValueHeader.Count].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    rowDataStart++;
                    if (foods[i].FoodAdditions.Count() > 0)
                    {
                        rowDataStart--;
                    }
                }

                // Tạo chiều rộng của mỗi cột
                for (int i = 0; i < listValueHeader.Count; i++)
                {
                    workSheet.Column(i + 1).Width = widthColumns[i];
                }

                package.Save();
            }
            stream.Position = 0;
            return stream;
        }
    }
}
