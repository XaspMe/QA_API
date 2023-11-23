using System.ComponentModel.DataAnnotations;

namespace QA.Common.Dtos
{
    public class ElementCreateDto
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
