using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PestKit.Models
{
	public class Employee
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        //
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        //
        public Position Position { get; set; }
        public int PositionId { get; set; }
        //
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }

        //
        public string? IgAcc { get; set; }
        public string? FbAcc { get; set; }
        public string? TwAcc { get; set; }
        public string? LinAcc { get; set; }

    }
}

