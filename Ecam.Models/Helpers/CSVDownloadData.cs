using Ecam.Contracts;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Mail;
using System.Linq;
using Ecam.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data.Entity.Validation;
using Ecam.Models.Helpers;
using Ecam.Contracts.Enums;
using Ecam.Framework.ExcelHelper;
using CsvHelper;

namespace Ecam.Models {
    public class CSVDownloadData {
        private List<string> _SYMBOLS_LIST = null;
        private bool _IS_NOT_SUCCESS = false;
        public CSVDownloadData(string file_name, List<string> symbols,ManualResetEvent doneEvent) {
            _SYMBOLS_LIST = symbols;
            _ORIGINAL_FILE_NAME = file_name;
            _FILE_NAME = file_name;
            _doneEvent = doneEvent;
        }

        public CSVDownloadData() {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext) {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...",threadIndex);
            if(string.IsNullOrEmpty(_FILE_NAME) == false) {
                CSVDataDownload(_FILE_NAME);
                //CalculateRSI(_FILE_NAME);
            }
            Console.WriteLine("thread {0} result calculated...",threadIndex);
            string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
            string IMPORT_BACKUP_CSV = IMPORT_CSV + "\\Backup";
            string fileName = IMPORT_CSV + "\\" + _ORIGINAL_FILE_NAME + ".csv";
            //if(File.Exists(fileName) == true) {
            //    File.Delete(fileName);
            //}
            if(this._IS_NOT_SUCCESS == false) {
                if(string.IsNullOrEmpty(_ORIGINAL_FILE_NAME) == false) {
                    if(Directory.Exists(IMPORT_BACKUP_CSV) == false) {
                        Directory.CreateDirectory(IMPORT_BACKUP_CSV);
                    }
                    string movefile_name = System.IO.Path.Combine(IMPORT_BACKUP_CSV,System.IO.Path.GetFileName(fileName));
                    if(System.IO.File.Exists(movefile_name) == false) {
                        System.IO.File.Move(fileName,movefile_name);
                    }
                }
            }
            _doneEvent.Set();
        }

        private void CSVDataDownload(string tempfilename) {
            List<OldSymbol> oldSymbolList = new List<Models.OldSymbol> {
                new OldSymbol { old_symbol = "PFRL", new_symbol = "ABFRL" },
                new OldSymbol { old_symbol = "SKSMICRO", new_symbol = "BHARATFIN" },
                new OldSymbol { old_symbol = "ADI", new_symbol = "FAIRCHEM" },
                new OldSymbol { old_symbol = "FCEL", new_symbol = "FCONSUMER" },
                new OldSymbol { old_symbol = "FRL", new_symbol = "FEL" },
                new OldSymbol { old_symbol = "HCIL", new_symbol = "HSCL" },
                new OldSymbol { old_symbol = "HITACHIHOM", new_symbol = "JCHAC" },
                new OldSymbol { old_symbol = "MAX", new_symbol = "MFSL" },
                new OldSymbol { old_symbol = "GULFCORP", new_symbol = "GOCLCORP" },
                new OldSymbol { old_symbol = "IBSEC", new_symbol = "IBVENTURES" },
                new OldSymbol { old_symbol = "VIKASGLOB", new_symbol = "VIKASECO" },
            };
            if(string.IsNullOrEmpty(tempfilename) == false) {
                string url = string.Empty;
                string html = string.Empty;
                string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
                string fileName = IMPORT_CSV + "\\" + tempfilename + ".csv";
                CsvReader csv = null;
                int i = 0;
                if(File.Exists(fileName) == true) {
                    using(TextReader reader = File.OpenText(fileName)) {
                        csv = new CsvReader(reader);
                        i = 0;
                        while(csv.Read()) {
                            i += 1;
                            string symbol = csv.GetField<string>("Symbol");
                            string series = csv.GetField<string>("Series");
                            string date = csv.GetField<string>("Date");
                            string open = csv.GetField<string>("Open Price");
                            string high = csv.GetField<string>("High Price");
                            string low = csv.GetField<string>("Low Price");
                            string close = csv.GetField<string>("Close Price");
                            string lastTrade = csv.GetField<string>("Last Price");
                            string prev = csv.GetField<string>("Prev Close");
                            DateTime dt = DataTypeHelper.ToDateTime(date);
                            if(string.IsNullOrEmpty(symbol) == false
                                && series == "EQ") {
                                symbol = symbol.Replace("&amp;","&");
                                if(_SYMBOLS_LIST.Contains(symbol) == false) {
                                    OldSymbol oldSymbol = (from q in oldSymbolList
                                                           where q.old_symbol == symbol
                                                           select q).FirstOrDefault();
                                    if(oldSymbol != null) {
                                        symbol = oldSymbol.new_symbol;
                                    }
                                }
                                if(_SYMBOLS_LIST.Contains(symbol) == false) {
                                    Helper.Log("Symbol does not exist Symbol=" + symbol + ",FileName=" + tempfilename,"CSVDataDownload" + symbol + "_" + (new Random()).Next(1000,10000));
                                    this._IS_NOT_SUCCESS = true;
                                    break;
                                } else {
                                    TradeHelper.ImportPrice(new TempClass {
                                        symbol = symbol,
                                        trade_date = dt,
                                        close_price = DataTypeHelper.ToDecimal(close),
                                        high_price = DataTypeHelper.ToDecimal(high),
                                        low_price = DataTypeHelper.ToDecimal(low),
                                        open_price = DataTypeHelper.ToDecimal(open),
                                        ltp_price = DataTypeHelper.ToDecimal(lastTrade),
                                        prev_price = DataTypeHelper.ToDecimal(prev),
                                        is_prev_price_exist = true
                                    });
                                }
                            }
                        }
                    }
                }  
            }
        }

        public string file_name { get { return _FILE_NAME; } }
        private string _FILE_NAME;
        private string _ORIGINAL_FILE_NAME;

        private ManualResetEvent _doneEvent;
    }

    public class OldSymbol {
        public string old_symbol { get; set; }
        public string new_symbol { get; set; }
    }
}