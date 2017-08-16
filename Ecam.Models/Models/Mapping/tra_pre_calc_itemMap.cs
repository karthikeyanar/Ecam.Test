
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_pre_calc_itemMap : EntityTypeConfiguration<tra_pre_calc_item> {
        public tra_pre_calc_itemMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            // Table & Column Mappings
			this.ToTable("tra_pre_calc_item");
            this.Property(t => t.id).HasColumnName("prev_calc_item_id");
            this.Property(t => t.percentage).HasColumnName("percentage");
            this.Property(t => t.success_count).HasColumnName("success_count");
            this.Property(t => t.fail_count).HasColumnName("fail_count");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}
