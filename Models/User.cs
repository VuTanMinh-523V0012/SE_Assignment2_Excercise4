using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Column("IsLocked")]
        [Display(Name = "Locked")]
        public bool IsLocked { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "User";

        [DataType(DataType.DateTime)]
        public DateTime? LastLogin { get; set; }
    }
}

