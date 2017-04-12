using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework {
	 [Serializable]
    public partial struct ValidationRuleSets {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Read = "Read";
        public const string Delete = "Delete";
        /// <summary>
        /// Rule to represent Update, Read, and Delete. Use this Rule where the Property is required when Updating, reading, or deleting, but not 
        /// while creating. Eg: for FundID, you will use this attribute 
        /// </summary>
        public const string AlreadyExisting = "AlreadyExisting";
        public const string ContextCreate = "ContextCreate";
        public const string ContextUpdate = "ContextUpdate";
        public const string DerivedCreate = "DerivedCreate";
    }
}
