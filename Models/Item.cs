using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("Item")]
    public class Item
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }

        [StringLength(50)]
        public string Size { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}