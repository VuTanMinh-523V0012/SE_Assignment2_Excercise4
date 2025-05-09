using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("Agent")]
    public class Agent
    {
        [Key]
        public int AgentID { get; set; }

        [Required]
        [StringLength(100)]
        public string AgentName { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(20)]
        public string ContactNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; }
    }
}