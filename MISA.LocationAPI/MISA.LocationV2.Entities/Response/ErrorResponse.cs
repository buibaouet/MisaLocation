using MISA.LocationV2.Entities.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Response
{
    /// <summary>
    /// Class tạo ra các response trả về lỗi
    /// </summary>
    public class ErrorResponse : BaseResponse
    {
        #region Constructor
        public ErrorResponse() : base()
        {
            Data = "";
            Status = "Failed!";
        }
        public ErrorResponse(int code, string message) : this()
        {
            Code = code;
            Message = message;
        }
        #endregion
    }
}
