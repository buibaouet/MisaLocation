using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class CountryIDValidationAttr : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String countryID = Convert.ToString(value);
            bool isValid = true;
            var location = (Location)validationContext.ObjectInstance;
            String ID = location.ID;
            int kind = location.Kind;
            try
            {
                if (ID.Substring(0, 2) != countryID) isValid = false;
            }
            catch (Exception)
            {
                isValid = false;
            }
            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(Resources.validateFormatCountryID);
        }
    }
}
