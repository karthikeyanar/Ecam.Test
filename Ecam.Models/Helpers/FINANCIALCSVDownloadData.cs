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
    public class FINANCIALCSVDownloadData {
        private List<string> _SYMBOLS_LIST = null;
        private bool _IS_NOT_SUCCESS = false;
        public FINANCIALCSVDownloadData(string file_name,List<string> symbols,ManualResetEvent doneEvent) {
            _SYMBOLS_LIST = symbols;
            _ORIGINAL_FILE_NAME = file_name;
            _FILE_NAME = file_name;
            _doneEvent = doneEvent;
        }

        public FINANCIALCSVDownloadData() {
        }

        // Wrapper method for use with thread pool.
        public void ThreadPoolCallback(Object threadContext) {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...",threadIndex);
            if(string.IsNullOrEmpty(_FILE_NAME) == false) {
                FINANCIALCSVDataDownload(_FILE_NAME);
                //CalculateRSI(_FILE_NAME);
            }
            Console.WriteLine("thread {0} result calculated...",threadIndex);
            string IMPORT_FINANCIAL_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_FINANCIAL_CSV"];
            string IMPORT_BACKUP_FINANCIALCSV = IMPORT_FINANCIAL_CSV + "\\Backup";
            string fileName = IMPORT_FINANCIAL_CSV + "\\" + _ORIGINAL_FILE_NAME + ".csv";
            //if(File.Exists(fileName) == true) {
            //    File.Delete(fileName);
            //}
            if(this._IS_NOT_SUCCESS == false) {
                if(string.IsNullOrEmpty(_ORIGINAL_FILE_NAME) == false) {
                    if(Directory.Exists(IMPORT_BACKUP_FINANCIALCSV) == false) {
                        Directory.CreateDirectory(IMPORT_BACKUP_FINANCIALCSV);
                    }
                    string movefile_name = System.IO.Path.Combine(IMPORT_BACKUP_FINANCIALCSV,System.IO.Path.GetFileName(fileName));
                    if(System.IO.File.Exists(movefile_name) == false) {
                        System.IO.File.Move(fileName,movefile_name);
                    }
                }
            }
            _doneEvent.Set();
        }

        private void FINANCIALCSVDataDownload(string tempfilename) {
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
            string symbol = string.Empty;
            if(string.IsNullOrEmpty(tempfilename) == false) {
                string url = string.Empty;
                string html = string.Empty;
                string IMPORT_FINANCIAL_CSV = System.Configuration.ConfigurationManager.AppSettings["IMPORT_FINANCIAL_CSV"];
                string fileName = IMPORT_FINANCIAL_CSV + "\\" + tempfilename + ".csv";
                CsvReader csv = null;
                int i = 0;
                int j = 0;
                if(File.Exists(fileName) == true) {
                    using(TextReader reader = File.OpenText(fileName)) {
                        csv = new CsvReader(reader);
                        i = 0;
                        while(csv.Read()) {
                            i += 1;
                            symbol = csv.FieldHeaders[0];
                            using(EcamContext context = new EcamContext()) {
                                int? cnt = (from q in context.tra_company
                                            where q.symbol == symbol
                                            select q).Count();
                                if((cnt ?? 0) <= 0) {
                                    Helper.Log("Symbol does not exist Symbol=" + symbol + ",FileName=" + tempfilename,"FINANCIALCSVDataDownload" + symbol + "_" + (new Random()).Next(1000,10000));
                                    this._IS_NOT_SUCCESS = true;
                                }
                            }
                            if(this._IS_NOT_SUCCESS == false) {
                                for(j = 0;j < csv.FieldHeaders.Length;j++) {
                                    string categoryName = csv.GetField(0).Replace("&amp;","");
                                    tra_financial_category financialCategory = null;
                                    using(EcamContext context = new EcamContext()) {
                                        financialCategory = (from q in context.tra_financial_category
                                                             where q.category_name == categoryName
                                                             select q).FirstOrDefault();
                                        if(financialCategory == null) {
                                            financialCategory = new Models.tra_financial_category {
                                                category_name = categoryName,
                                                is_archive = false
                                            };
                                            context.tra_financial_category.Add(financialCategory);
                                            context.SaveChanges();
                                        }
                                    }
                                    if(financialCategory != null) {
                                        if((financialCategory.is_archive ?? false) == false) {
                                            if(j > 0) {
                                                DateTime financialDate = DataTypeHelper.ToDateTime(csv.FieldHeaders[j]);
                                                using(EcamContext context = new EcamContext()) {
                                                    tra_financial traFinancial = (from q in context.tra_financial
                                                                                  where q.symbol == symbol
                                                                                  && q.financial_category_id == financialCategory.id
                                                                                  && q.financial_date == financialDate
                                                                                  select q).FirstOrDefault();
                                                    if(traFinancial == null) {
                                                        traFinancial = new tra_financial {
                                                            financial_category_id = financialCategory.id,
                                                            symbol = symbol,
                                                            financial_date = financialDate,
                                                            value = DataTypeHelper.ToDecimal(csv.GetField(csv.FieldHeaders[j])),
                                                            prev_value = 0
                                                        };
                                                        context.tra_financial.Add(traFinancial);
                                                        context.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(string.IsNullOrEmpty(symbol) == false) {
                using(EcamContext context = new EcamContext()) {
                    List<tra_financial> traFinancials = (from q in context.tra_financial
                                                         where q.symbol == symbol
                                                         orderby q.financial_date descending, q.financial_category_id ascending
                                                         select q).ToList();
                    foreach(var row in traFinancials) {
                        tra_financial prev = (from q in traFinancials
                                              where q.symbol == symbol
                                              && q.financial_category_id == row.financial_category_id
                                              && q.financial_date < row.financial_date
                                              orderby q.financial_date descending
                                              select q).FirstOrDefault();
                        if(prev != null) {
                            row.prev_value = prev.value;
                            context.Entry(row).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
            }
            Console.WriteLine("FINANCIAL COMPLETED SYMBOL=" + symbol);
        }

        public string file_name { get { return _FILE_NAME; } }
        private string _FILE_NAME;
        private string _ORIGINAL_FILE_NAME;

        private ManualResetEvent _doneEvent;
    }

}