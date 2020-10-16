using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Entities.Models.Countries
{
    public class Province
    {
        public string ProvinceName { get; set; }
        // ID tỉnh cũ
        public string ProvinceIDCode { get; set; } = "";
        // ID tỉnh mới
        public string ProvinceID { get; set; }
        public int Kind { get; set; }
        public List<District> districts { get; set; }

        public Province()
        {
            districts = new List<District>();
        }
    }
}
