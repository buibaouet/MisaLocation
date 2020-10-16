using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.LocationV2.Web.CustomValidation
{
    public class DateValidationAttr : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            String createdDate = Convert.ToString(value);
            return Regex.IsMatch(createdDate, @"^2\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)Z$");
        }
    }
}
