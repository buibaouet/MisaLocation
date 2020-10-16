using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISALocationV2.Services.Properties;
using Nest;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MISALocationV2.Services.Controllers.LocationAPIControllers
{
    [Route("[controller]")]
    [ApiController]
    public class SynchronizedController : ControllerBase
    {
        private readonly ISynchronized _synchronized;
        private readonly IElasticClient _elasticClient;

        public SynchronizedController(ISynchronized synchronized, IElasticClient elasticClient)
        {
            _synchronized = synchronized;
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// API trả về danh sách các địa chỉ đã được đồng bộ mã và danh sách các địa chỉ khác nhau
        /// </summary>
        /// <returns></returns>
        /// Created by bvbao (21/7/2020)
        /// Modified by bvbao (14/10/2020)
        [HttpPost]
        [Route("loc-synchronized")]
        public ArrayList CheckLocationDifferent()
        {
            IFormFile fileOld = Request.Form.Files[0];
            IFormFile fileNew = Request.Form.Files[1];
            return _synchronized.CheckLocationDifferent(fileOld, fileNew);
        }

        /// <summary>
        /// API trả về danh sách toàn bộ dữ liệu địa chỉ đã hợp nhất ID cũ mới dưới dạng file excel
        /// </summary>
        /// <returns>file excel chứa dữ liệu toàn bộ địa chỉ</returns>
        /// Midified by bvbao (14/20/2020)
        [HttpGet]
        [Route("loc-downloadexcel")]
        public async Task<IActionResult> DownloadLocationExcel()
        {
            await Task.Yield();
            var location = _synchronized.ExportListLocation();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add(Resources.SheetName);

                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                //Set độ rộng cho các cột
                workSheet.Column(1).Width = 10;//STT
                workSheet.Column(2).Width = 20;//ID
                workSheet.Column(3).Width = 20;//ID
                workSheet.Column(4).Width = 35;//Name
                workSheet.Column(5).Width = 20;//Kind
                workSheet.Column(6).Width = 20;//ParentID 

                //Đặt tên cho header
                workSheet.Cells[1, 1].Value = Resources.STT;
                workSheet.Cells[1, 2].Value = Resources.Header_LocID;
                workSheet.Cells[1, 3].Value = Resources.Header_ID;
                workSheet.Cells[1, 4].Value = Resources.Header_Name;
                workSheet.Cells[1, 5].Value = Resources.Header_Kind;
                workSheet.Cells[1, 6].Value = Resources.Header_ParentID;

                //Điền các dữ liệu từ list vào các row
                int recordIndex = 2;
                foreach (var loc in location)
                {
                    workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                    workSheet.Cells[recordIndex, 2].Value = loc.LocationID;
                    workSheet.Cells[recordIndex, 3].Value = loc.ID;
                    workSheet.Cells[recordIndex, 4].Value = loc.LocationName;
                    workSheet.Cells[recordIndex, 5].Value = (loc.Kind == 0) ? Resources.Country 
                        : (loc.Kind == 1) ? Resources.Privince 
                        : (loc.Kind == 2) ? Resources.District 
                        : Resources.Ward;
                    workSheet.Cells[recordIndex, 6].Value = loc.ParentID;
                    recordIndex++;
                }

                package.Save();
            }
            stream.Position = 0;
            string excelName = Resources.DownloadName + ".xlsx";

            //return File(stream, "application/octet-stream", excelName);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        /// <summary>
        /// API trả về danh sách toàn bộ dữ liệu địa chỉ đã hợp nhất ID cũ mới dưới dạng file json
        /// </summary>
        /// <returns>file json chứa dữ liệu toàn bộ địa chỉ</returns>
        /// Created by bvbao (30/7/2020)
        /// Midified by bvbao (14/20/2020)
        [HttpGet]
        [Route("loc-downloadjson")]
        public FileContentResult DownloadLocationJson()
        {
            var location = _synchronized.ExportListLocation();

            byte[] fileContent = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(location, Formatting.Indented)
                );
            string jsonName = Resources.DownloadName + ".json";
            return File(fileContent, "application/json", jsonName);
        }

        #region Update data to Elasticsearch 
        /// <summary>
        /// Update dữ liệu lên databse Elasticsearch 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// Created by bvbao (8/3/2020)
        [HttpPost]
        [Route("loc-updatedb")]
        public async Task<IActionResult> UpdateDataToElasticsearch()
        {
            var location = _synchronized.ExportListLocation();

            var indexManyAsyncResponse = await _elasticClient.IndexManyAsync(location);

            if (!indexManyAsyncResponse.IsValid)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
        #endregion

        #region Suggestion of location
        /// <summary>
        /// Tạo các giá trị cho trường suggestion cho địa chỉ
        /// </summary>
        /// <param name="locationName">tên địa chỉ cần tạo suggestion</param>
        /// <returns></returns>
        /// created by bvbao (31/7/2020)
        [HttpGet]
        [Route("loc-suggestion")]
        public List<string> MakeSuggestionForLocation(string locationName)
        {
            return _synchronized.MakeSuggestionForLocation(locationName);
        }
        #endregion
    }
}
