
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_marketMap:EntityTypeConfiguration<tra_market> {
        public tra_marketMap() {
            // Primary Key
            this.HasKey(t => new { t.symbol,t.trade_date });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.open_price)
                .HasPrecision(13,4);

            this.Property(t => t.high_price)
                .HasPrecision(13,4);

            this.Property(t => t.low_price)
                .HasPrecision(13,4);

            this.Property(t => t.ltp_price)
                .HasPrecision(13,4);

            this.Property(t => t.close_price)
                .HasPrecision(13,4);

            this.Property(t => t.prev_price)
                .HasPrecision(13,4);

            this.Property(t => t.super_trend_signal)
              .HasMaxLength(1);

            this.Property(t => t.ema_5)
                .HasPrecision(13,8);

            this.Property(t => t.ema_20)
                .HasPrecision(13,8);

            this.Property(t => t.ema_cross)
                .HasPrecision(13,8);

            this.Property(t => t.ema_profit)
               .HasPrecision(13,4);

            this.Property(t => t.ema_signal)
             .HasMaxLength(1);

            this.Property(t => t.ema_min_profit)
              .HasPrecision(13,4);

            this.Property(t => t.ema_max_profit)
              .HasPrecision(13,4);

            this.Property(t => t.ema_increase_profit)
             .HasPrecision(13,4);

            this.Property(t => t.ema_min_cross)
            .HasPrecision(13,8);

            // Table & Column Mappings
            this.ToTable("tra_market");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.open_price).HasColumnName("open_price");
            this.Property(t => t.high_price).HasColumnName("high_price");
            this.Property(t => t.low_price).HasColumnName("low_price");
            this.Property(t => t.ltp_price).HasColumnName("ltp_price");
            this.Property(t => t.close_price).HasColumnName("close_price");
            this.Property(t => t.prev_price).HasColumnName("prev_price");

            this.Property(t => t.super_trend_signal).HasColumnName("super_trend_signal");
            this.Property(t => t.is_indicator).HasColumnName("is_indicator");
            this.Property(t => t.ema_5).HasColumnName("ema_5");
            this.Property(t => t.ema_20).HasColumnName("ema_20");
            this.Property(t => t.ema_cross).HasColumnName("ema_cross");
            this.Property(t => t.ema_profit).HasColumnName("ema_profit");
            this.Property(t => t.ema_signal).HasColumnName("ema_signal");
            this.Property(t => t.ema_min_profit).HasColumnName("ema_min_profit");
            this.Property(t => t.ema_max_profit).HasColumnName("ema_max_profit");
            this.Property(t => t.ema_cnt).HasColumnName("ema_cnt");
            this.Property(t => t.ema_increase).HasColumnName("ema_increase");
            this.Property(t => t.ema_increase_profit).HasColumnName("ema_increase_profit");
            this.Property(t => t.ema_min_cross).HasColumnName("ema_min_cross");
        }
    }
}
