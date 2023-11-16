using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QA.Common.Dtos;
using QA.Data;
using QA.Models.Models;

namespace QA_API.Controllers
{
    [Route("api/Categories")]
    [ApiController]
    public class QACategoriesController : Controller
    {
        private readonly IQaRepo _repo;
        private readonly IMapper _mapper;

        public QACategoriesController(IQaRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        //GET api/Categories
        [HttpGet]
        public ActionResult<IEnumerable<CategoryReadDto>> GetAllCategories()
        {
            var result = _repo.GetAllCategories();
            var mappedResult = _mapper.Map<IEnumerable<CategoryReadDto>>(result);
            return Ok(mappedResult);
        }

        //GET api/Categories/{id}
        [HttpGet("{id}", Name = "GetCategoriesById")]
        public ActionResult<CategoryReadDto> GetCategoriesById(int id)
        {
            var result = _repo.GetCategoryById(id);

            if (result != null)
            {
                var mappedResult = _mapper.Map<CategoryReadDto>(result);
                return Ok(mappedResult);
            }

            return NotFound();
        }

        // POST: api/Create/
        [HttpPost]
        public ActionResult<CategoryReadDto> Create(CategoryCreateDto category)
        {
            if (category != null)
            {
                var categoryModel = _mapper.Map<QACategory>(category);

                if (_repo.GetCategoryByName(category.Name) != null)
                {
                    return ValidationProblem($"Category already exist {category.Name}");
                }

                _repo.CreateCategory(categoryModel);

                _repo.SaveChanges();
                var readDto = _mapper.Map<CategoryReadDto>(categoryModel);
                // TODO: Return id value does not workin
                return CreatedAtRoute(nameof(GetCategoriesById), new { Id = readDto.Id }, readDto);
            }

            return ValidationProblem(nameof(category));
        }
    }
}