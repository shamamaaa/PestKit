using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PestKit.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
        public List<Employee>? Employees { get; set; }
    }
}
