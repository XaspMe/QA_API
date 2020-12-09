using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly QAContext _qaContext;

        public QAElementsController(QAContext qaContext)
        {
            _qaContext = qaContext;
        }

        // TODO: Не возвращаются категории при обращении к элементам.
        //GET api/Categories
        [HttpGet]
        public IAsyncEnumerable<ElementReadDto> GetAllElements()
        {
            return _qaContext.Elements
                .AsNoTracking()
                .AsEnumerable()
                .Select(x => x.ToView())
                .ToAsyncEnumerable();
        }

        //GET api/Categories/{id}
        [HttpGet("{id}", Name = "GetElementById")]
        public async Task<ActionResult<ElementReadDto>> GetElementById(Guid id)
        {
            var entry = await _qaContext.Elements.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == id);
            if (entry == null) return NotFound();
            return entry.ToView();
        }


        // POST: api/Create/
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(ElementCreateDto element, CancellationToken cancellationToken)
        {
            if (element == null)
                return BadRequest();
            var category = await _qaContext.Categories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == element.CategoryName, cancellationToken);

            // TODO: проверь категорию, если null - создать новую
            var entity = new QAElement(element);
            entity.Category = category;
            await _qaContext.Elements.AddAsync(entity, cancellationToken);
            await _qaContext.SaveChangesAsync(cancellationToken);

            return Ok(entity.Guid);
        }
    }
}