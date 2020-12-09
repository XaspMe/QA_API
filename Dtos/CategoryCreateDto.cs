using System.ComponentModel.DataAnnotations;
using QA_API.Models;

namespace QA_API.Dtos
{
    public class CategoryCreateDto : CreateCommand

    {
    // Specify annotation to view errors on client withot stack trace horror
    [Required] [MaxLength(250)] public string Name { get; set; }
    }
}
