using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class FullAddressValidationAttr : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            String fullAddress = Convert.ToString(value);
            var location = (Location)context.ObjectInstance;
            String locationName = location.LocationName;
            bool isValid = fullAddress.IndexOf(locationName) == 0;
            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(Resources.validateFullAddress);

        }
    }
}
