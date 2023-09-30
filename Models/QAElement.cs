using System.ComponentModel.DataAnnotations;
using QA_API.Dtos;

namespace QA_API.Models
{
    public class QAElement : AggregateRoot<ElementCreateDto, ElementReadDto>
    {
        [Required]
        [MaxLength(250)]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public QACategory Category { get; set; }

        public QAElement(ElementCreateDto command) : base(command)
        {
            Question = command.Question;
            Answer = command.Answer;
        }

        public override ElementReadDto ToView()
        {
            return new ElementReadDto
            {
                Id = Guid,
                Question = Question,
                Answer = Answer
            };
        }
    }
}