using System;
using System.Text.Json.Serialization;

namespace catalogapi.Models
{
	public class Product	{
        
            public int ProductId { get; set; }

            public string? Name { get; set; }

            public string? Description { get; set; }

            public decimal Price { get; set; }

            public DateTime DatePurchase { get; set; }

            public int stock { get; set; }

            public int CategoryId { get; set; }

           [JsonIgnore]
            public Category? Category { get; set; }
        }
        
}

