using System.ComponentModel.DataAnnotations;

namespace PracticeWebApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public List<int> BookIds { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; } 

    }
}
