using System;
namespace PestKit.Areas.Admin.ViewModels
{
	public class CreateEmployeeVM
	{
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public IFormFile Photo { get; set; }
        public string? IgAcc { get; set; }
        public string? FbAcc { get; set; }
        public string? TwAcc { get; set; }
        public string? LinAcc { get; set; }
    }
}

