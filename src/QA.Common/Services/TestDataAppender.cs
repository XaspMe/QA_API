using QA.Data;
using QA.Models.Models;

namespace QA.Common.Services;

public class TestDataAppender
{
    public async Task Append(IQaRepo _repo)
    {
        var categories = await File.ReadAllLinesAsync(Path.Combine("testData", "categories.csv"));
        foreach (var str in categories)
        {
            var res = str.Split(@";");
            var model = new QACategory()
            {
                Id = int.Parse(res[0]),
                Name = res[1]
            };
            _repo.CreateCategory(model);
        }

        _repo.SaveChanges();

        var elements = await File.ReadAllLinesAsync(Path.Combine("testData", "elements.csv"));
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
    }
}