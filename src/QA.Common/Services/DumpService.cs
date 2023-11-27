using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using QA.Data;
using QA.Models.Models;

namespace QA.Common.Services
{
    public class DumpService
    {
        private readonly IQaRepo _qaRepo;

        public DumpService(IQaRepo qaRepo)
        {
            _qaRepo = qaRepo;
        }

        [DataContract]
        [Serializable]
        public class QaDump
        {
            [DataMember] public int id;
            [DataMember] public string question;
            [DataMember] public string answer;
            [DataMember] public string category;
            [DataMember] public int categoryId;

            public QaDump()
            {

            }


            public QaDump(int id, string question, string answer, string category, int categoryId)
            {
                this.id = id;
                this.question = question;
                this.answer = answer;
                this.category = category;
                this.categoryId = categoryId;
            }
        }

        public async Task Dump()
        {
            var elements = _qaRepo.GetAllElements();
            var categories = _qaRepo.GetAllCategories();
            var dumpQas =
                elements.Select(x => new
                        QaDump
                        {
                            id = x.Id,
                            question = x.Question,
                            answer = x.Answer,
                            category = x.Category.Name,
                            categoryId = x.Category.Id
                        }).ToList();
            var serializeObject = JsonConvert.SerializeObject(dumpQas);

            var qaDumpSavePath = Environment.GetEnvironmentVariable("QA_DUMP_SAVE_PATH", EnvironmentVariableTarget.User);
            if (qaDumpSavePath is "" or null)
                throw new ArgumentException("QA_DUMP_SAVE_PATH environment variable dos not exists on this machine or empty");

            var path = Path.Combine(qaDumpSavePath, "dumps");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var combine = Path.Combine(path, $"qa-dump-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.json");
            await File.WriteAllTextAsync(combine, serializeObject);
        }
    }
}