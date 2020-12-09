using System;

namespace QA_API.Dtos
{
    public class ElementReadDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
