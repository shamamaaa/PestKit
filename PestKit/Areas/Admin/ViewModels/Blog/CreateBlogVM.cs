using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PestKit.Areas.Admin.ViewModels
{
	public class CreateBlogVM
	{
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
        public int CommentCount { get; set; }
        [Required]
        public int? AuthorId { get; set; }
        public IFormFile Photo { get; set; }

        public List<int> TagIds { get; set; }
        public SelectList? Tags { get; set; }
        public SelectList? Authors { get; set; }

    }
}

