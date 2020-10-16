using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MISA.LocationAPI.Response;
using MISA.LocationV2.Entities.Response;
using MISALocationV2.Services.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.BL.Healthz
{
    [Route("[controller]")]
    [ApiController]
    public class HealthzController : ControllerBase
    {
        public readonly IConfiguration configuration;

        public HealthzController(IConfiguration _config)
        {
            configuration = _config;
        }

        /// <summary>
        /// API trả về thông tin bản version của app
        /// </summary>
        /// <returns></returns>
        /// Created by bvbao(1/9/2020)
        [HttpGet]
        public BaseResponse getVersionApplication()
        {
            var version = configuration["healthz:version"];
            var description = configuration["healthz:description"];
            var maintainer = configuration["healthz:maintainer"];
            var data = new DataResponse()
            {
                version = version,
                description = description,
                maintainer = maintainer
            };

            BaseResponse response = new BaseResponse()
            {
                Code = 200,
                Message = Resources.HealthzMeg,
                Status = Resources.HeathzStatus,
                Data = data
            };

            return response;
        }

        public class DataResponse
        {
            public string version { get; set; }
            public string description { get; set; }
            public string maintainer { get; set; }
        }
    }
}
