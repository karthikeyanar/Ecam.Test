
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_mutual_fund_pfMap : EntityTypeConfiguration<tra_mutual_fund_pf> {
        public tra_mutual_fund_pfMap() {
		            // Primary Key
            this.HasKey(t => new { t.fund_id, t.symbol });

            // Properties
            this.Property(t => t.fund_id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.quantity)
                .HasPrecision(13,4);

            this.Property(t => t.stock_value)
                .HasPrecision(13,4);

            this.Property(t => t.stock_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.stock_name)
                .HasMaxLength(500);

            // Table & Column Mappings
			this.ToTable("tra_mutual_fund_pf");
            this.Property(t => t.fund_id).HasColumnName("fund_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.quantity).HasColumnName("quantity");
            this.Property(t => t.stock_value).HasColumnName("stock_value");
            this.Property(t => t.stock_percentage).HasColumnName("stock_percentage");
            this.Property(t => t.stock_name).HasColumnName("stock_name");
												        }
    }
}
