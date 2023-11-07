using System.ComponentModel.DataAnnotations;

namespace QA.Models.Models
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