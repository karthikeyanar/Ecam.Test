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
        public List<string> _SYMBOLS_LIST = null;
        private bool _IS_NOT_SUCCESS = false;
        public CSVDownloadData(string file_name,List<string> symbols,ManualResetEvent doneEvent) {
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

        public string CSVDataDownload(string tempfilename,bool isTakeTempFileName = false) {
            string lastSymbol = "";
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
                new OldSymbol { old_symbol = "FINANTECH", new_symbol = "63MOONS" },
                new OldSymbol { old_symbol = "CROMPGREAV", new_symbol = "CGPOWER" },
                new OldSymbol { old_symbol = "FEDDERLOYD", new_symbol = "FEDDERELEC" },
                new OldSymbol { old_symbol = "GEOJITBNPP", new_symbol = "GEOJITFSL" },
                new OldSymbol { old_symbol = "HITECHPLAS", new_symbol = "HITECHCORP" },
                new OldSymbol { old_symbol = "LLOYDELENG", new_symbol = "LEEL" },
                new OldSymbol { old_symbol = "NAGAAGRI", new_symbol = "NACLIND" },
                new OldSymbol { old_symbol = "SUJANATWR", new_symbol = "NTL" },
                new OldSymbol { old_symbol = "VIDHIDYE", new_symbol = "VIDHIING" },
                new OldSymbol { old_symbol = "STOREONE", new_symbol = "SORILINFRA" },
                new OldSymbol { old_symbol = "ABSHEKINDS", new_symbol = "TRIDENT" },
                new OldSymbol { old_symbol = "ARVINDMILL", new_symbol = "ARVIND" },
                new OldSymbol { old_symbol = "BAJAJAUTO", new_symbol = "BAJAJHLDNG" },
                new OldSymbol { old_symbol = "BAJAUTOFIN", new_symbol = "BAJFINANCE" },
                new OldSymbol { old_symbol = "BIRLAJUTE", new_symbol = "BIRLACORPN" },
                new OldSymbol { old_symbol = "BOC", new_symbol = "LINDEINDIA" },
                new OldSymbol { old_symbol = "CHOLADBS", new_symbol = "CHOLAFIN" },
                new OldSymbol { old_symbol = "DALMIABEL", new_symbol = "DALMIABHA" },
                new OldSymbol { old_symbol = "DCMSRMCONS", new_symbol = "DCMSHRIRAM" },
                new OldSymbol { old_symbol = "DEWANHOUS", new_symbol = "DHFL" },
                new OldSymbol { old_symbol = "FCH", new_symbol = "CAPF" },
                new OldSymbol { old_symbol = "FVIL", new_symbol = "FCONSUMER" },
                new OldSymbol { old_symbol = "GUJAMBCEM", new_symbol = "AMBUJACEM" },
                new OldSymbol { old_symbol = "GWALCHEM", new_symbol = "GEECEE" },
                new OldSymbol { old_symbol = "HEROHONDA", new_symbol = "HEROMOTOCO" },
                new OldSymbol { old_symbol = "HINDALC0", new_symbol = "HINDALCO" },
                new OldSymbol { old_symbol = "HINDLEVER", new_symbol = "HINDUNILVR" },
                new OldSymbol { old_symbol = "HINDSANIT", new_symbol = "HSIL" },
                new OldSymbol { old_symbol = "IBPOW", new_symbol = "RTNPOWER" },
                new OldSymbol { old_symbol = "ICI", new_symbol = "AKZOINDIA" },
                new OldSymbol { old_symbol = "ILFSTRANS", new_symbol = "IL&FSTRANS" },
                new OldSymbol { old_symbol = "JSTAINLESS", new_symbol = "JSL" },
                new OldSymbol { old_symbol = "MADRASCEM", new_symbol = "RAMCOCEM" },
                new OldSymbol { old_symbol = "MICO", new_symbol = "BOSCHLTD" },
                new OldSymbol { old_symbol = "MYSORECEM", new_symbol = "HEIDELBERG" },
                new OldSymbol { old_symbol = "NAGARCONST", new_symbol = "NCC" },
                new OldSymbol { old_symbol = "NEYVELILIG", new_symbol = "NLCINDIA" },
                new OldSymbol { old_symbol = "OBEROIREAL", new_symbol = "OBEROIRLTY" },
                new OldSymbol { old_symbol = "PANTALOONR", new_symbol = "FEL" },
                new OldSymbol { old_symbol = "RAINCOM", new_symbol = "RAIN" },
                new OldSymbol { old_symbol = "REL", new_symbol = "RELINFRA" },
                new OldSymbol { old_symbol = "SESAGOA", new_symbol = "VEDL" },
                new OldSymbol { old_symbol = "SOLAREX", new_symbol = "SOLARINDS" },
                new OldSymbol { old_symbol = "SPLLTD", new_symbol = "SOMANYCERA" },
                new OldSymbol { old_symbol = "SREINTFIN", new_symbol = "SREINFRA" },
                new OldSymbol { old_symbol = "SSLT", new_symbol = "VEDL" },
                new OldSymbol { old_symbol = "SWARAJMAZD", new_symbol = "SMLISUZU" },
                new OldSymbol { old_symbol = "TATATEA", new_symbol = "TATAGLOBAL" },
                new OldSymbol { old_symbol = "UNIPHOS", new_symbol = "UPL" },
                new OldSymbol { old_symbol = "VISHALRET", new_symbol = "V2RETAIL" },
                new OldSymbol { old_symbol = "WABCO-TVS", new_symbol = "WABCOINDIA" },
                new OldSymbol { old_symbol = "WELGUJ", new_symbol = "WELCORP" },
                new OldSymbol { old_symbol = "PRSMJOHNSN", new_symbol = "PRISMCEM" },
                new OldSymbol { old_symbol = "UTIBANK", new_symbol = "AXISBANK" },
            };
            if(string.IsNullOrEmpty(tempfilename) == false) {
                string url = string.Empty;
                string html = string.Empty;
                string IMPORT_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_CSV"];
                string fileName = IMPORT_CSV + "\\" + tempfilename + ".csv";
                if(isTakeTempFileName == true) {
                    fileName = tempfilename;
                }
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
                            string turnOver = csv.GetField<string>("Turnover");
                            DateTime dt = DataTypeHelper.ToDateTime(date);
                            if(string.IsNullOrEmpty(symbol) == false) {
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
                                    });
                                    lastSymbol = symbol;
                                }
                            }
                        }
                    }
                }
            }
            return lastSymbol;
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