using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using MISA.LocationAPI.Models;
using MISA.LocationV2.Entities.Models.Countries;
using MISALocationV2.Services.Properties;
using NPOI.SS.UserModel;

namespace MISALocationV2.Services.Controllers.LocationAPIControllers
{
    public class Synchronized : ISynchronized
    {
        private List<Province> listLocationSync = new List<Province>();

        #region Check Location Different and mapping ID 
        /// <summary>
        /// Kiểm tra tất cả các dữ liệu giống và khác nhau từ 2 list dữ liệu cũ và mới
        /// </summary>
        /// <returns>Danh sách tất cả dữ liệu trùng nhau và khác nhau</returns>
        /// Created by bvbao (21/7/2020)

        public ArrayList CheckLocationDifferent(IFormFile fileOld, IFormFile fileNew)
        {
            //Lấy 2 file excel
            //Đọc 2 file excel và lấy data
            var listSynch = ReadNewLocationExcelFile(fileNew);
            var listOld = ReadOldLocationExcelFile(fileOld);
            var listDuplication = new List<LocationChangesModel>();
            var listDifferent = new List<LocationChangesModel>();

            foreach (LocationChangesModel location in listOld)
            {
                var expiredString = Resources.ExpridMeg;
                if (location.OldLocName.Contains(expiredString) || location.OldDistrict.Contains(expiredString))
                {
                    listDifferent.Add(location);
                }
                else
                {
                    bool check = false;
                    foreach (Province province in listSynch)
                    {
                        if (RemoveVietnameseTone(province.ProvinceName).Contains(RemoveVietnameseTone(location.OldProvince)))
                        {
                            if (province.ProvinceIDCode == "")
                            {
                                province.ProvinceIDCode = location.OldID.Substring(0, 5);
                                province.ProvinceName = location.OldProvince;
                            }
                            foreach (District district in province.districts)
                            {
                                if (RemoveVietnameseTone(district.DistrictName).Equals(RemoveVietnameseTone(location.OldDistrict)))
                                {
                                    if (district.DistrictIDCode == "")
                                    {
                                        district.DistrictIDCode = location.OldID.Substring(0, 7);
                                    }
                                    foreach (Ward ward in district.wards)
                                    {
                                        if (RemoveVietnameseTone(ward.WardName).Equals(RemoveVietnameseTone(location.OldLocName)))
                                        {
                                            //Map 2 ID cũ và mới nếu trùng địa chỉ
                                            check = true;
                                            ward.WardIDCode = location.OldID;
                                            location.NewID = ward.WardID;
                                            location.NewLocName = ward.WardName;
                                            location.NewDistrict = district.DistrictName;
                                            location.NewProvince = province.ProvinceName;
                                            listDuplication.Add(location);
                                            break;
                                        }
                                    }
                                    if (!check)
                                    {
                                        check = true;
                                        listDifferent.Add(location);
                                    }
                                    break;
                                }
                            }
                            if (!check)
                            {
                                check = true;
                                listDifferent.Add(location);
                            }
                            break;
                        }
                    }
                    if (!check)
                    {
                        listDifferent.Add(location);
                    }
                }
            }
            ArrayList response = new ArrayList();
            response.Add(listDuplication);
            response.Add(listDifferent);
            response.Add(listSynch);
            this.listLocationSync = listSynch;
            return response;
        }
        #endregion


        /// <summary>
        /// Xuất toàn bộ địa chỉ (đã bao gồm mã cũ và mới) ra list Location
        /// </summary>
        /// <returns></returns>
        /// Created by bvabao (30/7/2020)
        public List<Location> ExportListLocation()
        {
            List<Location> listLocation = new List<Location>();
            foreach (Province province in this.listLocationSync)
            {
                Location provinceInfo = new Location(MakeSuggestionForLocation(province.ProvinceName))
                {
                    Kind = province.Kind,
                    LocationName = province.ProvinceName,
                    LocationID = province.ProvinceIDCode,
                    ID = province.ProvinceID,
                    ProvinceID = province.ProvinceIDCode,
                    PID = province.ProvinceID,
                    DID = "",
                    DistrictID = "",
                    CountryID = "VN",
                    FullAddress = province.ProvinceName + ", Việt Nam",
                    ParentID = "VN",
                    CreatedBy = "bvbao",
                    ModifiedBy = "bvbao",
                    PostalCode = (province.ProvinceID.Length > 0) ? province.ProvinceID.Substring(2) : "",
                    CreatedDate = DateTime.Now.ToString("s") + "Z",
                    ModifiedDate = DateTime.Now.ToString("s") + "Z",
                };
                listLocation.Add(provinceInfo);

                foreach (District district in province.districts)
                {
                    Location districtInfo = new Location(MakeSuggestionForLocation(district.DistrictName))
                    {
                        Kind = district.Kind,
                        LocationName = district.DistrictName,
                        LocationID = district.DistrictIDCode,
                        ID = district.DistrictID,
                        ProvinceID = province.ProvinceIDCode,
                        PID = province.ProvinceID,
                        DistrictID = district.DistrictIDCode,
                        DID = district.DistrictID,
                        CountryID = "VN",
                        FullAddress = district.DistrictName + ", " + province.ProvinceName + ", Việt Nam",
                        ParentID = province.ProvinceID,
                        PostalCode = (district.DistrictID.Length > 0) ? district.DistrictID.Substring(2) : "",
                        CreatedBy = "bvbao",
                        ModifiedBy = "bvbao",
                        CreatedDate = DateTime.Now.ToString("s") + "Z",
                        ModifiedDate = DateTime.Now.ToString("s") + "Z"
                    };
                    listLocation.Add(districtInfo);
                    foreach (Ward ward in district.wards)
                    {
                        Location wardInfo = new Location(MakeSuggestionForLocation(ward.WardName))
                        {
                            Kind = ward.Kind,
                            LocationName = ward.WardName,
                            LocationID = ward.WardIDCode,
                            ID = ward.WardID,
                            ProvinceID = province.ProvinceIDCode,
                            PID = province.ProvinceID,
                            DistrictID = district.DistrictIDCode,
                            DID = district.DistrictID,
                            CountryID = "VN",
                            FullAddress = ward.WardName + ", " + district.DistrictName + ", " + province.ProvinceName + ", Việt Nam",
                            ParentID = district.DistrictID,
                            PostalCode = (ward.WardID.Length > 0) ? ward.WardID.Substring(2) : "",
                            CreatedBy = "bvbao",
                            ModifiedBy = "bvbao",
                            CreatedDate = DateTime.Now.ToString("s") + "Z",
                            ModifiedDate = DateTime.Now.ToString("s") + "Z"
                        };
                        listLocation.Add(wardInfo);
                    }
                }
            }
            return listLocation;
        }

        #region Suggestion of location
        /// <summary>
        /// Tạo các giá trị cho trường suggestion cho địa chỉ
        /// </summary>
        /// <param name="locationName">tên địa chỉ cần tạo suggestion</param>
        /// <returns></returns>
        /// created by bvbao (31/7/2020)

        public List<string> MakeSuggestionForLocation(string locationName)
        {
            List<string> textSuggest = new List<string>();
            textSuggest.Add(locationName);
            if (locationName != RemoveVietnameseSign(locationName))
            {
                textSuggest.Add(RemoveVietnameseSign(locationName));
            }
            locationName = Regex.Replace(locationName, "Xã |Phường |Thị trấn |Quận |Huyện |Thị xã |Tỉnh |Thành phố |", "");
            if (!int.TryParse(locationName, out int number) && !textSuggest.Contains(locationName))
            {
                textSuggest.Add(locationName);
                if (locationName != RemoveVietnameseSign(locationName))
                {
                    textSuggest.Add(RemoveVietnameseSign(locationName));
                }
            }

            return textSuggest;
        }
        #endregion

        #region Read New Location from Excel File to List
        /// <summary>
        /// API lấy dữ liệu địa chỉ tất cả các xã từ file excel mã mới
        /// </summary>
        /// <returns>List địa chỉ với mã mới</returns>
        /// Created by bvbao (21/7/2020)
        /// Modified by bvbao (24/7/2020)
        private List<Province> ReadNewLocationExcelFile(IFormFile file)
        {
            List<Province> list = new List<Province>();
            string filePath = UploadFile(file);
            // upload file and get filepath
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheetAt(0);

                // đọc sheet này bắt đầu từ row 1 (0 bỏ vì tiêu đề)
                int rowIndex = 1;

                // nếu vẫn chưa gặp end thì vẫn lấy data
                while (sheet.LastRowNum >= rowIndex)
                {
                    // lấy row hiện tại
                    var nowRow = sheet.GetRow(rowIndex);

                    var wardID = "VN" + NumericOrString(nowRow.GetCell(6)) + NumericOrString(nowRow.GetCell(4)) + NumericOrString(nowRow.GetCell(0));
                    var districtID = "VN" + NumericOrString(nowRow.GetCell(6)) + NumericOrString(nowRow.GetCell(4));
                    var provinceID = "VN" + NumericOrString(nowRow.GetCell(6));
                    var wardName = nowRow.GetCell(1).StringCellValue;
                    var districtName = nowRow.GetCell(5).StringCellValue;
                    var provinceName = nowRow.GetCell(7).StringCellValue;

                    if (list.Count > 0)
                    {
                        bool checkExist = false;
                        foreach (Province province in list)
                        {
                            if (province.ProvinceName == provinceName)
                            {
                                if (province.districts.Count > 0)
                                {
                                    foreach (District district in province.districts)
                                    {
                                        if (district.DistrictName == districtName)
                                        {
                                            checkExist = true;
                                            district.wards.Add(DefineNewWard(wardID, wardName));
                                            break;
                                        }
                                    }
                                    if (!checkExist)
                                    {
                                        if (wardName != "")
                                        {
                                            checkExist = true;
                                            District newDistrict = DefineNewDistrict(districtID, districtName);
                                            province.districts.Add(newDistrict);
                                            newDistrict.wards.Add(DefineNewWard(wardID, wardName));
                                        }
                                        else
                                        {
                                            checkExist = true;
                                            province.districts.Add(DefineNewDistrict(districtID, districtName));
                                        }
                                    }
                                }
                                else
                                {
                                    checkExist = true;
                                    District newDistrict = DefineNewDistrict(districtID, districtName);
                                    province.districts.Add(newDistrict);
                                    newDistrict.wards.Add(DefineNewWard(wardID, wardName));
                                }
                                break;
                            }
                        }
                        if (!checkExist)
                        {
                            checkExist = true;
                            Province newProvince = DefineNewProvince(provinceID, provinceName);
                            list.Add(newProvince);
                            District newDistrict = DefineNewDistrict(districtID, districtName);
                            newProvince.districts.Add(newDistrict);
                            newDistrict.wards.Add(DefineNewWard(wardID, wardName));
                        }
                    }
                    else
                    {
                        Province newProvince = DefineNewProvince(provinceID, provinceName);
                        list.Add(newProvince);
                        District newDistrict = DefineNewDistrict(districtID, districtName);
                        newProvince.districts.Add(newDistrict);
                        newDistrict.wards.Add(DefineNewWard(wardID, wardName));
                    }
                    rowIndex++;
                }
            }
            System.IO.File.Delete(filePath);
            return list;
        }
        #endregion

        #region Read New Location from Excel File to List
        /// <summary>
        /// API lấy danh sách tất cả địa chỉ của xã từ file excel với mã cũ
        /// </summary>
        /// <returns>List địa chỉ với mã mới</returns>
        /// Created by bvbao (21/7/2020)
        private List<LocationChangesModel> ReadOldLocationExcelFile(IFormFile file)
        {
            List<LocationChangesModel> list = new List<LocationChangesModel>();
            string filePath = UploadFile(file);
            // upload file and get filepath
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheetAt(1);

                // đọc sheet này bắt đầu từ row 3 (0,1,2 bỏ vì tiêu đề)
                int rowIndex = 3;

                while (NumericOrString(sheet.GetRow(rowIndex).GetCell(7)) != null)
                {
                    // lấy row hiện tại
                    var nowRow = sheet.GetRow(rowIndex);
                    var IdCode = NumericOrString(nowRow.GetCell(7));

                    var location = new LocationChangesModel
                    {
                        OldID = "VN" + IdCode,
                        OldLocName = nowRow.GetCell(6).StringCellValue,
                        OldDistrict = GetDistrictAndProvinceName(sheet, IdCode.Substring(0, 5), 4),
                        OldProvince = GetDistrictAndProvinceName(sheet, IdCode.Substring(0, 3), 2),
                    };
                    list.Add(location);
                    rowIndex++;
                }
            }
            System.IO.File.Delete(filePath);
            return list;
        }
        #endregion


        #region common function 
        /// <summary>
        /// Khởi tạo một tỉnh/ thành phố mới
        /// </summary>
        /// <param name="id">ID của tỉnh</param>
        /// <param name="name">Tên của tỉnh</param>
        /// <returns></returns>
        private Province DefineNewProvince(string id, string name)
        {
            Province newProvince = new Province
            {
                Kind = 1,
                ProvinceID = id,
                ProvinceName = name,
            };
            return newProvince;
        }
        /// <summary>
        /// Khởi tạo một quân/ huyện/ thị xã mới
        /// </summary>
        /// <param name="id">ID của huyện</param>
        /// <param name="name">Tên của huyện</param>
        /// <returns></returns>
        private District DefineNewDistrict(string id, string name)
        {
            District newDistrict = new District
            {
                Kind = 2,
                DistrictID = id,
                DistrictName = name,
            };
            return newDistrict;
        }
        /// <summary>
        /// Khởi tạo một xã/ phường/ thị trấn mới
        /// </summary>
        /// <param name="id">ID của xã</param>
        /// <param name="name">Tên của xã</param>
        /// <returns></returns>
        private Ward DefineNewWard(string id, string name)
        {
            Ward newWard = new Ward
            {
                Kind = 3,
                WardID = id,
                WardName = name,
            };
            return newWard;
        }
        /// <summary>
        /// Xử lý mã ID code từ Numeric về String
        /// </summary>
        /// <param name="cell">cell chứa mã IDcode dạng Numeric</param>
        /// <returns></returns>
        /// Created by bvbao (21/7/2020)
        private string NumericOrString(ICell cell)
        {
            string value = null;
            if (cell.CellType == CellType.String)
                value = cell.StringCellValue;
            if (cell.CellType == CellType.Numeric)
                value = cell.NumericCellValue.ToString();
            return value;
        }

        /// <summary>
        /// Lấy tên Tỉnh hoặc tên huyện từ parentID truyền vào
        /// </summary>
        /// <param name="sheet">Sheet excel đang duyệt</param>
        /// <param name="parentID">mã cha của địa chỉ cần lấy tên</param>
        /// <param name="index">hàng chứa tên tỉnh, huyện (2: tỉnh - 4: huyện)</param>
        /// <returns></returns>
        /// Created by bvbao (21/7/2020)
        private string GetDistrictAndProvinceName(ISheet sheet, string parentID, int index)
        {
            int rowIndex = 3;
            string LocationName = null;

            while (LocationName == null)
            {
                // lấy row hiện tại
                var nowRow = sheet.GetRow(rowIndex);

                if (parentID == NumericOrString(nowRow.GetCell(index)))
                {
                    LocationName = nowRow.GetCell(index - 1).StringCellValue;
                }
                rowIndex++;
            }
            return LocationName;
        }

        /// <summary>
        /// Upload file excel và tạo Workbook để đọc file
        /// </summary>
        /// <param name="file">file upload lên từ client</param>
        /// <returns></returns>
        /// Created by bvbao (21/7/2020)
        private string UploadFile(IFormFile file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (fileExt == ".xlsx" || fileExt == ".xls")
            {
                if (file.Length > 0)
                {
                    var filePath = Path.GetTempFileName();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return filePath;
                }
            }
            return null;
        }

        /// <summary>
        /// Loại bỏ dấu, chữ tiếng việt và chỉnh sửa các từ viết tắt, từ sai
        /// </summary>
        /// <param name="text">chuỗi tiếng việt cần xử lý</param>
        /// <returns></returns>
        /// created by bvbao (23/7/2020)
        private string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = result.Replace("òa", "oà").Replace("óa", "oá").Replace("ỏa", "oả").Replace("ọa", "oạ").Replace("õa", "oã");
            result = result.Replace("ùy", "uỳ").Replace("úy", "uý").Replace("ủy", "uỷ").Replace("ụy", "uỵ").Replace("ũy", "uỹ");
            result = result.Replace("quì", "quỳ").Replace("quí", "quý").Replace("quỉ", "quỷ").Replace("quị", "quỵ").Replace("quĩ", "quỹ").Replace("qui", "quy");
            result = Regex.Replace(result, "dak|đak|đák|đăk|dăk|dăk", "đắk");
            result = Regex.Replace(result, "`|'|-|", "");
            result = Regex.Replace(result, " ", "");

            if (result.Length > 6 && result[6].Equals('0'))
            {
                result = result.Substring(0, 6) + result.Substring(7);
            }
            return result;
        }

        /// <summary>
        /// Loại bỏ dấu (sắc, huyền, ngã, hỏi, nặng) trong tiếng việt
        /// </summary>
        /// <param name="text">từ cần loại bỏ dấu</param>
        /// <returns>chuỗi ký tự không dấu</returns>
        /// Created by bvbao (31/7/2020)
        private string RemoveVietnameseSign(string text)
        {
            text = Regex.Replace(text, "À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ắ|Ằ|Ặ|Ẳ|Ẵ|/g", "A");
            text = Regex.Replace(text, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            text = Regex.Replace(text, "È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ|/g", "E");
            text = Regex.Replace(text, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            text = Regex.Replace(text, "Ì|Í|Ị|Ỉ|Ĩ|/g", "I");
            text = Regex.Replace(text, "ì|í|ị|ỉ|ĩ|/g", "i");
            text = Regex.Replace(text, "Ò|Ó|Ọ|Ỏ|Õ|Ô|Ố|Ồ|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ|/g", "O");
            text = Regex.Replace(text, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            text = Regex.Replace(text, "Ù|Ú|Ụ|Ủ|Ũ|Ư|Ứ|Ừ|Ự|Ử|Ữ|/g", "U");
            text = Regex.Replace(text, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            text = Regex.Replace(text, "Ỳ|Ý|Ỵ|Ỷ|Ỹ|/g", "Y");
            text = Regex.Replace(text, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            text = Regex.Replace(text, "đ", "d");
            text = Regex.Replace(text, "Đ", "D");
            return text;
        }
        #endregion
    }
}
