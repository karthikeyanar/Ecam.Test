using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecam.Framework {
	public class BaseContract {
        [NotNullValidator(Ruleset = ValidationRuleSets.AlreadyExisting)]
        public int? id { get; set; }

		public IEnumerable<ErrorInfo> Errors {  get; set; }
    }
    
    /// <summary>
    /// Contract that has the Audit fields on it
    /// </summary>
    public class AuditableContract : BaseContract {
        public DateTime? created_date { get; set; }
        public int? created_by { get; set; }
				public DateTime? last_updated_date { get; set; }
        public int? last_updated_by { get; set; }
    }
}
