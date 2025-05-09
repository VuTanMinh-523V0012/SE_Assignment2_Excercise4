using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderDate { get; set; }

        [Required]
        public int AgentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("AgentID")]
        public virtual Agent Agent { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}