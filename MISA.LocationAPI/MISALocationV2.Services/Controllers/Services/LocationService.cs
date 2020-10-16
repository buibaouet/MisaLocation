using MISA.LocationAPI.Models;
using MISA.LocationV2.Web.LocationAPIControllers;
using Nest;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MISA.LocationV2.BL.Services
{
    public class LocationService : ILocationService
    {
        private readonly IElasticClient _elasticClient;

        public LocationService(IElasticClient elasticClient)
        {
            this._elasticClient = elasticClient;
        }
        #region Fields
        private IEnumerable<LocationChangesModel> list = new List<LocationChangesModel>();
        #endregion

        #region Overriden methods
        /// <summary>
        /// Phương thức GetLocationChanges trả về tất cả các thay đổi sau khi đọc file
        /// </summary>
        /// <returns>Danh sách chứa các thông tin về các địa chỉ đã có thay đổi</returns>
        public IEnumerable<LocationChangesModel> GetLocationChanges()
        {
            return this.list;
        }

        /// <summary>
        /// Phương thức ReadLocationChanges đọc file và lấy ra những địa chỉ có thay đổi
        /// </summary>
        /// <param name="sheet">Sheet hiện tại đang làm việc</param>
        /// <param name="listParam">Một danh sách để chứa những thông tin về địa chỉ thay đỏi</param>
        /// <param name="colCount">Số trường thông tin trong sheet (số cột)</param>
        /// Created by nmthang - 31/05/2020
        /// Modified by nmthang - 01/06/2020
        public void ReadLocationChanges(ISheet sheet, List<LocationChangesModel> listParam, int colCount)
        {
            int newNameIdx = (colCount - 3) / 2; // biến lưu vị trí tên địa chỉ mới
            int oldNameIdx = (colCount - 1) / 2; // biến lưu vị trí tên địa chỉ cũ
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                // GetCell có thể không trả về đúng thứ tự của cell vì NPOI có thể skip các blank cell
                // tùy chọn MissingCellPolicy.RETURN_BLANK_AS_NULL trả các blank cell về null, đảm bảo thứ tự của các cell trong một row
                var curRow = sheet.GetRow(i);
                var newNameCell = curRow.GetCell(newNameIdx, MissingCellPolicy.RETURN_BLANK_AS_NULL);
                var oldNameCell = curRow.GetCell(oldNameIdx, MissingCellPolicy.RETURN_BLANK_AS_NULL);

                // xử lý trường hợp một địa chỉ bị xóa
                if (newNameCell == null)
                {
                    HandleDeletedLocation(listParam, curRow, colCount);
                }
                else if (oldNameCell == null)
                {
                    HandleInsertedLocation(listParam, curRow, colCount);
                }
                else
                {
                    HandleUpdatedLocation(listParam, curRow, colCount);
                }
            }
            this.list = listParam;
        }

        /// <summary>
        /// Phương thức IsChangedRow kiểm tra xem một dòng có thay đổi nào hay không
        /// </summary>
        /// <param name="row">Dòng cần đọc</param>
        /// <param name="comparePos">Vị trí bắt đầu dùng để đối chiếu</param>
        /// <param name="colCount">Số cột trong sheet</param>
        /// <returns>True nếu có thay đổi trong dòng, ngược lại trả về false</returns>
        public bool IsChangedRow(IRow row, int comparePos, int colCount)
        {
            bool hasChanged = false;
            for (int j = comparePos; j >= 1; j -= 2)
            {
                if (row.GetCell(j).StringCellValue != row.GetCell(colCount - 2 - j).StringCellValue)
                {
                    hasChanged = true;
                    break;
                }
            }
            return hasChanged;
        }

        /// <summary>
        /// Phương thức IsExistedID kiểm tra xem một ID đã tồn tại trong database hay chưa
        /// </summary>
        /// <param name="id">ID cần kiểm tra</param>
        /// <returns>True nếu đã tồn tại, false nếu chưa tồn tại</returns>
        /// Created by nmthang - 25/06/2020
        public bool IsExistedID(string id)
        {
            try
            {
                var locationCon = new LocationController(_elasticClient);

                var checkID = locationCon.GetLocByID(id);


                if (!String.IsNullOrEmpty(checkID.Result.Data.ToString()))
                {
                    dynamic responseObj = JsonConvert.DeserializeObject(checkID.Result.Data.ToString());
                    if (responseObj != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }


        /// <summary>
        /// Hàm kiểm tra Location ID đã tồn tại hay chưa
        /// </summary>
        /// <param name="locationId">LocationID cần kiểm tra</param>
        /// <returns>Trả về true nếu đã tồn tại</returns>
        /// Created by bvbao (19/8/2020)
        public async Task<bool> IsExistedLocationIDAsync(string locationId)
        {
            var response = await this._elasticClient.SearchAsync<Location>(s => s
                .Query(q => q.Term(f => f.LocationID.Suffix("keyword"), locationId))
                .Source(sf => sf.Includes(i => i.Fields(f => f.LocationID, f => f.LocationName))));
            if (!response.IsValid)
            {
                return false;
            }
            var location = response.Documents.Select(l => new
            {
                l.LocationID,
                l.LocationName
            }).FirstOrDefault();

            if (location != null)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Phương thức xử lý dòng thể hiện địa chỉ bị xóa
        /// </summary>
        /// <param name="listParam">Danh sách các thay đổi</param>
        /// <param name="curRow">Row hiện tại</param>
        /// <param name="colCount">Số cột trong sheet</param>
        /// created by nmthang (17/06/2020)
        private void HandleDeletedLocation(List<LocationChangesModel> listParam, IRow curRow, int colCount)
        {
            int oldNameIdx = (colCount - 1) / 2;
            var oldID = new StringBuilder("VN");
            for (int j = colCount - 2; j >= oldNameIdx + 1; j -= 2)
            {
                oldID.Append(curRow.GetCell(j).StringCellValue);
            }
            var newChange = new LocationChangesModel
            {
                OldLocName = curRow.GetCell(oldNameIdx).StringCellValue,
                OldID = oldID.ToString(),
                OldDistrict = curRow.GetCell(colCount - 5).StringCellValue,
                OldProvince = curRow.GetCell(colCount - 3).StringCellValue,
                Action = "DELETE"
            };
            listParam.Add(newChange);
        }

        /// <summary>
        /// PHương thức xử lý dòng thể hiện địa chỉ được thêm mới
        /// </summary>
        /// <param name="listParam">Danh sách các thay đổi</param>
        /// <param name="curRow">Row hiện tại</param>
        /// <param name="colCount">Số cột trong sheet</param>
        private void HandleInsertedLocation(List<LocationChangesModel> listParam, IRow curRow, int colCount)
        {
            int newNameIdx = (colCount - 3) / 2;
            var newID = new StringBuilder("VN");
            for (int j = 0; j <= newNameIdx - 1; j += 2)
            {
                newID.Append(curRow.GetCell(j).StringCellValue);
            }
            var newChange = new LocationChangesModel
            {
                NewLocName = curRow.GetCell(newNameIdx).StringCellValue,
                NewID = newID.ToString(),
                NewDistrict = curRow.GetCell(3).StringCellValue,
                NewProvince = curRow.GetCell(1).StringCellValue,
                Action = "INSERT"
            };
            listParam.Add(newChange);
        }

        /// <summary>
        /// Phương thức xử lý dòng thể hiện địa chỉ được cập nhật
        /// </summary>
        /// <param name="listParam">Danh sách các thay đổi</param>
        /// <param name="curRow">Row hiện tại</param>
        /// <param name="colCount">Số cột trong sheet</param>
        private void HandleUpdatedLocation(List<LocationChangesModel> listParam, IRow curRow, int colCount)
        {
            int oldNameIdx = (colCount - 1) / 2;
            int newNameIdx = (colCount - 3) / 2;
            string countryID = "VN";
            if (IsChangedRow(curRow, newNameIdx, colCount))
            {
                var oldID = new StringBuilder(countryID);
                for (int j = colCount - 2; j >= oldNameIdx + 1; j -= 2)
                {
                    oldID.Append(curRow.GetCell(j).StringCellValue);
                }
                var newID = new StringBuilder(countryID);
                for (int j = 0; j <= newNameIdx - 1; j += 2)
                {
                    newID.Append(curRow.GetCell(j).StringCellValue);
                }
                var newChange = new LocationChangesModel
                {
                    NewProvince = curRow.GetCell(1).StringCellValue,
                    NewDistrict = curRow.GetCell(3).StringCellValue,
                    NewID = newID.ToString(),
                    NewLocName = curRow.GetCell(newNameIdx).StringCellValue,
                    OldLocName = curRow.GetCell(oldNameIdx).StringCellValue,
                    OldID = oldID.ToString(),
                    OldDistrict = curRow.GetCell(colCount - 5).StringCellValue,
                    OldProvince = curRow.GetCell(colCount - 3).StringCellValue,
                    Action = "UPDATE"
                };
                listParam.Add(newChange);
            }
        }
        #endregion

    }
}
