using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework {
    public class AutoCompleteList {
        public AutoCompleteList() {
            id = 0;
            label = string.Empty;
            value = string.Empty;
        }
        public int id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
    }

    public class Select2List {
        public Select2List() {
            id = string.Empty;
            label = string.Empty;
            value = string.Empty;
        }
        public string id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public string other { get; set; }
    }

    public class AutoCompleteListExtend {
        public AutoCompleteListExtend() {
            id = 0;
            label = string.Empty;
            value = string.Empty;
        }
        public int id { get; set; }
        public int? id2 { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public string other { get; set; }
        public string other2 { get; set; }
        public string other3 { get; set; }
    }

    public class Select2ListExtend {
        public Select2ListExtend() {
            id = string.Empty;
            label = string.Empty;
            value = string.Empty;
        }
        public string id { get; set; }
        public string id2 { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public string other { get; set; }
        public string other2 { get; set; }
        public string other3 { get; set; }
    }

    public class DateTimeConverter:DateTimeConverterBase {
       
        public override object ReadJson(JsonReader reader,Type objectType,object existingValue,Newtonsoft.Json.JsonSerializer serializer) {
            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer,object value,Newtonsoft.Json.JsonSerializer serializer) {
            DateTime dt = DateTime.Now.Date;
            DateTime.TryParse(Convert.ToString(value),out dt);
            if(dt.Hour > 0)
                writer.WriteValue(dt.ToString());
            else if(dt.Year > 1900)
                writer.WriteValue(dt.ToString("MM/dd/yyyy"));
            else
                writer.WriteValue(string.Empty);
        }
    }
}
