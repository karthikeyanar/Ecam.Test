using Ecam.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecam.Views.Models {
    public class ExportGridViewModel {
        public ExportGridViewModel() {
            this.Req = new Dictionary<string,string>();
        }
        public Dictionary<string,string> Req { get; set; }
        public List<Ecam.Framework.ExcelHelper.ExcelSheet> Sheets { get; set; }
    }
}
