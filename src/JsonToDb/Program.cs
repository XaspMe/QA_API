using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QA.Common.Services;
using QA.Data;
using QA.Models.Models;


var services = new ServiceCollection();

var dBConnectionString = Environment.GetEnvironmentVariable("QA_DB", EnvironmentVariableTarget.User);
if (dBConnectionString is "" or null)
    throw new ArgumentException("QA_DB environment variable does not exists on this machine or empty");

services.AddDbContext<QaContext>(opt =>
    opt.UseSqlServer(dBConnectionString));
services.AddScoped<IQaRepo, SqlQaRepo>();

var serviceProvider = services.BuildServiceProvider();
var _appDbContext = serviceProvider.GetService<IQaRepo>();

var result = File.ReadAllText("C:\\temp\\dumps\\qa-dump-2023-11-27T05-06-36.json"); // here your path to file
var deserialised = JsonConvert.DeserializeObject<ICollection<DumpService.QaDump>>(result);
// add categories
// foreach (var category in deserialised.ToList().GroupBy(x => x.categoryId).Select(x => x.First()))
// {
//     var qaCategory = new QACategory()
//     {
//         Id = category.categoryId,
//         Name = category.category
//     };
//     _appDbContext.CreateCategory(qaCategory);
//     _appDbContext.SaveChanges();
// }

var catsResult = _appDbContext.GetAllCategories();
Console.WriteLine("text");
// add questions
foreach (var question in deserialised.ToList())
{
    {
        QAElement element = new QAElement()
        {
            Category = new QACategory() { Name = question.category },
            Answer = question.answer,
            Question = question.question
        };

        await _appDbContext.CreateElementWithCategoryLoading(element);
    }
}

var addingResult = _appDbContext.CategoriesStats();
Console.WriteLine(addingResult);
// await _appDbContext.CreateElementWithCategoryLoading()