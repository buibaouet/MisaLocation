using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.LocationAPI.Models;
using MISA.LocationV2.BL.Services;
using MISALocationV2.Services.APIKeyAuth;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MISA.LocationV2.Web.LocationAPIControllers
{
    // [Authorize] sau
    [Route("[controller]")]
    [ApiController]
    [APIKeyAuth]

    public class ChangesController : ControllerBase
    {
        private readonly ILocationService _service;

        public ChangesController(ILocationService service)
        {
            this._service = service;
        }

        /// <summary>
        /// Phương thức GetAllChanges trả về danh sách tất cả các thay đổi sau khi đọc từ file
        /// </summary>
        /// <returns>Danh sách các dòng có nội dung được thay đổi</returns>
        /// <remarks></remarks>
        /// Created by nmthang - 27/05/2020
        [HttpGet]
        [Route("")]
        public IEnumerable<LocationChangesModel> GetAllChanges()
        {
            return this._service.GetLocationChanges();
        }

        /// <summary>
        /// Phương thức Checkchanges đọc file excel đã được kéo vào dropzone và kiểm tra các thay đổi
        /// </summary>
        /// <param name="file">File được kéo vào</param>
        /// <returns></returns>
        /// Created by nmthang - 27/05/2020
        /// Modified by nmthang - 18/06/2020
        [HttpPost]
        [Route("")]
        public async Task<bool> Checkchanges(IFormFile file)
        {
            try
            {
                string filePath = await UploadFile(file);
                List<LocationChangesModel> listOfChanges = new List<LocationChangesModel>();
                // upload file and get filepath
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    int colCount = sheet.GetRow(0).LastCellNum;
                    this._service.ReadLocationChanges(sheet, listOfChanges, colCount);
                }
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                // Console.WriteLine(ex.ToString());
                throw;
            }
            // return false;
        }

        /// <summary>
        /// Phương thức private UploadFile thực hiện việc upload file lên từ dropzone
        /// </summary>
        /// <param name="file">File được kéo vào</param>
        /// <returns>Đường dẫn của file sau khi được upload</returns>
        /// Created by nmthang - 27/05/2020
        /// Modified by nmthang - 18/06/2020
        private async Task<string> UploadFile(IFormFile file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (fileExt == ".xlsx" || fileExt == ".xls")
            {
                try
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return filePath;
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }
    }
}