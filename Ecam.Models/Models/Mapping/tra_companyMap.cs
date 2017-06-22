
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models
{
    public partial class tra_companyMap : EntityTypeConfiguration<tra_company>
    {
        public tra_companyMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.company_name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.open_price)
                .HasPrecision(13, 4);

            this.Property(t => t.high_price)
                .HasPrecision(13, 4);

            this.Property(t => t.low_price)
                .HasPrecision(13, 4);

            this.Property(t => t.ltp_price)
                .HasPrecision(13, 4);

            this.Property(t => t.close_price)
                .HasPrecision(13, 4);

            this.Property(t => t.prev_price)
                .HasPrecision(13, 4);

            this.Property(t => t.week_52_high)
                .HasPrecision(13, 4);

            this.Property(t => t.week_52_low)
                .HasPrecision(13, 4);

            this.Property(t => t.day_5)
                .HasPrecision(13, 4);

            this.Property(t => t.day_10)
                .HasPrecision(13, 4);

            this.Property(t => t.day_15)
                .HasPrecision(13, 4);

            this.Property(t => t.day_20)
                .HasPrecision(13, 4);

            this.Property(t => t.day_25)
                .HasPrecision(13, 4);

            this.Property(t => t.day_30)
                .HasPrecision(13, 4);

            this.Property(t => t.day_35)
                .HasPrecision(13, 4);

            this.Property(t => t.day_60)
                .HasPrecision(13, 4);

            this.Property(t => t.day_40)
                .HasPrecision(13, 4);

            this.Property(t => t.day_45)
                .HasPrecision(13, 4);

            this.Property(t => t.day_50)
                .HasPrecision(13, 4);

            this.Property(t => t.day_55)
                .HasPrecision(13, 4);

            this.Property(t => t.day_65)
                .HasPrecision(13, 4);

            this.Property(t => t.day_70)
                .HasPrecision(13, 4);

            this.Property(t => t.day_75)
                .HasPrecision(13, 4);

            this.Property(t => t.day_80)
                .HasPrecision(13, 4);

            this.Property(t => t.day_85)
                .HasPrecision(13, 4);

            this.Property(t => t.day_90)
                .HasPrecision(13, 4);

            this.Property(t => t.mf_qty)
                .HasPrecision(13, 4);

            this.Property(t => t.day_2)
                .HasPrecision(13, 4);

            this.Property(t => t.day_3)
                .HasPrecision(13, 4);

            this.Property(t => t.day_4)
                .HasPrecision(13, 4);

            this.Property(t => t.day_1)
                .HasPrecision(13, 4);

            this.Property(t => t.rsi)
                .HasPrecision(13, 4);

            this.Property(t => t.prev_rsi)
              .HasPrecision(13, 4);

            // Table & Column Mappings
            this.ToTable("tra_company");
            this.Property(t => t.id).HasColumnName("company_id");
            this.Property(t => t.company_name).HasColumnName("company_name");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.open_price).HasColumnName("open_price");
            this.Property(t => t.high_price).HasColumnName("high_price");
            this.Property(t => t.low_price).HasColumnName("low_price");
            this.Property(t => t.ltp_price).HasColumnName("ltp_price");
            this.Property(t => t.close_price).HasColumnName("close_price");
            this.Property(t => t.prev_price).HasColumnName("prev_price");
            this.Property(t => t.week_52_high).HasColumnName("week_52_high");
            this.Property(t => t.week_52_low).HasColumnName("week_52_low");
            this.Property(t => t.is_book_mark).HasColumnName("is_book_mark");
            this.Property(t => t.day_5).HasColumnName("day_5");
            this.Property(t => t.day_10).HasColumnName("day_10");
            this.Property(t => t.day_15).HasColumnName("day_15");
            this.Property(t => t.day_20).HasColumnName("day_20");
            this.Property(t => t.day_25).HasColumnName("day_25");
            this.Property(t => t.day_30).HasColumnName("day_30");
            this.Property(t => t.day_35).HasColumnName("day_35");
            this.Property(t => t.day_60).HasColumnName("day_60");
            this.Property(t => t.is_nifty_50).HasColumnName("is_nifty_50");
            this.Property(t => t.is_nifty_100).HasColumnName("is_nifty_100");
            this.Property(t => t.is_nifty_200).HasColumnName("is_nifty_200");
            this.Property(t => t.day_40).HasColumnName("day_40");
            this.Property(t => t.day_45).HasColumnName("day_45");
            this.Property(t => t.day_50).HasColumnName("day_50");
            this.Property(t => t.day_55).HasColumnName("day_55");
            this.Property(t => t.day_65).HasColumnName("day_65");
            this.Property(t => t.day_70).HasColumnName("day_70");
            this.Property(t => t.day_75).HasColumnName("day_75");
            this.Property(t => t.day_80).HasColumnName("day_80");
            this.Property(t => t.day_85).HasColumnName("day_85");
            this.Property(t => t.day_90).HasColumnName("day_90");
            this.Property(t => t.mf_cnt).HasColumnName("mf_cnt");
            this.Property(t => t.mf_qty).HasColumnName("mf_qty");
            this.Property(t => t.day_2).HasColumnName("day_2");
            this.Property(t => t.day_3).HasColumnName("day_3");
            this.Property(t => t.day_4).HasColumnName("day_4");
            this.Property(t => t.day_1).HasColumnName("day_1");
            this.Property(t => t.high_count).HasColumnName("high_count");
            this.Property(t => t.low_count).HasColumnName("low_count");
            this.Property(t => t.rsi).HasColumnName("rsi");
            this.Property(t => t.prev_rsi).HasColumnName("prev_rsi");
            Ignore(t => t.created_date);
            Ignore(t => t.created_by);
            Ignore(t => t.last_updated_date);
            Ignore(t => t.last_updated_by);
        }
    }
}
