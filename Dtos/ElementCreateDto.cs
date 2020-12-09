using System.ComponentModel.DataAnnotations;
using QA_API.Models;

namespace QA_API.Dtos
{
    public class ElementCreateDto : CreateCommand
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Question { get; set; }
        [Required]
        [MaxLength(250)]
        public string Answer { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string CategoryName { get; set; }
    }
}
