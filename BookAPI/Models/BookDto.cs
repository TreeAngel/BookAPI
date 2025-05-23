﻿using BookAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookAPI.Models
{
    public class BookDto
    {
        [Key]
        public int Id { get; set; }

        [Key]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public long Price { get; set; }

        public string Publisher { get; set; } = null!;

        public DateOnly PublishDate { get; set; }

        public string ImageCover { get; set; } = null!;

        public List<string> Genres { get; set; } = [];

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }
    }

    public class CreateBookDto
    {
        [Key, Required, MinLength(1)]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Required, Range(1, long.MaxValue)]
        public long Price { get; set; }

        [Required, MinLength(1)]
        public string Publisher { get; set; } = null!;

        [Required]
        public DateOnly PublishDate { get; set; }

        public IFormFile ImageCover { get; set; } = null!;
    }
}
