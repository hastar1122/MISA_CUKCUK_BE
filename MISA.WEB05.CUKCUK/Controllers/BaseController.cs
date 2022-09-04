using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.WEB05.CORE.Common;
using MISA.WEB05.CORE.Exceptions;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Resources;

namespace MISA.WEB05.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseController<MISAEntity> : ControllerBase
    {
        #region Properties
        IBaseService<MISAEntity> _service;
        IBaseRepository<MISAEntity> _repository;
        #endregion

        #region Constructor
        public BaseController(IBaseService<MISAEntity> service, IBaseRepository<MISAEntity> repository)
        {
            _service = service;
            _repository = repository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Controller lấy danh sách toàn bộ bản ghi
        /// </summary>
        /// <returns>Danh sách toàn bộ bản ghi</returns>
        /// Create by: NHANH (5/7/2022)
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Gọi repository lấy danh sách toàn bộ bản ghi
                var data = _repository.Get();

                // Trả lại kết quả cho client
                return Ok(data);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller lấy ra thông tin bản ghi theo khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns>Thông tin bản ghi theo khóa chính</returns>
        /// Created by: NHANH (5/7/2022)
        [HttpGet("{id}")]
        public virtual IActionResult Get(Guid id)
        {
            try
            {
                // Gọi repository lấy thông bản ghi theo khóa chính
                var data = _repository.Get(id);

                // Trả kết quả về cho client
                return Ok(data);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Bản ghi</param>
        /// <returns>Số dòng được thêm mới</returns>
        /// Created by: NHANH (5/7/2022)
        [HttpPost]
        public IActionResult Post(MISAEntity entity)
        {
            try
            {
                // Gọi service thêm mới bản ghi vào database
                var res = _service.InsertService(entity);

                // Trả lại kết quả cho client
                return StatusCode(201, res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="entity">Bản ghi</param>
        /// <returns>Số dòng được cập nhật</returns>
        /// Created by: NHANH (5/7/2022)
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, MISAEntity entity)
        {
            try
            {
                // Gọi service cập nhật thông tin bản ghi trong database
                var res = _service.UpdateService(entity);

                // Trả lại kết quả cho client
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Controller xóa thông tin bản ghi
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns>Số dòng bị xóa</returns>
        /// Created by: NHANH (5/7/2022)
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Gọi repository xóa thông tin bản ghi
                var res = _repository.Delete(id);

                // Trả kết quả về cho client
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Xử lý Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// Updated by: NHANH (5/7/2022)
        protected IActionResult HandleException(Exception ex)
        {
            // Ghi vào log hệ thống 
            // ....
            if (ex is MISAValidateException)
            {
                var res = new
                {
                    devMsg = ex.Message,
                    data = ex.Data,
                    userMsg = ex.Message
                };
                return StatusCode(400, res);
            }
            else
            {
                var res = new
                {
                    devMsg = ex.Message,
                    userMsg = Resource.ResourceManager.GetString($"ErrorException_{Common.LanguageCode}")
                };

                return StatusCode(500, res);
            }
        }
        #endregion
    }
}
