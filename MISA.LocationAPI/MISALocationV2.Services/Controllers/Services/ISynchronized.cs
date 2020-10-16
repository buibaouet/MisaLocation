using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.LocationAPI.Models;
using MISA.LocationV2.Entities.Models.Countries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISALocationV2.Services.Controllers.LocationAPIControllers
{
    public interface ISynchronized
    {
        /// <summary>
        /// Đọc 2 file excel và mapping ID cũ và ID mới của các địa chỉ
        /// </summary>
        /// <param name="fileOld">File Excel của địa chỉ chứa mã ID cũ</param>
        /// <param name="fileNew">File Excel của địa chỉ chứa mã ID mới</param>
        /// <returns></returns>
        /// Created by bvbao (14/10/2020)
        public ArrayList CheckLocationDifferent(IFormFile fileOld, IFormFile fileNew);
        /// <summary>
        ///  Tạo các giá trị cho trường suggestion cho địa chỉ
        /// </summary>
        /// <param name="locationName">tên địa chỉ cần tạo suggestion</param>
        /// <returns></returns>
        /// created by bvbao (14/10/2020)
        public List<string> MakeSuggestionForLocation(string locationName);
        /// <summary>
        /// Xuất danh sách địa chỉ đã đồng bộ mã ra list object Location
        /// </summary>
        /// <returns></returns>
        /// created by bvbao (14/10/2020)
        public List<Location> ExportListLocation();
    }
}
