using Microsoft.CodeAnalysis;
using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class ModifiedDateValidationAttr : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            string validationMsg = Resources.validateCreatedDate;
            bool isValid = true;
            var location = (Location)context.ObjectInstance;
            try
            {
                DateTime createdDate = DateTimeOffset.Parse(location.CreatedDate).UtcDateTime;
                DateTime modifiedDate = DateTimeOffset.Parse(Convert.ToString(value)).UtcDateTime;
                isValid = !(DateTime.Compare(createdDate, modifiedDate) > 0);
            }
            catch (Exception)
            {
                isValid = false;
                validationMsg = Resources.invalidDateFormat;
            }

            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(validationMsg);
        }
    }
}
