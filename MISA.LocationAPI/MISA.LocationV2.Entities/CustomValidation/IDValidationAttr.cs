using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class IDValidationAttr : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            String ID = Convert.ToString(value);
            return Regex.IsMatch(ID, "^[A-Z]{2}[0-9]{0,10}$");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String ID = Convert.ToString(value);
            int len = ID.Length;
            bool isValid = true;
            var location = (Location)validationContext.ObjectInstance;
            var kind = location.Kind;
            switch (kind)
            {
                case 0:
                    if (len != 2) isValid = false;
                    break;
                case 1:
                    if (len != 4) isValid = false;
                    break;
                case 2:
                    if (len != 7) isValid = false;
                    break;
                case 3:
                    if (len != 12) isValid = false;
                    break;
            }

            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(Resources.validateLengthID);
        }
    }
}
