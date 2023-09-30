using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QA_API.Models
{
    public class QAElement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public QACategory Category { get; set; } 
    }
}