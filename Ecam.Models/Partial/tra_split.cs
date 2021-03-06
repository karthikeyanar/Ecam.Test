using Ecam.Framework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Ecam.Models {
    [MetadataType(typeof(tra_splitMD))]
    public partial class tra_split {

        public tra_split _prev_split = null;

        public override void OnSaving() {
            base.OnSaving();
            if(this.id > 0) {
                using(EcamContext context = new EcamContext()) {
                    this._prev_split = (from q in context.tra_split where q.id == this.id select q).FirstOrDefault();
                }
            }
        }

        public override void OnDeleting() {
            if(this.id > 0) {
                using(EcamContext context = new EcamContext()) {
                    this._prev_split = (from q in context.tra_split where q.id == this.id select q).FirstOrDefault();
                }
            }
            base.OnDeleting();
        }

        public override void OnDeleted() {
            tra_split oldSplit = null;
            using(EcamContext context = new EcamContext()) {
                oldSplit = (from q in context.tra_split
                            where q.symbol == this.symbol
                            && q.split_date < this.split_date
                            select q).FirstOrDefault();
            }
            string toDateFilter = string.Empty;
            if(oldSplit != null) {
                toDateFilter = string.Format(" and trade_date>'{0}'",oldSplit.split_date.ToString("yyyy-MM-dd"));
            } else {
                toDateFilter = " ";
            }
            string sql = "";
            if(this._prev_split != null) {
                sql = string.Format("update tra_market set " + Environment.NewLine +
                      "open_price = ifnull(open_price,0)*{0}," + Environment.NewLine +
                      "high_price = ifnull(high_price,0)*{0}," + Environment.NewLine +
                      "low_price = ifnull(low_price,0)*{0}," + Environment.NewLine +
                      "ltp_price = ifnull(ltp_price,0)*{0}," + Environment.NewLine +
                      "close_price = ifnull(close_price,0)*{0}," + Environment.NewLine +
                      "prev_price = ifnull(prev_price,0)*{0}" + Environment.NewLine +
                      " where symbol='{1}' {2} and trade_date<'{3}'" + Environment.NewLine +
                      " ",this._prev_split.split_factor,this._prev_split.symbol,toDateFilter,this._prev_split.split_date.ToString("yyyy-MM-dd"));
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
            }
            base.OnDeleted();
        }

        public override void OnSaved() {
            base.OnSaved();
            this.UpdatePrice();
        }

        public void DeleteManually(int id) {
            tra_split split = null;
            tra_split oldSplit = null;
            using(EcamContext context = new EcamContext()) {
                split = (from q in context.tra_split
                         where q.id == id
                         select q).FirstOrDefault();
                if(split != null) {
                    oldSplit = (from q in context.tra_split
                                where q.symbol == split.symbol
                                && q.split_date < split.split_date
                                select q).FirstOrDefault();
                    string toDateFilter = string.Empty;
                    if(oldSplit != null) {
                        toDateFilter = string.Format(" and trade_date>'{0}'",oldSplit.split_date.ToString("yyyy-MM-dd"));
                    } else {
                        toDateFilter = " ";
                    }
                    string sql = "";
                    sql = string.Format("update tra_market set " + Environment.NewLine +
                            "open_price = ifnull(open_price,0)*{0}," + Environment.NewLine +
                            "high_price = ifnull(high_price,0)*{0}," + Environment.NewLine +
                            "low_price = ifnull(low_price,0)*{0}," + Environment.NewLine +
                            "ltp_price = ifnull(ltp_price,0)*{0}," + Environment.NewLine +
                            "close_price = ifnull(close_price,0)*{0}," + Environment.NewLine +
                            "prev_price = ifnull(prev_price,0)*{0}" + Environment.NewLine +
                            " where symbol='{1}' {2} and trade_date<'{3}'" + Environment.NewLine +
                            " ",split.split_factor,split.symbol,toDateFilter,split.split_date.ToString("yyyy-MM-dd"));
                    MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
                    context.tra_split.Remove(split);
                    context.SaveChanges();
                }
            }
        }

        public void UpdatePrice() {
            tra_split oldSplit = null;
            using(EcamContext context = new EcamContext()) {
                oldSplit = (from q in context.tra_split
                            where q.symbol == this.symbol
                            && q.split_date < this.split_date
                            select q).FirstOrDefault();
            }
            string toDateFilter = string.Empty;
            if(oldSplit != null) {
                toDateFilter = string.Format(" and trade_date>'{0}'",oldSplit.split_date.ToString("yyyy-MM-dd"));
            } else {
                toDateFilter = " ";
            }
            string sql = "";
            if(this._prev_split != null) {
                sql = string.Format("update tra_market set " + Environment.NewLine +
                      "open_price = ifnull(open_price,0)*{0}," + Environment.NewLine +
                      "high_price = ifnull(high_price,0)*{0}," + Environment.NewLine +
                      "low_price = ifnull(low_price,0)*{0}," + Environment.NewLine +
                      "ltp_price = ifnull(ltp_price,0)*{0}," + Environment.NewLine +
                      "close_price = ifnull(close_price,0)*{0}," + Environment.NewLine +
                      "prev_price = ifnull(prev_price,0)*{0}" + Environment.NewLine +
                      " where symbol='{1}' {2} and trade_date<'{3}'" + Environment.NewLine +
                      " ",this._prev_split.split_factor,this._prev_split.symbol,toDateFilter,this._prev_split.split_date.ToString("yyyy-MM-dd"));
                MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
            }

            sql = string.Format("update tra_market set " + Environment.NewLine +
                     "open_price = ifnull(open_price,0)/{0}," + Environment.NewLine +
                     "high_price = ifnull(high_price,0)/{0}," + Environment.NewLine +
                     "low_price = ifnull(low_price,0)/{0}," + Environment.NewLine +
                     "ltp_price = ifnull(ltp_price,0)/{0}," + Environment.NewLine +
                     "close_price = ifnull(close_price,0)/{0}," + Environment.NewLine +
                     "prev_price = ifnull(prev_price,0)/{0}" + Environment.NewLine +
                     " where symbol='{1}' {2} and trade_date<'{3}'" + Environment.NewLine +
                     " ",this.split_factor,this.symbol,toDateFilter,this.split_date.ToString("yyyy-MM-dd"));
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);

            using(EcamContext context = new EcamContext()) {
                decimal prevPrice = 0;
                tra_market prevMaket = (from q in context.tra_market
                                        where q.symbol == this.symbol && q.trade_date < this.split_date
                                        orderby q.trade_date descending
                                        select q).FirstOrDefault();
                if(prevMaket != null) {
                    prevPrice = (prevMaket.open_price ?? 0);
                }
                tra_market market = (from q in context.tra_market
                                     where q.symbol == this.symbol && q.trade_date == this.split_date
                                     select q).FirstOrDefault();
                if(market != null) {
                    market.prev_price = prevPrice;
                    context.Entry(market).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            //sql = string.Format("update tra_market set " + Environment.NewLine +
            //      " percentage = ((ifnull(ltp_price,0)-ifnull(prev_price,0))/ifnull(prev_price,0))*100 " + Environment.NewLine +
            //      " where symbol='{0}' " + Environment.NewLine +
            //      " ",this.symbol);
            //MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
            //TradeHelper.CreateAVG(this.symbol,this.split_date);
        }

        public class tra_splitMD {
            [Required(ErrorMessage = "Symbol is required")]
            [StringLength(50,ErrorMessage = "Symbol must be under 50 characters.")]
            public global::System.String symbol {
                get;
                set;
            }

            [Required(ErrorMessage = "Split Date is required")]
            [DateRange(ErrorMessage = "Split Date is required")]
            public DateTime split_date { get; set; }

            [Required(ErrorMessage = "Split Factor is required")]
            [Range(typeof(decimal),"0.1","79228162514264337593543950335",ErrorMessage = "Split Factor is required")]
            public decimal split_factor { get; set; }
        }
    }
}
