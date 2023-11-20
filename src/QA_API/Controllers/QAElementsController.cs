using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QA.Common.Dtos;
using QA.Common.Services;
using QA.Data;
using QA.Models.Models;


namespace QA_API.Controllers
{
    [Route("api/Elements")]
    [ApiController]
    public class QAElementsController : Controller
    {
        private readonly IQaRepo _repo;
        private readonly IMapper _mapper;
        private readonly DumpService _dumpService;

        public QAElementsController(IQaRepo repo, IMapper mapper, DumpService dumpService)
        {
            _repo = repo;
            _mapper = mapper;
            _dumpService = dumpService;
        }

        // TODO: Не возвращаются категории при обращении к элементам.
        //GET api/Categories
         [HttpGet]
         public ActionResult<IEnumerable<CategoryReadDto>> GetAllElements()
         {
             var result = _repo.GetAllElements();
             var mappedResult = _mapper.Map<IEnumerable<ElementReadDto>>(result);
             return Ok(mappedResult);
         }

        //GET api/Categories/{id}
        [HttpGet("{id}", Name = "GetElementById")]
        public ActionResult<ElementReadDto> GetElementById(int id)
        {
            var result = _repo.GetElementById(id);

            if (result != null)
            {
                var mappedResult = _mapper.Map<ElementReadDto>(result);
                return Ok(mappedResult);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("/api/Elements/Random")]
        public ActionResult<ElementReadDto> GetElementRandom()
        {
            var result = _repo.GetElementRandom();

            if (result != null)
            {
                var mappedResult = _mapper.Map<ElementReadDto>(result);
                return Ok(mappedResult);
            }
            return NotFound();
        }


        // POST: api/Create/
        [HttpPost]
        public ActionResult<ElementReadDto> Create(ElementCreateDto element)
        {
            if (element != null)
            {
                var elementCreateDto = _mapper.Map<ElementCreateDto>(element);

                var elementModel = _mapper.Map<QAElement>(element);
                elementModel.Category = _repo.GetCategoryByName(element.CategoryName);

                if (elementModel.Category != null)
                {
                    _repo.CreateElement(elementModel);
                    _repo.SaveChanges();
                    return CreatedAtRoute(nameof(GetElementById), new { Id = elementModel.Id }, elementModel);
                }

                return ValidationProblem($"Категория: {element.CategoryName} не существует, объект не был создан.");
            }

            return ValidationProblem(nameof(element));
        }
    }
}