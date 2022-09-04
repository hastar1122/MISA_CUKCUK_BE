using MISA.WEB05.CORE.Attributes;
using MISA.WEB05.CORE.Enum;
using MISA.WEB05.CORE.Exceptions;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.WEB05.CORE.Services
{
    public class BaseService<MISAEntity> : IBaseService<MISAEntity>
    {
        #region Properties
        IBaseRepository<MISAEntity> _repository;
        protected List<string> ErrorValidateMsgs;
        protected bool IsValid = true;
        #endregion

        #region Constructor
        public BaseService(IBaseRepository<MISAEntity> repository)
        {
            _repository = repository;
            ErrorValidateMsgs = new List<string>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Thực hiện thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thực thể được thêm mới</param>
        /// <returns>Khóa chính của bản ghi được thêm mới</returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: NHANH (4/7/2022)
        public virtual Guid InsertService(MISAEntity entity)
        {
            /// Validate dữ liệu
            CheckProPertiesNotAllowedNull(entity);
            CheckPropertiesNotAllowedDuplicate(entity, null);

            /// Thực hiện thêm mới
            if (IsValid == true)
            {
                return _repository.Insert(entity, null);
            }
            else
            {
                throw new MISAValidateException(ErrorValidateMsgs[0], ErrorValidateMsgs);
            }
        }

        /// <summary>
        /// Thực hiện cập nhật bản ghi
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns></returns>
        /// <exception cref="MISAValidateException"></exception>
        /// Created by: NHANH (4/7/2022)
        public virtual int UpdateService(MISAEntity entity)
        {
            /// Validate dữ liệu
            CheckProPertiesNotAllowedNull(entity);
            CheckPropertiesNotAllowedDuplicate(entity, null);

            /// Thực hiện cập nhật
            if (IsValid == true)
            {
                return _repository.Update(entity, null);
            }
            else
            {
                throw new MISAValidateException(ErrorValidateMsgs[0], ErrorValidateMsgs);
            }
        }

        /// <summary>
        /// Hàm kiểm tra những thuộc tính không được phép để trống
        /// </summary>
        /// <param name="entity">Thực thể cần kiểm tra</param>
        /// Created by: NHANH (4/8/2022)
        public virtual void CheckProPertiesNotAllowedNull<T>(T entity)
        {
            // Lấy ra những properties không được phép để trống
            var PropertiesNotAllowedNull = entity?.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(NotAllowedNull))
            );

            foreach (var prop in PropertiesNotAllowedNull)
            {
                if (prop.GetValue(entity) == null || String.IsNullOrWhiteSpace(prop.GetValue(entity).ToString()))
                {
                    IsValid = false;
                    var propName = prop.GetCustomAttributes(typeof(PropsName), true);
                    if (propName.Length > 0)
                    {
                        ErrorValidateMsgs.Add(String.Format(Resources.Validate.NotAllowedNull, ((PropsName)propName[0]).Name));
                    }
                    else
                    {
                        ErrorValidateMsgs.Add(String.Format(Resources.Validate.WarningMissingInformation));
                    }
                    //try
                    //{
                    //    var propName = (PropsName)prop.GetCustomAttributes(typeof(PropsName), true)[0];
                    //    ErrorValidateMsgs.Add(String.Format(Resources.Validate.NotAllowedNull, propName.Name));
                    //}
                    //catch
                    //{
                    //    ErrorValidateMsgs.Add(String.Format(Resources.Validate.WarningMissingInformation));
                    //}
                }
            }
        }

        /// <summary>
        /// Hàm kiểm tra những thuộc tính không được phép trùng dữ liệu
        /// </summary>
        /// <param name="entity">Thực thể cần kiểm tra</param>
        /// <param name="ID">Nếu khác null nghĩa là check trùng cho update</param>
        /// Created by: NHANH (4/8/2022)
        public virtual void CheckPropertiesNotAllowedDuplicate(MISAEntity entity, Guid? ID)
        {
            // Lấy ra những properties không được phép trùng
            var PropertiesNotAllowedDuplicate= entity?.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(NotAllowedDuplicate))
            );

            foreach (var prop in PropertiesNotAllowedDuplicate)
            {
                // Nếu có dữ liệu và kiểm tra bị trùng
                if (prop.GetValue(entity) != null && _repository.CheckDuplicate(entity, prop.Name, ID) == true)
                {
                    IsValid = false;
                    try
                    {
                        var propName = (PropsName)prop.GetCustomAttributes(typeof(PropsName), true)[0];
                        ErrorValidateMsgs.Add(String.Format(Resources.Validate.NotAllowedDuplicate, propName.Name, prop.GetValue(entity)));
                    }
                    catch
                    {
                        ErrorValidateMsgs.Add(String.Format(Resources.Validate.WarningDuplicateInformation));
                    }
                }
            }
        }
        #endregion
    }
}
