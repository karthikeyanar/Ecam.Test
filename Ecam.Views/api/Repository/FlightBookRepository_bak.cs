using Ecam.Api;
using Ecam.Contracts;
using Ecam.Models;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework.Repository {

    public interface IFlightBookRepository {
        PaginatedListResult<EC_FLIGHT_BOOK> Get(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK> GetTransitSummaryReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_STATUS> GetTransitGroupReportList(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK> GetTransitList(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging);
        List<EC_FLIGHT_BOOK_STATUS_DETAIL> GetStatusDetailList(EC_FLIGHT_BOOK_STATUS_DETAIL criteria);
        List<EC_FLIGHT_BOOK_STATUS_DETAIL> GetStatusDetail(int id);
        PaginatedListResult<EC_FLIGHT_BOOK_STATUS> GetFlightStatus(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging);
        List<AutoCompleteListExtend> SearchFlights(int companyId,int originId,int destId,DateTime? flightDate,int flightId,int maxDays);
        List<EC_FLIGHT_PAYLOAD> GetFlightPayLoad(EC_FLIGHT_BOOK_SEARCH criteria);
        List<EC_FLIGHT_BOOK_CHARGE> GetFlightBookCharges(string awbnos);
        List<EC_FLIGHT_BOOK_FILES> GetFlightBookFiles(string awbnos);
        ec_flight_book GetFlightBook(string awb_no);
        ec_flight_book_status GetFlightBookStatus(int id);
        void UpdateFlightBookStatus(int id,string status);
        void UpdateSentFFR(int id,bool isSentFFR);
        void DeleteBooking(string awbnos);
        void CancelBooking(string awbnos);
        void UpliftBooking(string awbnos);
        void DeleteFlightBookStatus(string awb_no);
        void UpdateMailLogFlightBookStatusDetail(int id);
        void Save(ref ec_flight_book flightBook);
        PaginatedListResult<EC_FLIGHT_BOOK_DRAFT> GetDrafts(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_TV> GetTVReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_MAP> GetMapReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_FLIGHT_DETAIL> GetFlightDetail(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_AWBNO_DETAIL> GetAWBNoDetail(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging);
        PaginatedListResult<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT> GetOffBoardFlights(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging);
    }

    public class FlightBookRepository:IFlightBookRepository {

        public PaginatedListResult<EC_FLIGHT_BOOK_STATUS> GetTransitGroupReportList(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status fbs {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            if(criteria.group_id <= 0) {
                if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                    where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentGroupID));
                } else if(role == "AA" || role == "AM") {
                    where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentAgentGroupIDs));
                } else {
                    where.Append(string.Format(" fb.group_id in({0})",0));
                }
            } else {
                where.Append(string.Format(" fb.group_id in({0})",criteria.group_id));
            }

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.AppendFormat(string.Format(" and fb.awb_no='{0}'",criteria.awb_no));
            } else {

                List<EC_USER_AIRLINE> userAirlines = null;
                List<EC_USER_AGENT> userAgents = null;
                FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

                if(criteria.start_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.end_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.flight_date.Year > 1900) {
                    where.Append(string.Format(" and fb.flight_date='{0}'",criteria.flight_date.ToString("yyyy-MM-dd")));
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                    string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                    foreach(int id in companyIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.company_id in({0})",ids);
                    userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
                }

                //agent id check
                if(criteria.agent_id > 0) {
                    where.AppendFormat(" and fb.agent_id in({0})",criteria.agent_id);
                }

                if(string.IsNullOrEmpty(criteria.agent_name) == false) {
                    string agentSql = string.Format("select agent_id from ec_agent where agent_name='{0}'",criteria.agent_name);
                    int agent_id = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,agentSql));
                    if(agent_id > 0) {
                        where.AppendFormat(" and fb.agent_id in({0})",agent_id);
                    }
                }

                //airline id check
                if(criteria.airline_id > 0) {
                    where.AppendFormat(" and fb.airline_id in({0})",criteria.airline_id);
                }

                //origin_id check
                if(criteria.origin_id > 0) {
                    where.AppendFormat(" and fbs.origin_id in({0})",criteria.origin_id);
                }

                //flight id check
                if(criteria.flight_id > 0) {
                    where.AppendFormat(" and fbs.flight_id in({0})",criteria.flight_id);
                }

                //destination check
                if(criteria.dest_id > 0) {
                    where.AppendFormat(" and fbs.dest_id in({0})",criteria.dest_id);
                }
            }

            //joining the detail table as left join
            joinTables = " left join ec_flight_book_status_detail as fbsd on fbs.flight_book_status_id=fbsd.flight_book_status_id";

            joinTables += " join ec_flight_book fb on fb.awb_no = fbs.awb_no " +
                          " join ec_airport originAirport on originAirport.airport_id = fbs.origin_id " +
                          " join ec_airport destiAirport on destiAirport.airport_id = fbs.dest_id " +
                          " join ec_airport fbOriginAirport on fbOriginAirport.airport_id = fb.origin_id " +
                          " join ec_airport fbDestAirport on fbDestAirport.airport_id = fb.dest_id " +
                          " join ec_flight fli on fbs.flight_id=fli.flight_id " +
                          " join ec_agent agent on fb.agent_id=agent.agent_id ";

            selectFields = "count(*) as cnt";
            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");
            //sql = "select count(*) from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = " fb.agent_id, fbs.origin_id, " +
                            " fli.flight_no, " +
                            " fbs.awb_no, " +
                            " fbsd.cargo_status as cargo_status_detail, " +
                            " fbs.cargo_status, " +
                            " fbsd.flight_date as flight_date_detail, " +
                            " fbsd.grwt,fbsd.chwt, " +
                            " fbsd.pieces,originAirport.airport_iata_code as origin_iata_code," +
                            " destiAirport.airport_iata_code as dest_iata_code," +
                            " fbOriginAirport.airport_iata_code as fb_origin_iata_code," +
                            " fbDestAirport.airport_iata_code as fb_dest_iata_code," +
                            " agent.agent_name as agent_name";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK_STATUS> rows = new List<EC_FLIGHT_BOOK_STATUS>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS>(sql).ToList();
            }

            return new PaginatedListResult<EC_FLIGHT_BOOK_STATUS> {
                total = paging.Total, rows = rows
            };
        }

        public PaginatedListResult<EC_FLIGHT_BOOK> GetTransitSummaryReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book fb {1} where {2} group by {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            if(criteria.group_id <= 0) {
                if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                    where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentGroupID));
                } else if(role == "AA" || role == "AM") {
                    where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentAgentGroupIDs));
                } else {
                    where.Append(string.Format(" fb.group_id in({0})",0));
                }
            } else {
                where.Append(string.Format(" fb.group_id in({0})",criteria.group_id));
            }

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.AppendFormat(string.Format(" and fb.awb_no='{0}'",criteria.awb_no));
            } else {

                List<EC_USER_AIRLINE> userAirlines = null;
                List<EC_USER_AGENT> userAgents = null;
                FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

                if(criteria.start_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.end_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.flight_date.Year > 1900) {
                    where.Append(string.Format(" and fb.flight_date='{0}'",criteria.flight_date.ToString("yyyy-MM-dd")));
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                    string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                    foreach(int id in companyIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.company_id in({0})",ids);
                    userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                    string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and flight.flight_id in({0})",ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                    string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.airline_id in({0})",ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.agent_ids) == false) {
                    string[] arr = criteria.agent_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.agent_id in({0})",ids);
                }

                if(string.IsNullOrEmpty(criteria.cargo_status) == false) {
                    //where.AppendFormat(" and fbs.cargo_status in({0})", criteria.cargo_status);
                    where.AppendFormat(" and ifnull(fbs.cargo_status,'')='{0}'",criteria.cargo_status);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.dest_ids) == false) {
                    string[] arr = criteria.dest_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.dest_id in({0})",ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.origin_ids) == false) {
                    string[] arr = criteria.origin_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.origin_id in({0})",ids);
                }
            }

            switch(criteria.group_key) {
                case "awbno":
                    groupByName = " fb.awb_no ";
                    break;
                case "flight":
                    groupByName = " fbs.flight_id ";
                    break;
                case "airline":
                    groupByName = " fb.airline_id ";
                    break;
                case "agent":
                    groupByName = " agent.agent_name";
                    break;
                case "origin":
                    groupByName = " fb.origin_id ";
                    break;
                case "destination":
                    groupByName = " fb.dest_id ";
                    break;
                default:
                    groupByName = " fb.awb_no";
                    break;
            }


            joinTables += " join ec_airline airline on fb.airline_id = airline.airline_id" +
                          " join ec_agent agent on fb.agent_id = agent.agent_id" +
                          " join ec_airport originAirport on fb.origin_id = originAirport.airport_id" +
                          " join ec_airport destAirport on fb.dest_id = destAirport.airport_id" +
                          " join ec_route rou on fb.route_id = rou.route_id" +
                          " join ec_company company on fb.company_id = company.company_id";

            if(criteria.group_key == "flight" || string.IsNullOrEmpty(criteria.flight_ids) == false) {
                //joinTables += " join ec_flight_book_status fbs on fb.awb_no = fbs.awb_no";
                joinTables += " join ec_flight flight on fbs.flight_id = flight.flight_id ";
            }

            if(string.IsNullOrEmpty(criteria.cargo_status) == false) {
                joinTables += " join ec_flight_book_status fbs on fb.awb_no = fbs.awb_no";
            }

            selectFields = "count(*) as cnt";
            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");
            sql = "select count(*) from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "airline.airline_name," +
                           "airline.airline_iata_code," +
                           "sum(fb.chwt) as sum_chwt, " +
                           "sum(fb.grwt) as sum_grwt, " +
                           "sum(fb.total_pieces) as sum_total_pieces, " +
                           "agent.agent_name," +
                           "originAirport.airport_iata_code as origin_iata_code," +
                           "destAirport.airport_iata_code as dest_iata_code," +
                           "fb.awb_no as id," +
                           "fb.flight_date," +
                           "fb.awb_no," +
                           "fb.airline_id, " +
                           "fbs.cargo_status";

            switch(criteria.group_key) {
                case "awbno":
                    selectFields += string.Format(" , count(fb.awb_no) as count_awb,{0} as key_value",groupByName);
                    //selectFields += " , fb.awb_no as key_id ";
                    break;
                case "flight":
                    selectFields += string.Format(" ,flight.flight_id, count(fbs.flight_id) as count_flight_id,{0} as key_value, {1} as key_id","flight.flight_no","flight.flight_id");
                    break;
                case "airline":
                    selectFields += string.Format(" , count(airline.airline_iata_code) as count_airline_iata_code,{0} as key_value, {1} as key_id "," airline.airline_iata_code","fb.airline_id");
                    break;
                case "agent":
                    selectFields += string.Format(" , count(agent.agent_name) as count_agent_name,{0} as key_value",groupByName);
                    break;
                case "origin":
                    selectFields += string.Format(" , count(fb.origin_id) as count_origin_id,{0} as key_value, {1} as key_id","originAirport.airport_iata_code","originAirport.airport_id");
                    break;
                case "destination":
                    selectFields += string.Format(" , count(fb.dest_id) as count_dest_id,{0} as key_value, {1} as key_id","destAirport.airport_iata_code","destAirport.airport_id");
                    break;
                default:
                    groupByName = " fb.awb_no";
                    break;
            }

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK> rows = new List<EC_FLIGHT_BOOK>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK>(sql).ToList();
            }

            return new PaginatedListResult<EC_FLIGHT_BOOK> {
                total = paging.Total, rows = rows
            };
        }

        public List<EC_FLIGHT_BOOK_STATUS_DETAIL> GetStatusDetailList(EC_FLIGHT_BOOK_STATUS_DETAIL criteria) {
            string sql = string.Empty;
            StringBuilder where = new StringBuilder();
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            sql = string.Format("select fbsd.* from ec_flight_book_status_detail fbsd where flight_book_status_id={0}",criteria.flight_book_status_id,where);

            List<EC_FLIGHT_BOOK_STATUS_DETAIL> rows = new List<EC_FLIGHT_BOOK_STATUS_DETAIL>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS_DETAIL>(sql).ToList();
            }
            return rows;
        }

        public PaginatedListResult<EC_FLIGHT_BOOK> Get(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book fb {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.AppendFormat(string.Format(" fb.awb_no='{0}'",criteria.awb_no));
            } else {

                if(criteria.group_id <= 0) {
                    if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                        where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentGroupID));
                    } else if(role == "AA" || role == "AM") {
                        where.Append(string.Format(" fb.group_id in({0})",Authentication.CurrentAgentGroupIDs));
                    } else {
                        where.Append(string.Format(" fb.group_id in({0})",0));
                    }
                } else {
                    where.Append(string.Format(" fb.group_id in({0})",criteria.group_id));
                }

                List<EC_USER_AIRLINE> userAirlines = null;
                List<EC_USER_AGENT> userAgents = null;
                FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

                if(criteria.start_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.end_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
                }
                if(criteria.flight_date.Year > 1900) {
                    where.Append(string.Format(" and fb.flight_date='{0}'",criteria.flight_date.ToString("yyyy-MM-dd")));
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                    string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                    foreach(int id in companyIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.company_id in({0})",ids);
                    userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                    string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                    foreach(int id in airlineIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.airline_id in({0})",ids);
                }


                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.agent_ids) == false) {
                    string[] arr = criteria.agent_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true && (role == "AA" || role == "AM")) {
                    List<int> agentIds = userAgents.Select(q => q.agent_id).Distinct().ToList();
                    foreach(int id in agentIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.agent_id in({0})",ids);
                }


                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                    string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    string tempWhere = string.Empty;
                    if(criteria.start_date.HasValue) {
                        tempWhere += (tempWhere != string.Empty ? " and " : "") + string.Format(" flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd"));
                    }
                    if(criteria.end_date.HasValue) {
                        tempWhere += (tempWhere != string.Empty ? " and " : "") + string.Format(" flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd"));
                    }
                    if(criteria.flight_date.Year > 1900) {
                        tempWhere += (tempWhere != string.Empty ? " and " : "") + string.Format(" flight_date='{0}'",criteria.flight_date.ToString("yyyy-MM-dd"));
                    }
                    tempWhere += (tempWhere != string.Empty ? " and " : "") + string.Format(" flight_id in({0})",ids);
                    sql = string.Format("select distinct awb_no from ec_flight_book_status where {0}",tempWhere);
                    using(EcamContext context = new EcamContext()) {
                        List<string> flightBookAWBNos = context.Database.SqlQuery<string>(sql).ToList();
                        ids = string.Empty;
                        foreach(string awb_no in flightBookAWBNos) {
                            ids += string.Format("'{0}',",awb_no);
                        }
                        if(string.IsNullOrEmpty(ids) == false) {
                            ids = ids.Substring(0,ids.Length - 1);
                            where.AppendFormat(" and fb.awb_no in({0})",ids);
                        } else {
                            where.Append(" and fb.awb_no='0'");
                        }
                    }
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.origin_ids) == false) {
                    string[] arr = criteria.origin_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.origin_id in({0})",ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.dest_ids) == false) {
                    string[] arr = criteria.dest_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.dest in({0})",ids);
                }

                if(criteria.is_non_invoice == "true") {
                    if(criteria.invoice_id > 0) {
                        where.AppendFormat(" and (ifnull(fb.invoice_id,0)=0 or ifnull(fb.invoice_id,0)={0})",criteria.invoice_id);
                    } else {
                        where.Append(" and ifnull(fb.invoice_id,0)=0");
                    }
                }

                if(string.IsNullOrEmpty(criteria.status) == false) {
                    where.AppendFormat(" and ifnull(fb.status,'')='{0}'",criteria.status);
                }

                if(criteria.is_submit_invoice.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_submit_invoice,0)={0}",((criteria.is_submit_invoice ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_received.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_received,0)={0}",((criteria.is_received ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_uplift.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_uplift,0)={0}",((criteria.is_uplift ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_discrepancy.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_discrepancy,0)={0}",((criteria.is_discrepancy ?? false) == true ? "1" : "0"));
                }

                if(string.IsNullOrEmpty(criteria.cargo_types) == false) {
                    string[] arr = criteria.cargo_types.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    string types = "";
                    foreach(string s in arr) {
                        types += string.Format("'{0}',",s);
                    }
                    if(string.IsNullOrEmpty(types) == false) {
                        types = types.Substring(0,types.Length - 1);
                    }
                    where.AppendFormat(" and fb.cargo_type in({0})",types);
                }
            }

            joinTables += " join ec_airline airline on fb.airline_id = airline.airline_id" +
                          " join ec_agent agent on fb.agent_id = agent.agent_id" +
                          " join ec_airport originAirport on fb.origin_id = originAirport.airport_id" +
                          " join ec_airport destAirport on fb.dest_id = destAirport.airport_id" +
                          " left outer join ec_route rou on fb.route_id = rou.route_id" +
                          " join ec_company company on fb.company_id = company.company_id" +
                          " left outer join ec_flight flight on fb.flight_id = flight.flight_id";

            selectFields = "count(distinct fb.awb_no) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            if(paging.SortName == "id")
                paging.SortName = "awb_no";

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "airline.airline_name," +
                           "airline.airline_iata_code," +
                           "company.company_name," +
                           "company.company_code," +
                           "agent.agent_name," +
                           "originAirport.airport_iata_code as origin_iata_code," +
                           "destAirport.airport_iata_code as dest_iata_code," +
                           "agent.agent_name," +
                           "agent.agent_iata_code," +
                           "rou.route_key," +
                           "flight.flight_no," +
                           "fb.awb_no as id," +
                           "fb.*";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK> rows = new List<EC_FLIGHT_BOOK>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK>(sql).ToList();
            }

            if(criteria.is_ignore_child_details == false) {
                if(rows.Count() == 1) {
                    criteria.awb_no = rows.FirstOrDefault().awb_no;
                }
                if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                    EC_FLIGHT_BOOK fb = rows.FirstOrDefault();
                    if(fb != null) {
                        string items = string.Empty;
                        items = fb.booking_items;
                        if(string.IsNullOrEmpty(items) == false) {
                            fb.booking_item_list = JsonConvert.DeserializeObject<List<EC_FLIGHT_BOOK_DETAIL>>(items);
                        }
                        items = fb.received_items;
                        if(string.IsNullOrEmpty(items) == false) {
                            fb.received_item_list = JsonConvert.DeserializeObject<List<EC_FLIGHT_BOOK_DETAIL>>(items);
                        }
                    }
                    sql = string.Format("select  origin.airport_iata_code as origin_iata_code" +
                                        ",dest.airport_iata_code as dest_iata_code" +
                                        ",f.flight_no as flight_no" +
                                        ",ftype.flight_type_name" +
                                        ",concat(user.first_name,' ',user.last_name) as allotment_by_name" +
                                        ",fb.route_id" +
                                        ",rou.route_key" +
                                        ",fs.* from ec_flight_book_status fs" +
                                        " join ec_flight_book fb on fb.awb_no = fs.awb_no" +
                                        " join ec_flight f on fs.flight_id = f.flight_id" +
                                        " left outer join ec_route rou on fb.route_id = rou.route_id" +
                                        " left outer join ec_flight_type ftype on fs.flight_type_id = ftype.flight_type_id" +
                                        " left outer join ec_user user on fs.allotment_by = user.user_id" +
                                        " join ec_airport origin on fs.origin_id = origin.airport_id" +
                                        " join ec_airport dest on fs.dest_id = dest.airport_id" +
                                        " where fs.awb_no='{0}' order by fs.flight_book_status_id",criteria.awb_no);

                    using(EcamContext context = new EcamContext()) {
                        if(fb != null) {
                            List<EC_FLIGHT_BOOK_STATUS> flightBookStatusList = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS>(sql).ToList();
                            fb.flight_details = flightBookStatusList;
                            if(flightBookStatusList.Count() > 0) {
                                string flightIds = "";
                                foreach(var fbs in flightBookStatusList) {
                                    flightIds += fbs.flight_id + ",";
                                }
                                if(flightIds != "") {
                                    flightIds = flightIds.Substring(0,flightIds.Length - 1);
                                }
                                CompanyFlightRepository repo = new CompanyFlightRepository();
                                PaginatedListResult<EC_COMPANY_FLIGHTS> companyFlightDetails = repo.GetFlights(new EC_COMPANY_FLIGHTS_SEARCH {
                                    flight_ids = flightIds,
                                    dest_ids = "-1",
                                    origin_ids = "-1",
                                    flight_date = criteria.flight_date,
                                    company_ids = fb.company_id.ToString(),
                                    group_ids = fb.group_id.ToString()
                                },new Paging { SortName = "company_flight_id",SortOrder = "asc",PageSize = 0 });

                                DateTime startDate = flightBookStatusList.OrderBy(q => q.flight_date).Select(q => q.flight_date).FirstOrDefault();
                                DateTime endDate = flightBookStatusList.OrderByDescending(q => q.flight_date).Select(q => q.flight_date).FirstOrDefault();

                                List<EC_FLIGHT_BOOK_STATUS> companyFlightStatus = this.GetFlightStatus(new EC_FLIGHT_BOOK_STATUS_SEARCH {
                                    flight_ids = flightIds,
                                    start_date = startDate,
                                    end_date = endDate,
                                    company_ids = fb.company_id.ToString(),
                                    status = Ecam.Contracts.Enums.FlightBookingStatus.Approved,
                                    group_ids = fb.group_id.ToString()
                                },new Paging { PageSize = 0 }).rows.ToList();

                                foreach(var fbs in flightBookStatusList) {
                                    int dayIndex = (int)fbs.flight_date.DayOfWeek;
                                    fbs.dest_eta_days = (from q in companyFlightDetails.rows
                                                         where q.flight_id == fbs.flight_id
                                                         && q.day_index == dayIndex
                                                         select (q.dest_eta_days ?? 0)).FirstOrDefault();
                                    fbs.open_days = (from q in companyFlightDetails.rows
                                                     where q.flight_id == fbs.flight_id
                                                     && q.day_index == dayIndex
                                                     select q.open_days).FirstOrDefault();
                                    fbs.total_payload = (from q in companyFlightDetails.rows
                                                         where q.flight_id == fbs.flight_id
                                                         && q.day_index == dayIndex
                                                         select q.payload).FirstOrDefault();
                                    fbs.confirm_chwt = (from q in companyFlightStatus
                                                        where q.flight_id == fbs.flight_id
                                                        && q.origin_id == fbs.origin_id
                                                        && q.dest_id == fbs.dest_id
                                                        && q.origin_etd == fbs.origin_etd
                                                        && q.dest_eta == fbs.dest_eta
                                                        && q.flight_date == fbs.flight_date
                                                        && q.status == Ecam.Contracts.Enums.FlightBookingStatus.Approved
                                                        select q).Sum(q => q.chwt);
                                }
                            }
                        }
                        // Load Files
                        sql = string.Format("select fle.file_id,fle.awb_no,f.file_name,f.file_path from ec_flight_book_files fle join ec_file f on f.file_id = fle.file_id where awb_no='{0}'",criteria.awb_no);
                        fb.files = context.Database.SqlQuery<EC_FLIGHT_BOOK_FILES>(sql).ToList();

                        // Load Charges
                        sql = string.Format("select * from ec_flight_book_charge where awb_no='{0}'",fb.awb_no);
                        List<EC_FLIGHT_BOOK_CHARGE> charges;
                        charges = context.Database.SqlQuery<EC_FLIGHT_BOOK_CHARGE>(sql).ToList();
                        fb.iata_commission = (from q in charges where q.awb_no == fb.awb_no && q.field_code == Ecam.Contracts.Enums.InvoiceFields.IATACommission select q.calc_value).FirstOrDefault();
                        fb.additional_commission = (from q in charges where q.awb_no == fb.awb_no && q.field_code == Ecam.Contracts.Enums.InvoiceFields.AdditionalCommission select q.calc_value).FirstOrDefault();
                        fb.discount = (from q in charges where q.awb_no == fb.awb_no && q.field_code == Ecam.Contracts.Enums.InvoiceFields.Discount select q.calc_value).FirstOrDefault();
                        fb.tds = (from q in charges where q.awb_no == fb.awb_no && q.field_code == Ecam.Contracts.Enums.InvoiceFields.TDS select q.calc_value).FirstOrDefault();
                        fb.service_tax = (from q in charges where q.awb_no == fb.awb_no && q.field_code == Ecam.Contracts.Enums.InvoiceFields.ServiceTax select q.calc_value).FirstOrDefault();

                        // Load Cargo Movements
                        sql = string.Format("select org.airport_iata_code as origin_iata_code,dest.airport_iata_code as dest_iata_code,fli.flight_no,det.detail_id as id,det.* from ec_flight_book_status_detail det" +
                                              " join ec_flight_book_status sta on sta.flight_book_status_id = det.flight_book_status_id" +
                                              " join ec_airport org on org.airport_id = sta.origin_id" +
                                              " join ec_airport dest on dest.airport_id = sta.dest_id" +
                                              " join ec_flight fli on fli.flight_id = sta.flight_id" +
                                              " where sta.awb_no='{0}' order by det.flight_date,det.flight_book_status_id",criteria.awb_no);
                        fb.cargo_details = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS_DETAIL>(sql).ToList();
                    }
                }
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK> { total = paging.Total,rows = rows };
        }

        public List<EC_FLIGHT_PAYLOAD> GetFlightPayLoad(EC_FLIGHT_BOOK_SEARCH criteria) {
            string sql = string.Empty;
            StringBuilder where = new StringBuilder();
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                where.Append(string.Format(" fs.group_id in({0})",Authentication.CurrentGroupID));
            } else if(role == "AA" || role == "AM") {
                where.Append(string.Format(" fs.group_id in({0})",Authentication.CurrentAgentGroupIDs));
            } else {
                where.Append(string.Format(" fs.group_id in({0})",0));
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                foreach(int id in companyIds) {
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fs.company_id in({0})",ids);
                userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fs.flight_id in({0})",ids);
            }

            int dayIndex = 0;
            if(criteria.flight_date.Year > 1900) {
                dayIndex = ((int)criteria.flight_date.DayOfWeek);
            }
            if(dayIndex > 0) {
                where.Append(string.Format(" and fst.day_index={0}",dayIndex));
            }

            sql = string.Format("select fs.company_id,fs.flight_id,fli.flight_no,fst.payload,ori.airport_iata_code as origin_iata_code,dest.airport_iata_code as dest_iata_code from ec_company_flight fs " +
                  " join ec_company_flight_setting fst on fst.company_flight_id = fs.company_flight_id " +
                  " join ec_flight fli on fs.flight_id = fli.flight_id " +
                  " join ec_airport ori on fs.origin_id = ori.airport_id " +
                  " join ec_airport dest on fs.dest_id = dest.airport_id " +
                  " where {1} ",criteria.flight_date.ToString("yyyy-MM-dd"),where);

            List<EC_FLIGHT_PAYLOAD> rows = new List<EC_FLIGHT_PAYLOAD>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_PAYLOAD>(sql).ToList();
                foreach(var row in rows) {
                    row.flight_date = criteria.flight_date;
                }
            }
            return rows;
        }

        public ec_flight_book GetFlightBook(string awb_no) {
            using(EcamContext context = new EcamContext()) {
                return context.ec_flight_book.Where(q => q.awb_no == awb_no).FirstOrDefault();
            }
        }

        public void DeleteFlightBookStatus(string awb_no) {
            using(EcamContext context = new EcamContext()) {
                List<ec_flight_book_status> flightBookStatus = context.ec_flight_book_status.Where(q => q.awb_no == awb_no).ToList();
                foreach(var fbs in flightBookStatus) {
                    context.ec_flight_book_status.Remove(fbs);
                }
                context.SaveChanges();
            }
        }

        public ec_flight_book_status GetFlightBookStatus(int id) {
            using(EcamContext context = new EcamContext()) {
                return context.ec_flight_book_status.Where(q => q.id == id).FirstOrDefault();
            }
        }

        public void UpdateFlightBookStatus(int id,string status) {
            ec_flight_book flightBook = null;
            using(EcamContext context = new EcamContext()) {
                var fbs = context.ec_flight_book_status.Where(q => q.id == id).FirstOrDefault();
                if(fbs != null) {
                    if(fbs.status == Ecam.Contracts.Enums.FlightBookingStatus.Pending) {
                        fbs.status = status;
                        fbs.allotment_by = Authentication.CurrentUserID;
                        context.Entry(fbs).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        flightBook = context.ec_flight_book.Where(q => q.awb_no == fbs.awb_no).FirstOrDefault();
                    }
                }
            }
            if(flightBook != null) {
                flightBook.UpdateStatus();
            }
        }

        public void CancelBooking(string awbnos) {
            using(EcamContext context = new EcamContext()) {
                string[] arr = awbnos.Split((",").ToCharArray());
                List<string> processAWBNos = new List<string>();
                foreach(string strid in arr) {
                    if(string.IsNullOrEmpty(strid) == false) {
                        processAWBNos.Add(strid);
                    }
                }
                if(processAWBNos.Count() > 0) {
                    var flightBooks = context.ec_flight_book.Where(q => processAWBNos.Contains(q.awb_no) == true).ToList();
                    foreach(var fb in flightBooks) {
                        if(fb.status == Ecam.Contracts.Enums.FlightBookingStatus.Pending) {
                            fb.status = Ecam.Contracts.Enums.FlightBookingStatus.Canceled;
                            context.Entry(fb).State = System.Data.Entity.EntityState.Modified;
                            List<ec_flight_book_status> statusList = context.ec_flight_book_status.Where(q => q.awb_no == fb.awb_no).ToList();
                            foreach(var fbs in statusList) {
                                fbs.status = Ecam.Contracts.Enums.FlightBookingStatus.Canceled;
                                context.Entry(fbs).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public void DeleteBooking(string awbnos) {
            using(EcamContext context = new EcamContext()) {
                List<ec_awb_lot_detail> lotDetails = new List<ec_awb_lot_detail>();
                string[] arr = awbnos.Split((",").ToCharArray());
                List<string> processAWBNos = new List<string>();
                string awbnoQuery = string.Empty;
                foreach(string strid in arr) {
                    if(string.IsNullOrEmpty(strid) == false) {
                        processAWBNos.Add(strid);
                        awbnoQuery += string.Format("'{0}',",strid);
                    }
                }
                if(string.IsNullOrEmpty(awbnoQuery) == false) {
                    awbnoQuery = awbnoQuery.Substring(0,awbnoQuery.Length - 1);
                }
                if(processAWBNos.Count() > 0) {
                    var flightBooks = context.ec_flight_book.Where(q => processAWBNos.Contains(q.awb_no) == true).ToList();
                    foreach(var fb in flightBooks) {
                        if((fb.awb_lot_id ?? 0) > 0 && (fb.awb_agent_request_id ?? 0) > 0) {
                            lotDetails.Add(new ec_awb_lot_detail {
                                agent_id = fb.agent_id,
                                awb_agent_request_id = fb.awb_agent_request_id,
                                awb_lot_id = (fb.awb_lot_id ?? 0),
                                awb_no = fb.awb_no,
                                status = Ecam.Contracts.Enums.AWBLotStatus.Assigned
                            });
                        }
                        context.ec_flight_book.Remove(fb);
                    }
                    context.SaveChanges();
                    string sql = string.Format("delete from ec_flight_book_status where awb_no in({0})",awbnoQuery);
                    MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                    foreach(var lot in lotDetails) {
                        int? cnt = (from q in context.ec_awb_lot_detail
                                    where q.awb_no == lot.awb_no
                                    select q.awb_no).Count();
                        if((cnt ?? 0) <= 0) {
                            context.ec_awb_lot_detail.Add(lot);
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpliftBooking(string awbnos) {
            using(EcamContext context = new EcamContext()) {
                string[] arr = awbnos.Split((",").ToCharArray());
                List<string> processAWBNos = new List<string>();
                foreach(string strid in arr) {
                    if(string.IsNullOrEmpty(strid) == false) {
                        processAWBNos.Add(strid);
                    }
                }
                if(processAWBNos.Count() > 0) {
                    var flightBooks = context.ec_flight_book.Where(q => processAWBNos.Contains(q.awb_no) == true).ToList();
                    foreach(var fb in flightBooks) {
                        fb.is_uplift = true;
                        context.Entry(fb).State = System.Data.Entity.EntityState.Modified;
                        var flights = context.ec_flight_book_status.Where(q => q.awb_no == fb.awb_no).ToList();
                        foreach(var fli in flights) {
                            fli.is_uplift = true;
                            context.Entry(fli).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateSentFFR(int id,bool isSentFFR) {
            ec_flight_book flightBook = null;
            using(EcamContext context = new EcamContext()) {
                var fbs = context.ec_flight_book_status.Where(q => q.id == id).FirstOrDefault();
                if(fbs != null) {
                    if(fbs.status == Ecam.Contracts.Enums.FlightBookingStatus.Pending) {
                        fbs.is_sent_ffr = isSentFFR;
                        context.Entry(fbs).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        flightBook = context.ec_flight_book.Where(q => q.awb_no == fbs.awb_no).FirstOrDefault();
                    }
                }
            }
            if(flightBook != null) {
                flightBook.UpdateStatus();
            }
        }

        private List<string> GetAWBNos(string flightIds,EC_FLIGHT_BOOK_STATUS_SEARCH criteria) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status fbs {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            where.Append("fbs.flight_book_status_id>0");

            if(criteria.start_date.HasValue) {
                where.Append(string.Format(" and fbs.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            }

            if(criteria.end_date.HasValue) {
                where.Append(string.Format(" and fbs.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
            }

            if(string.IsNullOrEmpty(flightIds) == false)
                where.Append(string.Format(" and fbs.flight_id in({0})",flightIds));

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                foreach(var companyId in companyIds) {
                    if(companyId > 0) {
                        ids += companyId + ",";
                    }
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
            }
            if(string.IsNullOrEmpty(ids) == false) {
                where.AppendFormat(" and {0}.company_id in({1})","fb",ids);
                userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                foreach(var airlineId in airlineIds) {
                    if(airlineId > 0) {
                        ids += airlineId + ",";
                    }
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
            }
            if(string.IsNullOrEmpty(ids) == false) {
                where.AppendFormat(" and {0}.airline_id in({1})","fb",ids);
            }


            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    if(id > 0)
                        ids += id + ",";
                }
            }

            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and {0}.flight_id in({1})","fbs",ids);
            }

            if(string.IsNullOrEmpty(criteria.status) == false) {
                where.AppendFormat(" and {0}.status='{1}'","fb",criteria.status);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.cargo_status) == false) {
                string[] arr = criteria.cargo_status.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                foreach(string sta in arr) {
                    ids += string.Format("'{0}',",sta);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                where.AppendFormat(" and {0}.cargo_status in({1})","fb",ids);
            }

            if(criteria.is_uplift.HasValue) {
                where.AppendFormat(" and ifnull(fbs.is_uplift,0)={0}",((criteria.is_uplift ?? false) == true ? "1" : "0"));
            }

            if(criteria.is_received.HasValue) {
                where.AppendFormat(" and ifnull(fbs.is_received,0)={0}",((criteria.is_received ?? false) == true ? "1" : "0"));
            }

            joinTables = " join ec_flight_book fb on fb.awb_no = fbs.awb_no";

            selectFields = "distinct fb.awb_no";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<string> rows = new List<string>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<string>(sql).ToList();
            }
            return rows;
        }

        public PaginatedListResult<EC_FLIGHT_BOOK> GetTransitList(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book fb {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            where.Append(string.Format(" {0}.group_id={1}","fb",Authentication.CurrentGroupID));

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.Append(string.Format(" and {0}.awb_no='{1}'","fb",criteria.awb_no));
            } else {

                if(criteria.start_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
                }

                if(criteria.end_date.HasValue) {
                    where.Append(string.Format(" and fb.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                    string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                    foreach(var companyId in companyIds) {
                        if(companyId > 0) {
                            ids += companyId + ",";
                        }
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    where.AppendFormat(" and {0}.company_id in({1})","fb",ids);
                    userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                    string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                    foreach(var airlineId in airlineIds) {
                        if(airlineId > 0) {
                            ids += airlineId + ",";
                        }
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    where.AppendFormat(" and {0}.airline_id in({1})","fb",ids);
                }


                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                    string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        if(id > 0)
                            ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }

                if(string.IsNullOrEmpty(ids) == false) {
                    List<string> awbnos = this.GetAWBNos(ids,criteria);
                    ids = string.Empty;
                    if(awbnos.Count() <= 0) {
                        awbnos.Add("-1");
                    }
                    if(awbnos.Count() > 0) {
                        foreach(string strawbno in awbnos) {
                            ids += string.Format("'{0}',",strawbno);
                        }
                    }
                    if(string.IsNullOrEmpty(ids) == false) {
                        ids = ids.Substring(0,ids.Length - 1);
                    }
                    if(string.IsNullOrEmpty(ids) == false) {
                        where.AppendFormat(" and {0}.awb_no in({1})","fb",ids);
                    }
                }

                if(string.IsNullOrEmpty(criteria.status) == false) {
                    where.AppendFormat(" and {0}.status='{1}'","fb",criteria.status);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.cargo_status) == false) {
                    string[] arr = criteria.cargo_status.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    foreach(string sta in arr) {
                        ids += string.Format("'{0}',",sta);
                    }
                    if(string.IsNullOrEmpty(ids) == false) {
                        ids = ids.Substring(0,ids.Length - 1);
                    }
                    where.AppendFormat(" and ({0}.cargo_status in({1}) or ifnull(fb.cargo_status,'')='')","fb",ids);
                }

                if(criteria.is_uplift.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_uplift,0)={0}",((criteria.is_uplift ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_received.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_received,0)={0}",((criteria.is_received ?? false) == true ? "1" : "0"));
                }
            }

            joinTables += " join ec_airline airline on fb.airline_id = airline.airline_id" +
                       " join ec_agent agent on fb.agent_id = agent.agent_id" +
                       " join ec_airport originAirport on fb.origin_id = originAirport.airport_id" +
                       " join ec_airport destAirport on fb.dest_id = destAirport.airport_id" +
                       " left outer join ec_route rou on fb.route_id = rou.route_id" +
                       " join ec_company company on fb.company_id = company.company_id" +
                       " left outer join ec_flight flight on fb.flight_id = flight.flight_id";

            selectFields = "count(distinct fb.awb_no) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "airline.airline_name," +
                           "airline.airline_iata_code," +
                           "company.company_name," +
                           "company.company_code," +
                           "agent.agent_name," +
                           "originAirport.airport_iata_code as origin_iata_code," +
                           "destAirport.airport_iata_code as dest_iata_code," +
                           "agent.agent_name," +
                           "rou.route_key," +
                           "flight.flight_no," +
                           "fb.awb_no as id," +
                           "fb.*";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK> rows = new List<EC_FLIGHT_BOOK>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK>(sql).ToList();
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK> {
                total = paging.Total, rows = rows
            };
        }

        public PaginatedListResult<EC_FLIGHT_BOOK_STATUS> GetFlightStatus(EC_FLIGHT_BOOK_STATUS_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status fbs {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.Append(string.Format(" {0}.awb_no='{1}'","fb",criteria.awb_no));
            } else {
                if(string.IsNullOrEmpty(criteria.group_ids) == true) {
                    if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                        where.Append(string.Format(" {0}.group_id={1}","fb",Authentication.CurrentGroupID));
                    } else if(role == "AA" || role == "AM") {
                        where.Append(string.Format(" {0}.group_id in({1})","fb",Authentication.CurrentAgentGroupIDs));
                    } else {
                        where.Append(string.Format(" {0}.group_id=0","fb"));
                    }
                } else {
                    where.Append(string.Format("{0}.group_id in({1})","fb",Helper.ConvertStringIds(criteria.group_ids)));
                }

                if(criteria.start_date.HasValue) {
                    where.Append(string.Format(" and fbs.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
                }

                if(criteria.end_date.HasValue) {
                    where.Append(string.Format(" and fbs.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                    string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        if(id > 0)
                            ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and {0}.flight_id in({1})","fbs",ids);
                }

                if(string.IsNullOrEmpty(criteria.status) == false) {
                    where.AppendFormat(" and {0}.status='{1}'","fbs",criteria.status);
                }

                List<EC_USER_AIRLINE> userAirlines = null;
                List<EC_USER_AGENT> userAgents = null;
                FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                    string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                    foreach(var companyId in companyIds) {
                        if(companyId > 0) {
                            ids += companyId + ",";
                        }
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    where.AppendFormat(" and {0}.company_id in({1})","fb",ids);
                    userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
                }

                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                    string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                    foreach(var airlineId in airlineIds) {
                        if(airlineId > 0) {
                            ids += airlineId + ",";
                        }
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    where.AppendFormat(" and {0}.airline_id in({1})","fb",ids);
                }

                if(criteria.is_uplift.HasValue) {
                    where.AppendFormat(" and ifnull(fbs.is_uplift,0)={0}",((criteria.is_uplift ?? false) == true ? "1" : "0"));
                }

                if(criteria.is_received.HasValue) {
                    where.AppendFormat(" and ifnull(fb.is_received,0)={0}",((criteria.is_received ?? false) == true ? "1" : "0"));
                }
            }
            joinTables = " join ec_flight_book fb on fb.awb_no = fbs.awb_no" +
                            " join ec_flight f on fbs.flight_id = f.flight_id" +
                            " join ec_agent ag on fb.agent_id = ag.agent_id" +
                            " left outer join ec_flight_type ftype on fbs.flight_type_id = ftype.flight_type_id" +
                            " left outer join ec_user user on fbs.allotment_by = user.user_id" +
                            " join ec_airport origin on fbs.origin_id = origin.airport_id" +
                            " join ec_airport dest on fbs.dest_id = dest.airport_id";

            selectFields = "count(distinct fbs.flight_book_status_id) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            if(paging.SortName == "id")
                paging.SortName = "fbs.flight_book_status_id";

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = " origin.airport_iata_code as origin_iata_code" +
                            ",dest.airport_iata_code as dest_iata_code" +
                            ",f.flight_no as flight_no" +
                            ",ftype.flight_type_name" +
                            ",concat(user.first_name,' ',user.last_name) as allotment_by_name" +
                            ",ag.agent_name" +
                            ",fbs.*,fb.is_received,fb.chwt,fb.grwt,fb.mc,fb.awb_no";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);
             
            List<EC_FLIGHT_BOOK_STATUS> rows = new List<EC_FLIGHT_BOOK_STATUS>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS>(sql).ToList();
                if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                    sql = string.Format("select det.detail_id as id,det.* from ec_flight_book_status_detail det join ec_flight_book_status fbs on fbs.flight_book_status_id = det.flight_book_status_id where fbs.awb_no='{0}'",criteria.awb_no);
                    List<EC_FLIGHT_BOOK_STATUS_DETAIL> details = context.Database.SqlQuery<EC_FLIGHT_BOOK_STATUS_DETAIL>(sql).ToList();
                    foreach(var row in rows) {
                        row.details = (from q in details
                                       where q.flight_book_status_id == row.flight_book_status_id
                                       select q).ToList();
                    }
                }
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK_STATUS> { total = paging.Total,rows = rows };
        }

        public List<AutoCompleteListExtend> SearchFlights(int companyId,int originId,int destId,DateTime? flightDate,int flightId,int maxDays) {
            string groupIds = string.Empty;
            string role = Authentication.CurrentRole;
            if(role == "AA" || role == "AM") {
                groupIds = Authentication.CurrentAgentGroupIDs;
            } else {
                groupIds = Authentication.CurrentGroupID.ToString();
            }
            string prefix = "cf";
            string settingPrefix = "cfs";

            string selectFields = string.Format("{0}.company_flight_id,",prefix) +
                           string.Format("{0}.flight_id,",prefix) +
                           string.Format("{0}.flight_allotment_id,",prefix) +
                           string.Format("{0}.origin_id,",prefix) +
                           string.Format("{0}.dest_id,",prefix) +
                           string.Format("{0}.dest1_id,",prefix) +
                           string.Format("{0}.dest2_id,",prefix) +
                           string.Format("{0}.open_days,",prefix) +
                           string.Format("{0}.group_id,",prefix) +
                           string.Format("{0}.is_truck,",prefix) +

                           string.Format("{0}.day_index,",settingPrefix) +
                           string.Format("{0}.origin_etd,",settingPrefix) +
                           string.Format("{0}.dest1_eta,",settingPrefix) +
                           string.Format("{0}.origin1_etd,",settingPrefix) +
                           string.Format("{0}.dest2_eta,",settingPrefix) +
                           string.Format("{0}.origin2_etd,",settingPrefix) +
                           string.Format("{0}.dest_eta,",settingPrefix) +
                           string.Format("{0}.dest_eta_days,",settingPrefix) +
                           string.Format("{0}.flight_type_id,",settingPrefix) +
                           string.Format("{0}.payload,",settingPrefix) +

                           "flightType.flight_type_name," +
                           "flight.flight_name," +
                           "flight.flight_no," +
                           "origin.airport_name as origin_name," +
                           "origin.airport_iata_code as origin_iata_code," +
                           "dest.airport_name as dest_name," +
                           "dest.airport_iata_code as dest_iata_code," +
                           "dest1.airport_name as dest1_name," +
                           "dest1.airport_iata_code as dest1_iata_code," +
                           "dest2.airport_name as dest2_name," +
                           "dest2.airport_iata_code as dest2_iata_code," +
                           "flightAllotment.allotment_name as flight_allotment_name"
                           ;


            string joinTables = string.Format(" join ec_company_flight_setting {0} on {0}.company_flight_id = {1}.company_flight_id",settingPrefix,prefix) +
                         string.Format(" left outer join ec_airport origin on origin.airport_id = {0}.origin_id",prefix) +
                         string.Format(" left outer join ec_airport dest on dest.airport_id = {0}.dest_id",prefix) +
                         string.Format(" left outer join ec_airport dest1 on dest1.airport_id = {0}.dest1_id",prefix) +
                         string.Format(" left outer join ec_airport dest2 on dest2.airport_id = {0}.dest2_id",prefix) +
                         string.Format(" join ec_flight flight on flight.flight_id = {0}.flight_id",prefix) +
                         string.Format(" left outer join ec_flight_type flightType on flightType.flight_type_id = {0}.flight_type_id",settingPrefix) +
                         string.Format(" left outer join ec_flight_allotment flightAllotment on flightAllotment.flight_allotment_id = {0}.flight_allotment_id",prefix)
                         ;

            string sql = "select " + selectFields + " from ec_company_flight cf " + joinTables;
            sql += string.Format(" where cf.group_id in({0}) and cf.company_id = {1}",groupIds,companyId);
            if(flightId > 0)
                sql += string.Format(" and cf.flight_id={0}",flightId);

            //sql += string.Format(" and ((cf.origin_id = {0} and cf.dest_id = {1}) || (cf.origin_id = {0} and cf.dest1_id = {1}) || (cf.dest1_id = {0} and cf.dest2_id = {1}) || (cf.dest2_id = {0} and cf.dest_id = {1})) order by flight.flight_no,cfs.day_index;",originId,destId);

            List<EC_COMPANY_FLIGHTS> rows = new List<EC_COMPANY_FLIGHTS>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_COMPANY_FLIGHTS>(sql).ToList();
            }

            List<AutoCompleteListExtend> returnList = new List<AutoCompleteListExtend>();

            DateTime dt = Convert.ToDateTime("01/01/1900");
            if(flightDate.HasValue == false) {
                flightDate = Convert.ToDateTime("01/01/1900");
            }
            int i;
            int dayIndex = 0;
            string flightStartTime = string.Empty;
            string flightEndTime = string.Empty;
            int flightOriginID = 0;
            int flightDestinationID = 0;
            string flightOriginAirportName = "";
            string flightDestinationAirportName = "";
            string flightOriginAirportIATACode = "";
            string flightDestinationAirportIATACode = "";


            List<int> flightIDsList = rows.Select(q => q.flight_id).Distinct().ToList();
            string flightIds = string.Empty;
            foreach(int id in flightIDsList) {
                if(id > 0)
                    flightIds += id + ",";
            }
            if(flightIds != "") {
                flightIds = flightIds.Substring(0,flightIds.Length - 1);
            }
            List<EC_FLIGHT_BOOK_STATUS> companyFlightStatus = this.GetFlightStatus(new EC_FLIGHT_BOOK_STATUS_SEARCH {
                flight_ids = flightIds,
                start_date = flightDate,
                end_date = flightDate.Value.AddDays(14),
                company_ids = companyId.ToString(),
                status = Ecam.Contracts.Enums.FlightBookingStatus.Approved
            },new Paging { PageSize = 0 }).rows.ToList();

            List<EC_FLIGHT_BOOK_STATUS> flightBookStatusDetails = new List<EC_FLIGHT_BOOK_STATUS>();

            i = 0;
            dayIndex = 0;
            dt = Convert.ToDateTime("01/01/1900");
            for(i = 0;i < maxDays;i++) {
                DateTime fd = flightDate.Value.AddDays(i).Date;
                dayIndex = ((int)fd.DayOfWeek);
                List<EC_COMPANY_FLIGHTS> flights = rows.Where(q => q.day_index == dayIndex).ToList();
                dt = fd;
                foreach(var flightDetail in flights) {

                    flightStartTime = string.Empty;
                    flightEndTime = string.Empty;
                    flightOriginID = 0;
                    flightDestinationID = 0;
                    flightOriginAirportName = "";
                    flightDestinationAirportName = "";
                    flightOriginAirportIATACode = "";
                    flightDestinationAirportIATACode = "";

                    if(flightDetail.origin_id == originId
                        && flightDetail.dest_id == destId
                        && flightDetail.day_index == dayIndex) {

                        flightStartTime = flightDetail.origin_etd;
                        flightEndTime = flightDetail.dest_eta;

                        flightOriginID = flightDetail.origin_id;
                        flightOriginAirportName = flightDetail.origin_name;
                        flightOriginAirportIATACode = flightDetail.origin_iata_code;

                        flightDestinationID = flightDetail.dest_id;
                        flightDestinationAirportName = flightDetail.dest_name;
                        flightDestinationAirportIATACode = flightDetail.dest_iata_code;

                    } else if(flightDetail.origin_id == originId
                        && flightDetail.dest1_id == destId
                        && flightDetail.day_index == dayIndex) {

                        flightStartTime = flightDetail.origin_etd;
                        flightEndTime = flightDetail.dest1_eta;

                        flightOriginID = flightDetail.origin_id;
                        flightOriginAirportName = flightDetail.origin_name;
                        flightOriginAirportIATACode = flightDetail.origin_iata_code;

                        flightDestinationID = (flightDetail.dest1_id ?? 0);
                        flightDestinationAirportName = flightDetail.dest1_name;
                        flightDestinationAirportIATACode = flightDetail.dest1_iata_code;

                    } else if(flightDetail.dest1_id == originId
                        && flightDetail.dest2_id == destId
                        && flightDetail.day_index == dayIndex) {

                        flightStartTime = flightDetail.origin1_etd;
                        flightEndTime = flightDetail.dest2_eta;

                        flightOriginID = (flightDetail.dest1_id ?? 0);
                        flightOriginAirportName = flightDetail.dest1_name;
                        flightOriginAirportIATACode = flightDetail.dest1_iata_code;

                        flightDestinationID = (flightDetail.dest2_id ?? 0);
                        flightDestinationAirportName = flightDetail.dest2_name;
                        flightDestinationAirportIATACode = flightDetail.dest2_iata_code;

                    } else if(flightDetail.dest2_id == originId
                         && flightDetail.dest_id == destId
                         && flightDetail.day_index == dayIndex) {

                        flightStartTime = flightDetail.origin2_etd;
                        flightEndTime = flightDetail.dest_eta;

                        flightOriginID = (flightDetail.dest2_id ?? 0);
                        flightOriginAirportName = flightDetail.dest2_name;
                        flightOriginAirportIATACode = flightDetail.dest2_iata_code;

                        flightDestinationID = flightDetail.dest_id;
                        flightDestinationAirportName = flightDetail.dest_name;
                        flightDestinationAirportIATACode = flightDetail.dest_iata_code;

                    }

                    if(string.IsNullOrEmpty(flightStartTime) == false && string.IsNullOrEmpty(flightEndTime) == false && dt.Year > 1900) {

                        flightBookStatusDetails.Add(new EC_FLIGHT_BOOK_STATUS {

                            flight_id = flightDetail.flight_id,
                            flight_no = flightDetail.flight_no,
                            flight_type_name = flightDetail.flight_type_name,
                            flight_type_id = flightDetail.flight_type_id,
                            day_index = flightDetail.day_index,

                            origin_etd = flightStartTime,
                            dest_eta = flightEndTime,
                            dest_eta_days = (flightDetail.dest_eta_days ?? 0),
                            open_days = flightDetail.open_days,

                            dest_id = flightDestinationID,
                            dest_iata_code = flightDestinationAirportIATACode,
                            dest_name = flightDestinationAirportName,

                            origin_id = flightOriginID,
                            origin_iata_code = flightOriginAirportIATACode,
                            origin_name = flightOriginAirportName,

                            flight_date = dt,
                            total_payload = flightDetail.payload,
                            confirm_chwt = (from q in companyFlightStatus
                                            where q.flight_id == flightDetail.flight_id
                                            && q.origin_id == flightOriginID
                                            && q.dest_id == flightDestinationID
                                            && q.origin_etd == flightStartTime
                                            && q.dest_eta == flightEndTime
                                            && q.flight_date == dt
                                            select q).Sum(q => q.chwt),
                        });
                    }
                }
            }

            flightBookStatusDetails = flightBookStatusDetails.OrderBy(q => q.flight_date).ToList();
            foreach(var fsd in flightBookStatusDetails) {
                returnList.Add(new AutoCompleteListExtend {
                    label = fsd.flight_no + " / " + fsd.flight_date.ToString("dd MMM") + " / ETD " + fsd.origin_etd,
                    value = fsd.flight_date.ToString("dd/MMM/yyyy"),
                    other = JsonConvert.SerializeObject(fsd)
                });
            }
            return returnList;
        }

        public List<EC_FLIGHT_BOOK_CHARGE> GetFlightBookCharges(string awbnos) {
            using(EcamContext context = new EcamContext()) {
                List<string> awbNosList = new List<string>();
                if(string.IsNullOrEmpty(awbnos) == false) {
                    string[] arr = awbnos.Split((",").ToCharArray());
                    foreach(string strid in arr) {
                        if(string.IsNullOrEmpty(strid) == false) {
                            awbNosList.Add(strid);
                        }
                    }
                    return (from fbc in context.ec_flight_book_charge
                            join field in context.ec_invoice_field on fbc.field_code equals field.field_code
                            where awbNosList.Contains(fbc.awb_no) == true
                            select new EC_FLIGHT_BOOK_CHARGE {
                                action = fbc.action,
                                calc_percentage = fbc.calc_percentage,
                                calc_value = fbc.calc_value,
                                field_code = fbc.field_code,
                                field_name = field.display_name,
                                awb_no = fbc.awb_no,
                                invoice_charge_id = fbc.invoice_charge_id
                            }).ToList();
                } else {
                    return null;
                }
            }
        }

        public List<EC_FLIGHT_BOOK_FILES> GetFlightBookFiles(string awbnos) {
            using(EcamContext context = new EcamContext()) {
                List<string> awbNosList = new List<string>();
                if(string.IsNullOrEmpty(awbnos) == false) {
                    string[] arr = awbnos.Split((",").ToCharArray());
                    foreach(string strid in arr) {
                        if(string.IsNullOrEmpty(strid) == false) {
                            awbNosList.Add(strid);
                        }
                    }
                    return (from fbf in context.ec_flight_book_files
                            join file in context.ec_file on fbf.file_id equals file.id
                            where awbNosList.Contains(fbf.awb_no) == true
                            select new EC_FLIGHT_BOOK_FILES {
                                awb_no = fbf.awb_no,
                                file_id = fbf.file_id,
                                file_name = file.file_name,
                                file_path = file.file_path
                            }).ToList();
                } else {
                    return null;
                }
            }
        }

        public void Save(ref ec_flight_book flightBook) {
            flightBook.Save();
        }

        public PaginatedListResult<EC_FLIGHT_BOOK_DRAFT> GetDrafts(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book fb {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;

            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            if(string.IsNullOrEmpty(criteria.invoice_period) == true) {
                criteria.invoice_period = Ecam.Contracts.Enums.InvoicePeriod.Fortnightly;
            }

            if(string.IsNullOrEmpty(criteria.payment_term) == true) {
                criteria.payment_term = Ecam.Contracts.Enums.InvoicePaymentTerm.MONTH;
            }

            if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
                where.Append(string.Format("fb.group_id={0}",Authentication.CurrentGroupID));
            } else {
                where.Append(string.Format("fb.group_id={0}",criteria.group_id));
            }

            if(string.IsNullOrEmpty(criteria.awb_no) == false) {
                where.Append(string.Format(" and fb.awb_no='{0}'",criteria.awb_no));
            }

            if(criteria.start_date.HasValue == false) {
                criteria.start_date = Convert.ToDateTime("01/01/1900");
            }

            if(criteria.end_date.HasValue == false) {
                criteria.end_date = DateTime.Now.Date;
            }

            if(criteria.start_date.HasValue) {
                where.Append(string.Format(" and fb.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            }
            if(criteria.end_date.HasValue) {
                where.Append(string.Format(" and fb.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));
            }

            string ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                foreach(int id in companyIds) {
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fb.company_id in({0})",ids);
                userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
            }


            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }

            if(string.IsNullOrEmpty(ids) == true) {
                List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                foreach(int id in airlineIds) {
                    ids += id + ",";
                }
            }

            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fb.airline_id in({0})",ids);
            }


            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.agent_ids) == false) {
                string[] arr = criteria.agent_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> agentIds = userAgents.Select(q => q.agent_id).Distinct().ToList();
                foreach(int id in agentIds) {
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fb.agent_id in({0})",ids);
            }

            where.AppendFormat(" and ifnull(fb.is_submit_invoice,0)=1 and ifnull(fb.invoice_id,0)=0");

            groupByName = "";

            if(criteria.is_ignore_group_by != "1") {
                switch(criteria.invoice_period) {
                    case Ecam.Contracts.Enums.InvoicePeriod.Monthly:
                        groupByName += "group by year(fb.flight_date),month(fb.flight_date)";
                        break;
                    case Ecam.Contracts.Enums.InvoicePeriod.Weekly:
                        groupByName += "group by week(fb.flight_date)";
                        break;
                    case Ecam.Contracts.Enums.InvoicePeriod.Daily:
                        groupByName += "group by fb.flight_date";
                        break;
                    case Ecam.Contracts.Enums.InvoicePeriod.AWBWise:
                        groupByName += "group by fb.awb_no";
                        break;
                    case Ecam.Contracts.Enums.InvoicePeriod.Fortnightly:
                        groupByName += "group by year(fb.flight_date),month(fb.flight_date),(case when day(fb.flight_date) < 16 then '1' else '2' end)";
                        break;
                }
            }

            joinTables = " join ec_airline airline on fb.airline_id = airline.airline_id" +
                         " join ec_agent agent on fb.agent_id = agent.agent_id" +
                         " join ec_company company on fb.company_id = company.company_id"
                         ;

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            sql = "select ifnull(sum(cnt),0) as cnt from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = string.Empty;

            if(criteria.is_ignore_group_by != "1") {
                selectFields += "agent.agent_name," +
                               "fb.agent_id," +
                               "fb.airline_id," +
                               "airline.airline_iata_code," +
                               "airline.airline_name," +
                               "fb.company_id," +
                               "company.company_name," +
                               "count(fb.awb_no) as count_awb,";

                if(criteria.invoice_period == Ecam.Contracts.Enums.InvoicePeriod.AWBWise) {
                    selectFields += "fb.awb_no as awb_no,";
                }
                selectFields += "year(fb.flight_date) as year," +
                               "month(fb.flight_date) as month," +
                               "week(fb.flight_date) as week," +
                               "(case when day(fb.flight_date) < 16 then 1 else 2 end) as fn_number," +
                               "fb.flight_date," +
                               string.Format("'{0}' as period,",criteria.invoice_period) +
                               string.Format("'{0}' as payment_term,",criteria.payment_term) +
                               string.Format("{0} as payment_days,",(criteria.payment_days ?? 0)) +
                               "sum(ifnull(fb.grwt,0)) as sum_grwt," +
                               "sum(ifnull(fb.chwt,0)) as sum_chwt," +
                               "sum(ifnull(fb.final_total,0)) as total_amount"
                               ;
            } else {
                selectFields += "fb.awb_no as awb_no";
            }

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK_DRAFT> rows = new List<EC_FLIGHT_BOOK_DRAFT>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_DRAFT>(sql).ToList();
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK_DRAFT> { total = paging.Total,rows = rows };
        }

        public List<EC_FLIGHT_BOOK_STATUS_DETAIL> GetStatusDetail(int id) {
            using(EcamContext context = new EcamContext()) {
                return (from q in context.ec_flight_book_status_detail
                        where q.id == id
                        select new EC_FLIGHT_BOOK_STATUS_DETAIL {
                            cargo_status = q.cargo_status,
                            chwt = q.chwt,
                            id = q.id,
                            flight_book_status_id = q.flight_book_status_id,
                            flight_date = q.flight_date,
                            grwt = q.chwt,
                            pieces = q.pieces,
                            remarks = q.remarks
                        }).ToList();
            }
        }

        public void UpdateMailLogFlightBookStatusDetail(int id) {
            using(EcamContext context = new EcamContext()) {
                var flightBookStatusDetail = context.ec_flight_book_status_detail.Where(q => q.id == id).FirstOrDefault();
                if(flightBookStatusDetail != null) {
                    flightBookStatusDetail.UpdateMailLog(flightBookStatusDetail.id,flightBookStatusDetail.cargo_status,true);
                }
            }
        }

        public PaginatedListResult<EC_FLIGHT_BOOK_TV> GetTVReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status_detail det {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;

            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            where.Append(string.Format(" det.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            where.Append(string.Format(" and det.flight_date<='{0}'",criteria.end_date.Value.ToString("yyyy-MM-dd")));

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.cargo_status) == false) {
                string[] arr = criteria.cargo_status.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                foreach(string sta in arr) {
                    ids += string.Format("'{0}',",sta);
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                }
                where.AppendFormat(" and {0}.cargo_status in({1})","det",ids);
            }

            //if(role != "EA" && role != "EM" && role != "AA" && role != "AM") {
            where.Append(string.Format(" and fb.group_id={0}",Authentication.CurrentGroupID));
            //} else if(role != "AA" && role != "AM") {
            //    where.Append(string.Format(" and fb.group_id in({0})",Authentication.CurrentAgentGroupIDs));
            //} else {
            //    where.Append(string.Format(" and fb.group_id={0}",-1));
            //}

            /*
            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                foreach(int id in companyIds) {
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fb.company_id in({0})",ids);
                userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
            }


            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                string[] arr = criteria.airline_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }

            if(string.IsNullOrEmpty(ids) == true) {
                List<int> airlineIds = userAirlines.Select(q => q.airline_id).Distinct().ToList();
                foreach(int id in airlineIds) {
                    ids += id + ",";
                }
            }

            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and fb.airline_id in({0})",ids);
            }

            if(role == "AA" || role == "AM") {
                ids = string.Empty;
                if(string.IsNullOrEmpty(criteria.agent_ids) == false) {
                    string[] arr = criteria.agent_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                    int id = 0;
                    foreach(var strid in arr) {
                        int.TryParse(strid,out id);
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == true) {
                    List<int> agentIds = userAgents.Select(q => q.agent_id).Distinct().ToList();
                    foreach(int id in agentIds) {
                        ids += id + ",";
                    }
                }
                if(string.IsNullOrEmpty(ids) == false) {
                    ids = ids.Substring(0,ids.Length - 1);
                    where.AppendFormat(" and fb.agent_id in({0})",ids);
                }
            }
            */

            joinTables = " join ec_flight_book_status sta on det.flight_book_status_id = sta.flight_book_status_id" +
                            " join ec_flight_book fb on fb.awb_no = sta.awb_no" +
                            " join ec_airline ai on ai.airline_id = fb.airline_id" +
                            " join ec_agent ag on ag.agent_id = fb.agent_id" +
                            " join ec_flight fli on sta.flight_id = fli.flight_id" +
                            " join ec_airport ori on sta.origin_id = ori.airport_id" +
                            " join ec_airport dest on sta.dest_id = dest.airport_id";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "sta.awb_no,sta.flight_id,fli.flight_no,ori.airport_iata_code as origin,dest.airport_iata_code as destination,ag.agent_name" +
                            ",ai.airline_iata_code,ai.airline_name,fb.airline_id" +
                            ",det.flight_date,sta.origin_etd,sta.dest_eta,det.grwt,det.chwt,det.pieces,det.cargo_status";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK_TV> rows = new List<EC_FLIGHT_BOOK_TV>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_TV>(sql).ToList();
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK_TV> { total = paging.Total,rows = rows };
        }
        

        public PaginatedListResult<EC_FLIGHT_BOOK_AWBNO_DETAIL> GetAWBNoDetail(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status fbs {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            where.AppendFormat(" fbs.awb_no in({0}) ", criteria.awb_no);

            joinTables = " join ec_flight fli on fbs.flight_id=fli.flight_id" +
                         " join ec_airport originAirport on originAirport.airport_id = fbs.origin_id" +
                         " join ec_airport destiAirport on destiAirport.airport_id = fbs.dest_id ";   

            selectFields = "count(*) as cnt";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, "", "");
            //sql = "select count(*) from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}", from, to);
            }

            orderBy = string.Format("order by {0} {1}", paging.SortName, paging.SortOrder);

            selectFields = "fbs.origin_id," +
                            "fbs.dest_id," +
                            "fbs.flight_id," +
                            "fbs.awb_no," +
                            "fbs.origin_etd," +
                            "fbs.dest_eta," +
                            "fli.flight_no," +
                            "originAirport.airport_iata_code as origin," +
                            "destiAirport.airport_iata_code as dest," +
                            "originAirport.airport_name as origin_name," +
                            "destiAirport.airport_name as dest_name,"+
                            "originAirport.airport_city as origin_city," +
                            "destiAirport.airport_city as dest_city";
                           
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<EC_FLIGHT_BOOK_AWBNO_DETAIL> rows = new List<EC_FLIGHT_BOOK_AWBNO_DETAIL>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_AWBNO_DETAIL>(sql).ToList();
            }

            return new PaginatedListResult<EC_FLIGHT_BOOK_AWBNO_DETAIL> {
                total = paging.Total, rows = rows
            };

        }

        public PaginatedListResult<EC_FLIGHT_BOOK_FLIGHT_DETAIL> GetFlightDetail(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status_detail fbsd {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            //where.AppendFormat(" fbs.flight_id in({0})", criteria.flight_id);

            if(criteria.flight_id > 0) {
                where.AppendFormat(" fbs.flight_id in({0})", criteria.flight_id);
            }

            if(string.IsNullOrEmpty(criteria.awb_no)==false) {
                where.AppendFormat(" fbs.awb_no in({0}) ", criteria.awb_no);
            }
                        
            if(criteria.start_date.HasValue) {
                where.Append(string.Format(" and fbsd.flight_date>='{0}'",criteria.start_date.Value.ToString("yyyy-MM-dd")));
            }

            //origin_id check
            if(criteria.origin_id > 0) {
                where.AppendFormat(" and fbs.origin_id in({0})",criteria.origin_id);
            }
           
            //destination check
            if(criteria.dest_id > 0) {
                where.AppendFormat(" and fbs.dest_id in({0})",criteria.dest_id);
            }
            
            joinTables = "join ec_flight_book_status fbs on fbsd.flight_book_status_id=fbs.`flight_book_status_id`" +
                           " join ec_airport originAirport on originAirport.airport_id = fbs.origin_id" +
                           " join ec_airport destiAirport on destiAirport.airport_id = fbs.dest_id " +                           
                         "  join ec_flight_book fb on fbs.awb_no=fb.awb_no"+
                         "  join ec_flight fli on fli.flight_id=fbs.flight_id";


            selectFields = "count(*) as cnt";
            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");
            //sql = "select count(*) from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "fbsd.flight_book_status_id," +
                            "fbs.flight_id," +
                            "fbs.awb_no, fbsd.flight_date," +
                            "fbsd.pieces, fbsd.`chwt`,fbsd.`grwt`,fb.mc,fb.total_pieces," +
                            "originAirport.airport_iata_code as origin_iata_code," +
                            "destiAirport.airport_iata_code as dest_iata_code," +
                            "fbs.origin_etd as origin_etd, " +
                            "fbs.dest_eta as dest_eta, " +
                            " fli.flight_no," +
                            " fbsd.cargo_status";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK_FLIGHT_DETAIL> rows = new List<EC_FLIGHT_BOOK_FLIGHT_DETAIL>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_FLIGHT_DETAIL>(sql).ToList();
            }

            return new PaginatedListResult<EC_FLIGHT_BOOK_FLIGHT_DETAIL> {
                total = paging.Total, rows = rows
            };
            
        }

        public PaginatedListResult<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT> GetOffBoardFlights(EC_FLIGHT_BOOK_SEARCH criteria, Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_flight_book_status_detail fbsd {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            // int flight_id = 1400;

            if(string.IsNullOrEmpty(criteria.cargo_status) == false) {                
                where.AppendFormat(" ifnull(fbs.cargo_status,'')='{0}'", criteria.cargo_status);
            }

            if(criteria.start_date.HasValue) {
                where.Append(string.Format(" and fbsd.flight_date='{0}'", criteria.start_date.Value.ToString("yyyy-MM-dd")));
            }

            //origin_id check
            if(criteria.origin_id > 0) {
                where.AppendFormat(" and fbs.origin_id in({0})", criteria.origin_id);
            }

            //destination check
            if(criteria.dest_id > 0) {
                where.AppendFormat(" and fbs.dest_id in({0})", criteria.dest_id);
            }

            joinTables = "join ec_flight_book_status fbs on fbsd.flight_book_status_id=fbs.`flight_book_status_id`" +
                        "left join ec_flight fli on fli.flight_id=fbs.flight_id";
            
            selectFields = "count(*) as cnt";
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, "", "");
            //sql = "select count(*) from (" + sql + ") as tbl";

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString, sql));

            if(string.IsNullOrEmpty(paging.SortOrder)) {
                paging.SortOrder = "asc";
            }

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}", from, to);
            }

            orderBy = string.Format("order by {0} {1}", paging.SortName, paging.SortOrder);
            selectFields = "fbsd.flight_book_status_id," +
                            "fbs.flight_id," +
                            "fli.flight_no";
            
            sql = string.Format(sqlFormat, selectFields, joinTables, where, groupByName, orderBy, pageLimit);

            List<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT> rows = new List<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT>(sql).ToList();
            }

            return new PaginatedListResult<EC_FLIGHT_BOOK_OFFBOARD_FLIGHT> {
                total = paging.Total, rows = rows
            };

            //return new PaginatedListResult<EC_FLIGHT_BOOK_AWBNO_DETAIL> { total = paging.Total, rows = rows };
        }
        public PaginatedListResult<EC_FLIGHT_BOOK_MAP> GetMapReportList(EC_FLIGHT_BOOK_SEARCH criteria,Paging paging) {
            StringBuilder where = new StringBuilder();
            string selectFields = "";
            string pageLimit = "";
            string orderBy = "";
            string groupByName = string.Empty;
            string joinTables = string.Empty;
            string sqlFormat = "select {0} from ec_company_flight cfli {1} where {2} {3} {4} {5}";
            string sql = string.Empty;
            string role = Authentication.CurrentRole;
            string ids = string.Empty;
            List<EC_USER_AIRLINE> userAirlines = null;
            List<EC_USER_AGENT> userAgents = null;
            FilterExtension.GetFilterList(ref userAirlines,ref userAgents,(Authentication.CurrentUserID ?? 0));

            if(role == "AA" || role == "AM") {
                where.Append(string.Format(" cfli.group_id in({0})",Authentication.CurrentAgentGroupIDs));
            } else {
                where.Append(string.Format(" cfli.group_id in({0})",Authentication.CurrentGroupID));
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.company_ids) == false) {
                string[] arr = criteria.company_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                List<int> companyIds = userAirlines.Select(q => q.company_id).Distinct().ToList();
                foreach(int id in companyIds) {
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and cfli.company_id in({0})",ids);
                userAirlines = FilterExtension.GetCompanyAirlines(userAirlines,ids);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.flight_ids) == false) {
                string[] arr = criteria.flight_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and cfli.flight_id in({0})",ids);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.origin_ids) == false) {
                string[] arr = criteria.origin_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and cfli.origin_id in({0})",ids);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.dest_ids) == false) {
                string[] arr = criteria.dest_ids.Split((",").ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid,out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0,ids.Length - 1);
                where.AppendFormat(" and cfli.dest_id in({0})",ids);
            }

            if((criteria.day_index ?? 0) > 0) {
                where.AppendFormat(" and setting.day_index={0}",criteria.day_index);
            }

            ids = string.Empty;
            if(string.IsNullOrEmpty(criteria.airline_ids) == false) {
                string[] arr = criteria.airline_ids.Split((",").ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int id = 0;
                foreach(var strid in arr) {
                    int.TryParse(strid, out id);
                    ids += id + ",";
                }
            }
            if(string.IsNullOrEmpty(ids) == false) {
                ids = ids.Substring(0, ids.Length - 1);
                where.AppendFormat(" and airline.airline_id in({0})", ids);
            }

            //where.Append(" and (ifnull(setting.origin_etd,'')!= '' && ifnull(setting.dest_eta,'')!= '')");

            joinTables = " join ec_company_flight_setting setting on cfli.company_flight_id = setting.company_flight_id" +
                            " join ec_company company on cfli.company_id = company.company_id" +
                            " join ec_flight fli on cfli.flight_id = fli.flight_id" +
                            " join ec_airport ori on cfli.origin_id = ori.airport_id" +
                            " left join ec_airline airline on airline.airline_id = fli.airline_id" +
                            " left outer join ec_timezone otz on otz.timezone_id = ori.timezone_id" +
                            " left outer join ec_country oricountry on ori.country_id = oricountry.country_id" +
                            " left outer join ec_airport dest1 on cfli.dest1_id = dest1.airport_id" +
                            " left outer join ec_airport dest2 on cfli.dest2_id = dest2.airport_id" +
                            " join ec_airport dest on cfli.dest_id = dest.airport_id" +
                            " left outer join ec_timezone dtz on dtz.timezone_id = dest.timezone_id" +
                            " left outer join ec_country destcountry on dest.country_id = destcountry.country_id" +
                            "";

            selectFields = "count(*) as cnt";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,"","");

            paging.Total = Convert.ToInt32(MySqlHelper.ExecuteScalar(Ecam.Framework.Helper.ConnectionString,sql));

            if(paging.PageSize > 0) {
                int from = (paging.PageIndex > 1) ? ((paging.PageIndex - 1) * paging.PageSize) : 0;
                int to = paging.PageSize;
                pageLimit = string.Format("limit {0},{1}",from,to);
            }

            orderBy = string.Format("order by {0} {1}",paging.SortName,paging.SortOrder);

            selectFields = "fli.flight_no" +
                            ",ori.airport_iata_code as origin" +
                            ",dest.airport_iata_code as dest" +
                            ",dest1.airport_iata_code as dest1" +
                            ",dest2.airport_iata_code as dest2" +
                            ",ori.airport_city as origin_city" +
                            ",dest.airport_city as dest_city" +
                            ",ori.airport_name as origin_name" +
                            ",dest.airport_name as dest_name" +
                            ",oricountry.country_name as origin_country" +
                            ",destcountry.country_name as dest_country" +
                            ",otz.gmt_offset as origin_gmt_offset" +
                            ",dtz.gmt_offset as dest_gmt_offset" +
                            ",cfli.*,setting.*" +
                            ",airline.airline_id as airline_id, airline.airline_name as airline_name" +
                            ",airline.airline_iata_code as airline_iata_code" +
                            "";

            sql = string.Format(sqlFormat,selectFields,joinTables,where,groupByName,orderBy,pageLimit);

            List<EC_FLIGHT_BOOK_MAP> rows = new List<EC_FLIGHT_BOOK_MAP>();
            using(EcamContext context = new EcamContext()) {
                rows = context.Database.SqlQuery<EC_FLIGHT_BOOK_MAP>(sql).ToList();
            }
            return new PaginatedListResult<EC_FLIGHT_BOOK_MAP> { total = paging.Total,rows = rows };
        }
    }
}
