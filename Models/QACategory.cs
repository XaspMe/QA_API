using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QA_API.Dtos;

namespace QA_API.Models
{
    [ComplexType]
    public class QACategory : AggregateRoot<CategoryCreateDto, CategoryReadDto>
    {
        public QACategory(CategoryCreateDto command) : base(command)
        {
            Name = command.Name;
        }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public override CategoryReadDto ToView()
        {
            return new CategoryReadDto
            {
                Id = Guid,
                Name = Name
            };
        }
    }
}
