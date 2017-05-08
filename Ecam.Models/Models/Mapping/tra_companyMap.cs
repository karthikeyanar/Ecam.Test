
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

            this.Property(t => t.close_price)
                .HasPrecision(13, 4);

            this.Property(t => t.prev_price)
                .HasPrecision(13, 4);

            this.Property(t => t.week_52_high)
                .HasPrecision(13, 4);

            this.Property(t => t.ltp_price)
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

            // Table & Column Mappings
            this.ToTable("tra_company");
            this.Property(t => t.id).HasColumnName("company_id");
            this.Property(t => t.company_name).HasColumnName("company_name");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.open_price).HasColumnName("open_price");
            this.Property(t => t.high_price).HasColumnName("high_price");
            this.Property(t => t.low_price).HasColumnName("low_price");
            this.Property(t => t.close_price).HasColumnName("close_price");
            this.Property(t => t.prev_price).HasColumnName("prev_price");
            this.Property(t => t.week_52_high).HasColumnName("week_52_high");
            this.Property(t => t.ltp_price).HasColumnName("ltp_price");
            this.Property(t => t.is_book_mark).HasColumnName("is_book_mark");
            this.Property(t => t.week_52_low).HasColumnName("week_52_low");
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
            Ignore(t => t.created_date);
            Ignore(t => t.created_by);
            Ignore(t => t.last_updated_date);
            Ignore(t => t.last_updated_by);
        }
    }
}

