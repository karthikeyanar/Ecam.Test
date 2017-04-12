using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Globalization;

namespace Ecam.Framework {
	public class DecimalModelBinder : IModelBinder {
		public object BindModel(ControllerContext controllerContext,
			ModelBindingContext bindingContext) {
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			ModelState modelState = new ModelState { Value = valueResult };
			object actualValue = null;
			if (valueResult != null) {
				string value = valueResult.AttemptedValue.ToString();

				actualValue = DataTypeHelper.ToDecimal(value);
			}
			return actualValue;
		}
	}

	public class Int32ModelBinder : IModelBinder {
		public object BindModel(ControllerContext controllerContext,
			ModelBindingContext bindingContext) {
			ValueProviderResult valueResult = bindingContext.ValueProvider
				.GetValue(bindingContext.ModelName);
			ModelState modelState = new ModelState { Value = valueResult };
			object actualValue = null;
			if (valueResult != null) {
				string value = valueResult.AttemptedValue.ToString();
				if (string.IsNullOrEmpty(value) == false) {
					actualValue = DataTypeHelper.ToInt32(value);
				} else {
					actualValue = null;
				}
			}
			return actualValue;
		}
	}

	public class Int16ModelBinder : IModelBinder {
		public object BindModel(ControllerContext controllerContext,
			ModelBindingContext bindingContext) {
			ValueProviderResult valueResult = bindingContext.ValueProvider
				.GetValue(bindingContext.ModelName);
			ModelState modelState = new ModelState { Value = valueResult };
			object actualValue = null;
			if (valueResult != null) {
				string value = valueResult.AttemptedValue.ToString();

				if (string.IsNullOrEmpty(value) == false) {
					actualValue = DataTypeHelper.ToInt16(value);
				} else {
					actualValue = null;
				}
			}
			return actualValue;
		}
	}
}