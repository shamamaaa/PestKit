using System;
namespace PestKit.Areas.Admin.ViewModels
{
	public class CreateProjectVM
	{
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile MainPhoto { get; set; }

        public List<IFormFile> OtherPhotos { get; set; }

    }
}

