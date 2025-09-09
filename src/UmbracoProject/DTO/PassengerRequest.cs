using System.ComponentModel.DataAnnotations;
using UmbracoProject.Models;

namespace UmbracoProject.DTO
{
    public class PassengerRequest
    {
        [Required(ErrorMessage = "First Name is required")] 
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Last Name is required")] 
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Birthdate is required")] 
        public DateOnly BirthDate { get; set; }
       
        [Required(ErrorMessage = "Gender is required")] 
        public Gender Gender { get; set; }
    }
}
