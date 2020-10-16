using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Entities.Response
{
    public class BaseResponse
    {
        #region Properties
        public int Code { get; set; }
        public Object Data { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        #endregion

        #region Constructor 
        public BaseResponse()
        {
        }
        #endregion
    }
}
