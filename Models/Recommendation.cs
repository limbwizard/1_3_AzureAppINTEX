﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAppINTEX.Models
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Customer")]
        public string? CustomerId { get; set; }
        public int? Rec1 { get; set; }
        public int? Rec2 { get; set; }
        public int? Rec3 { get; set; }
        public int? Rec4 { get; set; }
        public int? Rec5 { get; set; }
        public int? Rec6 { get; set; }
        public int? Rec7 { get; set; }
        public int? Rec8 { get; set; }
        public int? Rec9 { get; set; }
        public int? Rec10 { get; set; }
        public Product? Product { get; set; }
        public Customer? Customer { get; set; }

    }
}
