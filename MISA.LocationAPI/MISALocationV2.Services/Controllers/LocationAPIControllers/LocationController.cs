using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MISA.LocationAPI.Constants;
using MISA.LocationAPI.Models;
using MISA.LocationAPI.Response;
using MISA.LocationV2.Entities.Response;
using MISALocationV2.Services.APIKeyAuth;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MISA.LocationV2.Web.LocationAPIControllers
{
    [Route("")]
    [ApiController]
    [APIKeyAuth]
    public class LocationController : Controller
    {
        private readonly IElasticClient _elasticClient;

        public LocationController(IElasticClient elasticClient)
        {
            this._elasticClient = elasticClient;
        }

        #region GetLocByKindAndParentID
        /// <summary>
        /// API trả về các địa chỉ dựa theo Kind và ParentID
        /// </summary>
        /// <param name="kind">Cấp của các địa chỉ cần trả về</param>
        /// <param name="parentID">Mã cha của các địa chỉ</param>
        /// <returns>Các địa chỉ tìm cùng với status trả về</returns>
        /// Created by nmthang 19/05/2020
        /// Modified by nmthang 09/06/2020
        [HttpGet("loc")]
        public async Task<BaseResponse> GetLocByKindAndParentID(string kind, string parentID)
        {
            // kiem tra kind co hop le
            int kindNum = ValidateKind(kind);
            if (kindNum < 0 || (kindNum > 0 && string.IsNullOrEmpty(parentID)))
            {
                return new ErrorResponse(ConstMessages.InvalidInputDataCode, ConstMessages.InvalidInputDataMsg);
            }
            parentID = (parentID != null) ? parentID.Trim() : "";

            // response
            var response = await this._elasticClient.SearchAsync<Location>(s => s
                .From(0).Size(300)
                .Query(q => q.Term(l => l.Kind, kindNum)
                    && q.Term(l => l.ParentID.Suffix("keyword"), parentID))
                .Source(sf => sf.Includes(i => i.Fields(f => f.Kind, f => f.LocationName, f => f.ID, f => f.LocationID, f => f.CreatedDate, f => f.ModifiedDate)))
                );
            
            if (!response.IsValid)
            {
                return new ErrorResponse(); // lay ra thong tin khi thuc hien query
            }
            var locations = response.Documents.Select(l => JObject.FromObject(
                new
                {
                    l.Kind,
                    l.LocationName,
                    l.ID,
                    l.LocationID,
                    l.CreatedDate,
                    l.ModifiedDate
                }));

            if (locations.Any())
            {
                return new SuccessResponse(locations);
            }

            return new ErrorResponse(ConstMessages.DataNotFoundCode, ConstMessages.DataNotFoundMsg);
            
        }
        #endregion

        #region Get Location By ID
        /// <summary>
        /// API trả về tên địa chỉ theo ID
        /// </summary>
        /// <param name="id">ID của địa chỉ</param>
        /// <returns>Địa chỉ cần tìm kèm theo mã trả về</returns>
        /// Created by nmthang - 19/05/2020
        /// Modified by nmthang 16/06/2020
        [HttpGet("loc-name")]
        public async Task<BaseResponse> GetLocByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ErrorResponse(ConstMessages.InvalidInputDataCode, ConstMessages.InvalidInputDataMsg);
            }

            var response = await this._elasticClient.SearchAsync<Location>(s => s
                .Query(q => q.Term(l => l.ID.Suffix("keyword"), id)) //Dùng lệnh này nếu trường ID có keyword
                .Source(sf => sf.Includes(i => i.Fields(f => f.ID, f => f.LocationName)))
            ); // voi Elasticsearch duoi v7.0 thi them TypedKeys(null) vao cuoi truy van
            if (!response.IsValid)
            {
                return new ErrorResponse(ConstMessages.QueryFailedCode, ConstMessages.QueryFailedMsg);
            }
            var location = response.Documents.Select(l => new
            {
                l.ID,
                l.LocationName
            }).FirstOrDefault();

            if (location != null)
            {
                return new SuccessResponse(location, true);
            }

            return new ErrorResponse(ConstMessages.DataNotFoundCode, ConstMessages.DataNotFoundMsg);
        }
        #endregion

        /// <summary>
        /// Phương thức phụ kiểm tra kind nhập vào trên url có hợp lệ
        /// </summary>
        /// <param name="kindStr"></param>
        /// <returns></returns>
        public static int ValidateKind(string kindStr)
        {
            bool isNum = int.TryParse(kindStr, out int kindNum);
            return (isNum && -1 < kindNum && kindNum < 4) ? kindNum : -1;
        }
    }
}