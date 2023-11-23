using System.ComponentModel.DataAnnotations;

namespace QA.Common.Dtos
{
    public class CategoryCreateDto
    {
        // Specify annotation to view errors on client withot stack trace horror
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
    }
}
