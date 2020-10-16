using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MISA.LocationV2.Web.CustomValidation;
using Nest;

namespace MISA.LocationAPI.Models
{
    /// <summary>
    /// Class đại diện cho một địa chỉ
    /// </summary>
    /// Created by nmthang - 19/05/2020
    public class Location
    {
        [Required]
        [Range(0, 3)]
        public int Kind { get; set; }

        [Required(ErrorMessage = "Location name is required!")]
        [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "Length must be between 2 to 100!")]
        public string LocationName { get; set; }

        [Required(ErrorMessage = "ID is required!")]
        [StringLength(maximumLength: 12, MinimumLength = 2, ErrorMessage = "Length of ID must be between 2 to 12!")]
        [IDValidationAttr]
        public string ID { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DIDValidationAttr]
        public string DID { get; set; } = "";

        [Required(AllowEmptyStrings = true)]
        [PIDValidationAttr]
        public string PID { get; set; } = "";

        [Required]
        [CountryIDValidationAttr]
        public string CountryID { get; set; }

        public string ZIPCode { get; set; } = "";

        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "Length must be between 2 to 100!")]
        [FullAddressValidationAttr]
        public string FullAddress { get; set; }

        [Required(AllowEmptyStrings = true)]
        [ParentIDValidationAttr]
        public string ParentID { get; set; }

        [Required]
        [DateValidationAttr]
        public string CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        [Required]
        [DateValidationAttr]
        [ModifiedDateValidationAttr]
        public string ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        [Required]
        public int UsedCount { get; set; } = 0;

        public int SortOrder { get; set; } = 0;

        [Required(ErrorMessage = "LocationID is required!")]
        [StringLength(maximumLength: 9, MinimumLength = 2, ErrorMessage = "Length of LocationID must be between 2 to 9!")]
        public string LocationID { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string ProvinceID { get; set; } = "";

        [Required(AllowEmptyStrings = true)]
        public string DistrictID { get; set; } = "";

        [Required(AllowEmptyStrings = true)]
        public string AreaCode { get; set; } = "";

        [Required(AllowEmptyStrings = true)]
        public string PostalCode { get; set; } = "";

        [Required(AllowEmptyStrings = true)]
        public string LocationCode { get; set; } = "";

        public CompletionField Suggestion { get; set; }

        public Location(List<string> _input)
        {
            Suggestion = new CompletionField()
            {
                Input = _input
            };
        }
    }
}
