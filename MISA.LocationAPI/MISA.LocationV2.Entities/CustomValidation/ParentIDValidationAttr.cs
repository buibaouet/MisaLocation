using MISA.LocationV2.Entities.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Location = MISA.LocationAPI.Models.Location;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class ParentIDValidationAttr : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            String parentID = Convert.ToString(value);
            bool isValid = true;
            var location = (Location)validationContext.ObjectInstance;
            String ID = location.ID;
            int kind = location.Kind;
            int len = 0;
            switch (kind)
            {
                case 1:
                    len = 2;
                    break;
                case 2:
                    len = 4;
                    break;
                case 3:
                    len = 7;
                    break;
                default:
                    break;
            }
            try
            {
                isValid = ID.Substring(0, len) == parentID;
            }
            catch (Exception)
            {
                isValid = false;
            }

            return (isValid)
                ? ValidationResult.Success
                : new ValidationResult(Resources.validateParentID);
        }
    }
}
