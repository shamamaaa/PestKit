using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PestKit.Models
{
	public class Blog
	{
        public int Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CommentCount { get; set; }
        ///
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        ///
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }

        public List<BlogTag>? BlogTags { get; set; }
    }
}

