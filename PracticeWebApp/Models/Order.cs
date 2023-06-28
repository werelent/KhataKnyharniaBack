using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PracticeWebApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        // Store book IDs and corresponding quantities as a JSON string
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string BookQuantitiesJson
        {
            get => JsonSerializer.Serialize(BookQuantities);
            set => BookQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(value);
        }

        [NotMapped] // Exclude from database schema creation
        public Dictionary<int, int> BookQuantities { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        // Additional properties for storing user details from checkout
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
