using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Entities.Models.Countries
{
    public class District
    {
        public string DistrictName { get; set; }
        //ID huyện mới
        public string DistrictID { get; set; }
        //ID huyện cũ
        public string DistrictIDCode { get; set; } = "";
        public int Kind { get; set; }
        public List<Ward> wards { get; set; }

        public District()
        {
            wards = new List<Ward>();
        }
    }
}
