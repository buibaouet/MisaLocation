using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.LocationAPI.Models
{
    public class LocationChangesModel
    {
        public string NewProvince { get; set; } = "";

        public string NewDistrict { get; set; } = "";

        public string NewID { get; set; } = "";

        public string NewLocName { get; set; } = "";

        public string OldLocName { get; set; } = "";

        public string OldID { get; set; } = "";

        public string OldDistrict { get; set; } = "";

        public string OldProvince { get; set; } = "";

        public string Action { get; set; } = "";
    }
}
