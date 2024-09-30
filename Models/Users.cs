using System.ComponentModel.DataAnnotations;  // For data annotations

namespace ShoppingApp.Models
{
    public class Users
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
        public int Age { get; set; }

        public string ProfilePictureUrl { get; set; }
        public bool IsAdmin { get; set; }

        [Required]
        public string Role { get; set; } // Either "Seller" or "Buyer"

        public ICollection<Order> Orders { get; set; }
    }
}
