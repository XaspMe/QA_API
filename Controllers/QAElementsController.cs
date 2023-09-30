using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QA_API.Data;
using QA_API.Dtos;
using QA_API.Models;


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
            var categories = System.IO.File.ReadLines(@"G:\qa_db\категории.csv");
            foreach (var str in categories)
            {
                var _res = str.Split(@";");
                var model = new QACategory()
                {
                    Id = int.Parse(_res[0]),
                    Name = _res[1]
                };
                _repo.CreateCategory(model);
            }

            _repo.SaveChanges();

            var elements = System.IO.File.ReadLines(@"G:\qa_db\элементы.csv");
            foreach (var str in elements)
            {
                var _res = str.Split(@";");
                var model = new QAElement
                {
                    Question = _res[1],
                    Answer = _res[2],
                    Category = _repo.GetCategoryById(int.Parse(_res[3]))
                };
                _repo.CreateElement(model);
            }

            _repo.SaveChanges();

            if (element != null)
            {
                var elementCreateDto = _mapper.Map<ElementCreateDto>(element);

                var elementModel = _mapper.Map<QAElement>(element);
                elementModel.Category = _repo.GetCategoryByName(element.CategoryName);

                if (elementModel.Category != null)
                {
                    _repo.CreateElement(elementModel);
                    _repo.SaveChanges();
                    _dumpService.Dump();
                    return CreatedAtRoute(nameof(GetElementById), new { Id = elementModel.Id }, elementModel);
                }

                return ValidationProblem($"Категория: {element.CategoryName} не существует, объект не был создан.");
            }

            return ValidationProblem(nameof(element));
        }
    }
}