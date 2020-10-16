using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class PIDValidationAttr : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String PID = Convert.ToString(value);
            bool isValid = true;
            var location = (Location)validationContext.ObjectInstance;
            String ID = location.ID;
            int kind = location.Kind;
            switch (kind)
            {
                case 0:
                    if (PID != String.Empty) isValid = false;
                    break;
                default:
                    try
                    {
                        if (ID.Substring(0, 4) != PID) isValid = false;
                    }
                    catch (Exception)
                    {
                        isValid = false;
                    }
                    break;
            }

            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(Resources.validatePID);
        }
    }
}
