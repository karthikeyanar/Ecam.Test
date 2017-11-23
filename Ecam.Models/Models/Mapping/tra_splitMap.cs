
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_splitMap : EntityTypeConfiguration<tra_split> {
        public tra_splitMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.split_factor)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_split");
            this.Property(t => t.id).HasColumnName("split_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.split_date).HasColumnName("split_date");
            this.Property(t => t.split_factor).HasColumnName("split_factor");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}
