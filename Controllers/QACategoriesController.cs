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
    [Route("api/Categories")]
    [ApiController]
    public class QACategoriesController : Controller
    {
        private readonly QAContext _qaContext;

        public QACategoriesController(QAContext qaContext)
        {
            _qaContext = qaContext;
        }

        //GET api/Categories
        [HttpGet]
        public IAsyncEnumerable<CategoryReadDto> GetAllCategories()
        {
            return _qaContext.Categories
                .AsNoTracking()
                .AsEnumerable()
                .Select(x => x.ToView())
                .ToAsyncEnumerable();
        }

        //GET api/Categories/{id}
        [HttpGet("{id}", Name="GetCategoriesById")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoriesById(Guid id)
        {
            var entry = await _qaContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == id);
            if (entry == null) return NotFound();
            return entry.ToView();
        }

        // POST: api/Create/
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CategoryCreateDto category, CancellationToken cancellationToken)
        {
            if (category == null)
                return BadRequest();
            if (await _qaContext.Categories.AsNoTracking().AnyAsync(x => x.Name == category.Name, cancellationToken))
                return BadRequest($"Category already exist {category.Name}");

            var entity = new QACategory(category);
            await _qaContext.Categories.AddAsync(entity, cancellationToken);
            await _qaContext.SaveChangesAsync(cancellationToken);

            return Ok(entity.Guid);
        }
    }
}
