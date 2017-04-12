using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;

namespace Ecam.Framework
{
    public class SLExcelStatus
    {
        public string Message { get; set; }
        public bool Success
        {
            get { return string.IsNullOrWhiteSpace(Message); }
        }
    }

    public class SLExcelData
    {
		public SLExcelData() {
			Status = new SLExcelStatus();
			Headers = new List<string>();
			DataRows = new List<List<string>>();
		}

        public SLExcelStatus Status { get; set; }
        public Columns ColumnConfigurations { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public string SheetName { get; set; }

		public  string GetValue(List<string> row, string columnName) {
            string value = string.Empty;
            int index = this.GetColumnIndex(columnName);
            if(index >= 0) {
                value = row[index];
                if(string.IsNullOrEmpty(value) == true)
                    value = string.Empty;
            }
            return value;
		}

		public int GetColumnIndex(string columnName) {
            int index = -1;
            try {
                index = this.Headers.IndexOf(columnName);
            } catch { }
            return index;
		}

		public void SetValue(List<string> row, string columnName, string value) {
             int index = this.GetColumnIndex(columnName);
             if(index >= 0) {
                 row[index] = value;
             }
		}
    }
}