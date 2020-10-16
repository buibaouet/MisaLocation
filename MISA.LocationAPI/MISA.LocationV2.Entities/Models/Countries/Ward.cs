using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.LocationV2.Entities.Models.Countries
{
    public class Ward
    {
        public string WardName { get; set; }
        //ID xã cũ
        public string WardIDCode { get; set; } = "";
        //ID xã mới
        public string WardID { get; set; }
        public int Kind { get; set; }
    }
}
