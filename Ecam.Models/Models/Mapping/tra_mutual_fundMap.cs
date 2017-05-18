
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_mutual_fundMap : EntityTypeConfiguration<tra_mutual_fund> {
        public tra_mutual_fundMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.fund_name)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
			this.ToTable("tra_mutual_fund");
            this.Property(t => t.id).HasColumnName("mutual_fund_id");
            this.Property(t => t.fund_name).HasColumnName("fund_name");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}
