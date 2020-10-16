using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.LocationAPI.Constants;
using MISA.LocationAPI.Models;
using MISA.LocationAPI.Response;
using MISA.LocationV2.BL.Services;
using MISA.LocationV2.Entities.Response;
using MISA.LocationV2.Webhook.Interface;
using MISALocationV2.Services.APIKeyAuth;
using MISALocationV2.Services.Properties;
using Nest;

namespace MISA.LocationV2.Web.LocationAPIControllers
{
    [Route("[controller]")]
    [ApiController]
    [APIKeyAuth]
    public class SettingsController : ControllerBase
    {
        private readonly ILocationService _service;
        private readonly IElasticClient _elasticClient;
        private readonly IWebhookSender _webhookSender;

        public SettingsController(ILocationService service, IElasticClient elasticClient, IWebhookSender webhookSender)
        {
            _service = service;
            _elasticClient = elasticClient;
            _webhookSender = webhookSender;
        }

        [HttpGet]
        [Route("exist/{id}")]
        public bool CheckDuplicateID(string id)
        {
            return this._service.IsExistedID(id); 
        }

        /// <summary>
        /// Hàm kiểm tra Location ID đã tồn tại hay chưa
        /// </summary>
        /// <param name="locationId">LocationID cần kiểm tra</param>
        /// <returns>Trả về true nếu đã tồn tại</returns>
        /// Created by bvbao (19/8/2020)
        [HttpGet]
        [Route("exist/locationid/{locationId}")]
        public async Task<bool> CheckDuplicateLocationIDAsync(string locationId)
        {
            return await _service.IsExistedLocationIDAsync(locationId);
        }

        #region InsertNewLocation
        /// <summary>
        /// API thực hiện thêm địa chỉ mới vào DB
        /// </summary>
        /// <returns>Trả về thông báo nếu thành công hoặc mã lỗi nếu thất bại</returns>
        /// Created by bvbao 10/6/2020
        [Route("{locationID}")]
        [HttpPost]
        public async Task<BaseResponse> InsertNewLocation(string locationID, [FromBody] Location location)
        {
            try
            {
                // response
                var response = await _elasticClient.IndexDocumentAsync(location);

                if (!response.IsValid)
                {
                    return new ErrorResponse(ConstMessages.QueryFailedCode, ConstMessages.QueryFailedMsg);
                }
                else
                {
                    _webhookSender.WebhookStorage(Resources.Insert, location);
                    return new SuccessResponse(ConstMessages.InsertSuccess);
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ConstMessages.UnidentifiedErrorCode, ex.ToString());
            }

        }
        #endregion

        #region UpdateLocation
        /// <summary>
        /// API thực hiện chỉnh sửa địa chỉ theo yêu cầu vào DB
        /// </summary>
        /// <returns>Trả về thông báo nếu thành công hoặc mã lỗi nếu thất bại</returns>
        /// Created by bvbao 10/6/2020
        [Route("{locationID}")]
        [HttpPut]
        public async Task<BaseResponse> UpdateLocation(string locationID, [FromBody] Location location)
        {
            try
            {
                var response = await _elasticClient.UpdateByQueryAsync<Location>(u => u
                    .Query(q => q
                        .Term(f => f.ID.Suffix("keyword"), locationID)
                    )
                    .Script(s => s
                    .Source("ctx._source.LocationName = params.val.LocationName;" +
                    "ctx._source.ID = params.val.ID;" +
                    "ctx._source.LocationID = params.val.LocationID;" +
                    "ctx._source.ModifiedDate = params.val.ModifiedDate;" +
                    "ctx._source.ModifiedBy = params.val.ModifiedBy;" +
                    "ctx._source.CountryID = params.val.CountryID;" +
                    "ctx._source.PID = params.val.PID;" +
                    "ctx._source.ProvinceID = params.val.ProvinceID;" +
                    "ctx._source.DID = params.val.DID;" +
                    "ctx._source.DistrictID = params.val.DistrictID;" +
                    "ctx._source.PostalCode = params.val.PostalCode;" +
                    "ctx._source.Suggestion = params.val.Suggestion;" +
                    "ctx._source.FullAddress = params.val.FullAddress;")
                    .Lang("painless")
                    .Params(p => p.Add("val", location)))
                    .Conflicts(Conflicts.Proceed)
                    .Refresh(true)
                );

                if (!response.IsValid)
                {
                    return new ErrorResponse(ConstMessages.QueryFailedCode, ConstMessages.QueryFailedMsg);
                }
                else
                {
                    _webhookSender.WebhookStorage(Resources.Update, location);
                    return new SuccessResponse(ConstMessages.UpdateSuccess);
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ConstMessages.UnidentifiedErrorCode, ex.ToString());
            }
        }
        #endregion

        #region DeleteLocation
        /// <summary>
        /// API thực hiện xóa địa chỉ theo yêu cầu trong DB
        /// </summary>
        /// <returns>Trả về thông báo nếu thành công hoặc mã lỗi nếu thất bại</returns>
        /// Created by bvbao 10/6/2020
        [Route("{locationID}")]
        [HttpDelete]
        public async Task<BaseResponse> DeleteLocation(string locationID)
        {
            var location = GetLocationByID(locationID);
            try
            {
                var response = await _elasticClient.DeleteByQueryAsync<Location>(s => s
                    .Query(q => q.Term(f => f.ID.Suffix("keyword"), locationID)
                    )
                );

                if (!response.IsValid)
                {
                    return new ErrorResponse(ConstMessages.QueryFailedCode, ConstMessages.QueryFailedMsg);
                }
                else
                {
                    _webhookSender.WebhookStorage(Resources.Delete, location);
                    return new SuccessResponse(ConstMessages.DeleteSuccess);
                }
            }
            catch (Exception ex)
            {
                return new ErrorResponse(ConstMessages.UnidentifiedErrorCode, ex.ToString());
            }
        }
        #endregion

        /// <summary>
        /// Lấy tất cả toàn bộ thông tin của địa chỉ
        /// </summary>
        /// <param name="id">ID của địa chỉ</param>
        /// <returns></returns>
        /// Created by bvbao(13/8/2020)
        public Location GetLocationByID(string id)
        {
            var response = _elasticClient.Search<Location>(s => s
                .Query(q => q.Term(f => f.ID.Suffix("keyword"), id))
            ); // voi Elasticsearch duoi v7.0 thi them TypedKeys(null) vao cuoi truy van

            var location = response.Documents.FirstOrDefault();

            return location;
        }
    }
}