using System.Collections.Generic;

namespace lesson2.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<TaskItem> Tasks { get; set; } = new();
    }
}
