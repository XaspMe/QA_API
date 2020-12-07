using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QA_API.Dtos
{
    public class CategoryCreateDto
    {
        // Specify annotation to view errors on client withot stack trace horror
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
    }
}
