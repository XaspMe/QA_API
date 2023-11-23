using AutoMapper;
using QA.Common.Dtos;
using QA.Models.Models;

namespace QA.Common.Profiles
{
    public class QaProfile : Profile
    {
        public QaProfile()
        {
            // Source -> target
            CreateMap<QACategory, CategoryReadDto>();
            CreateMap<CategoryReadDto, QACategory>();

            CreateMap<CategoryCreateDto, QACategory>();
            CreateMap<QACategory, CategoryCreateDto>();
            CreateMap<QACategory, ElementCreateDto>();

            CreateMap<ElementReadDto, QAElement>();
            CreateMap<QAElement, ElementReadDto>();

            CreateMap<ElementCreateDto, QAElement>();
            CreateMap<QAElement, ElementCreateDto>();

            CreateMap<ElementCreateDto, ElementReadDto>();
            CreateMap<ElementReadDto, ElementCreateDto>();

            CreateMap<ElementCreateDto, CategoryCreateDto>();
            CreateMap<CategoryCreateDto, ElementCreateDto>();
        }
    }
}
