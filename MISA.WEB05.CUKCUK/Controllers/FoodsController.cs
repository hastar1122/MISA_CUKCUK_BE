using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Models;

namespace MISA.WEB05.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FoodsController : BaseController<Food>
    {
        IFoodRepository _Repository;
        IFoodService _Service;

        public FoodsController(IFoodService service, IFoodRepository repository) : base(service, repository)
        {
            _Repository = repository;
            _Service = service;
        }

        /// <summary>
        /// Controller lọc danh sách thực đơn và phân trang
        /// </summary>
        /// <param name="filter">Thông tin filter và phân trang</param>
        /// <returns>Tổng số trang, các bản ghi tìm được, tổng số bản ghi</returns>
        /// Created by: NHANH (15/8/2022)
        [HttpPost("filter")]
        public IActionResult Get(Filter filter)
        {
            try
            {
                // Gọi service lọc và phân trang danh sách thực đơn
                var res = _Service.FilterFood(filter);

                // Trả kết quả về cho client
                return Ok(res);
            }
            catch (Exception ex)
            {
                return base.HandleException(ex);
            }
        }

        /// <summary>
        /// Controller lấy mã thực đơn 
        /// </summary>
        /// <param name="FoodName">Tên thực đơn</param>
        /// <returns>Mã thực đơn</returns>
        /// Created by: NHANH (23/8/2022)
        [HttpGet("getFoodCode")]
        public IActionResult Get(String foodName)
        {
            try
            {
                // Gọi service tạo mã thực đơn
                var res = _Service.GetFoodCode(foodName);

                // Trả kết quả về cho client
                return Ok(res);
            }
            catch (Exception ex)
            {
                return base.HandleException(ex);
            }
        }

        /// <summary>
        /// Controller xóa danh sách thực đơn
        /// </summary>
        /// <param name="foodIDs">Danh sách khóa chính</param>
        /// <returns></returns>
        /// Created by: NHANH (24/8/2022)
        [HttpDelete("deleteMultiple")]
        public IActionResult DeleteRange(string foodIDs)
        {
            try
            {
                // Gọi repository xóa danh sách thực đơn
                var res = _Repository.DeleteMultiple(foodIDs, null);

                // Trả lại kết quả cho client
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller upload ảnh
        /// </summary>
        /// <param name="fileImage">file ảnh</param>
        /// <returns>tên ảnh được mã hóa</returns>
        /// Created by: NHANH (27/8/2022)
        [HttpPost("uploadImage")]
        public IActionResult UploadImage(IFormFile fileImage)
        {
            try
            {
                // Gọi service upload ảnh
                var res = _Service.UploadImage(fileImage);

                // Trả lại kết quả cho client
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller xuất ra file excel danh sách thực đơn
        /// </summary>
        /// <param name="filter">Thông tin lọc</param>
        /// <returns></returns>
        /// Created by: NHANH (29/8/2022)
        [HttpPost("export")]
        public IActionResult Export(Filter filter)
        {
            try
            {
                // Gọi service xuất file excel theo dữ liệu lọc
                var data = _Service.Export(filter);

                // Trả kết quả về cho client
                return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Danh sách thực đơn");
            }
            catch (Exception ex)
            {
                return base.HandleException(ex);
            }
        }
    }
}
