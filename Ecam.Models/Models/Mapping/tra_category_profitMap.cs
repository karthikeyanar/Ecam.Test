
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_category_profitMap : EntityTypeConfiguration<tra_category_profit> {
        public tra_category_profitMap() {
		            // Primary Key
            this.HasKey(t => new { t.category_name, t.profit_type, t.profit_date });

            // Properties
            this.Property(t => t.category_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.profit_type)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.profit)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_category_profit");
            this.Property(t => t.category_name).HasColumnName("category_name");
            this.Property(t => t.profit_type).HasColumnName("profit_type");
            this.Property(t => t.profit_date).HasColumnName("profit_date");
            this.Property(t => t.profit).HasColumnName("profit");
												        }
    }
}
