using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PestKit.Areas.Admin.ViewModels
{
	public class UpdateBlogVM
	{
        public string Name { get; set; }
        public string Description { get; set; }
        public int CommentCount { get; set; }
        [Required]
        public int? AuthorId { get; set; }
        public IFormFile? Photo { get; set; }
        public string ImageUrl { get; set; }

        public List<int> TagIds { get; set; }
        public SelectList? Tags { get; set; }
        public SelectList? Authors { get; set; }
    }
}

