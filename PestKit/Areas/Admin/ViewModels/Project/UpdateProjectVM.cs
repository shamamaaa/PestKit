using System;
using PestKit.Models;

namespace PestKit.Areas.Admin.ViewModels
{
	public class UpdateProjectVM
	{
        public string Name { get; set; }

        public string Description { get; set; }

        public List<int>? ImageIds { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public List<IFormFile>? OtherPhotos { get; set; }

        public List<ProjectImage>? ProjectImages { get; set; }
    }
}

