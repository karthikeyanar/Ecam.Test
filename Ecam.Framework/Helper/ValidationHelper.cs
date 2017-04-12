using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework
{
    public class ValidationHelper
    {
        /// <summary>
        /// Get any errors associated with the model also investigating any rules dictated by attached Metadata buddy classes.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IEnumerable<ErrorInfo> Validate(object instance)
        {
            //return new List<ErrorInfo>();
            var metadataAttrib = instance.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            var buddyClassOrModelClass = metadataAttrib != null ? metadataAttrib.MetadataClassType : instance.GetType();
            var buddyClassProperties = TypeDescriptor.GetProperties(buddyClassOrModelClass).Cast<PropertyDescriptor>();
            var modelClassProperties = TypeDescriptor.GetProperties(instance.GetType()).Cast<PropertyDescriptor>();

            List<ErrorInfo> errors = (from buddyProp in buddyClassProperties
                                      join modelProp in modelClassProperties on buddyProp.Name equals modelProp.Name
                                      from attribute in buddyProp.Attributes.OfType<ValidationAttribute>()
                                      where !attribute.IsValid(modelProp.GetValue(instance))
                                      select new ErrorInfo(buddyProp.Name, attribute.FormatErrorMessage(attribute.ErrorMessage), instance)).ToList();
            // Add in the class level custom attributes
            IEnumerable<ErrorInfo> classErrors = from attribute in TypeDescriptor.GetAttributes(buddyClassOrModelClass).OfType<ValidationAttribute>()
                                                 where !attribute.IsValid(instance)
                                                 select new ErrorInfo("ClassLevelCustom", attribute.FormatErrorMessage(attribute.ErrorMessage), instance);

            errors.AddRange(classErrors);
            return errors.AsEnumerable();
        }

        public static string GetErrorInfo(IEnumerable<ErrorInfo> errorInfo)
        {
            StringBuilder errors = new StringBuilder();
            if (errorInfo != null)
            {
                foreach (var err in errorInfo.ToList())
                {
                    errors.Append(err.ErrorMessage + "\n");
                }
            }
            return errors.ToString();
        }

    }

    public class DateRangeAttribute:ValidationAttribute {
        private const string DateFormat = "MM/dd/yyyy";
        private const string DefaultErrorMessage =
               "'{0}' must be a date between {1:d} and {2:d}.";

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        public DateRangeAttribute()
            : base(DefaultErrorMessage) {
            MinDate = ParseDate("01/01/1900");
            MaxDate = DateTime.MaxValue;
        }

        public DateRangeAttribute(string minDate,string maxDate)
            : base(DefaultErrorMessage) {
            MinDate = ParseDate(minDate);
            MaxDate = ParseDate(maxDate);
        }

        public override bool IsValid(object value) {
            if(value == null || !(value is DateTime)) {
                return true;
            }
            DateTime dateValue = (DateTime)value;
            return MinDate <= dateValue && dateValue <= MaxDate;
        }
        public override string FormatErrorMessage(string name) {
            return String.Format(CultureInfo.CurrentCulture,
                ErrorMessageString,
                name,MinDate,MaxDate);
        }

        private static DateTime ParseDate(string dateValue) {
            return DateTime.ParseExact(dateValue,DateFormat,
                 CultureInfo.InvariantCulture);
        }
    }
}
