using MISA.LocationV2.Entities.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Response
{
    /// <summary>
    /// Class tạo ra success response
    /// Created by nmthang
    /// Modified by nmthang (16/06/2020)
    /// </summary>
    public class SuccessResponse : BaseResponse
    {
        #region Constructor
        public SuccessResponse() : base()
        {
            Code = 200;
            Message = "Query success!";
            Status = "OK";
        }

        /// <summary>
        /// Constructor tạo success response dựa trên đối tượng trả về
        /// </summary>
        /// <param name="data">Đối tượng trả về</param>
        /// <param name="isSingleObject">Đối tượng trả về có phải là một đối tượng đơn không</param>
        public SuccessResponse(Object data, bool isSingleObject = false) : this()
        {
            if (isSingleObject)
            {
                Data = JObject.FromObject(data);
            }
            else Data = data;
        }
        #endregion
    }
}
