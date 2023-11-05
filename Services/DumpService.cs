using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using QA_API.Data;

namespace QA_API.Services
{
    public class DumpService
    {
        private readonly IQaRepo _repo;
        private readonly IMapper _mapper;

        public DumpService(IQaRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [DataContract]
        public class QaDump
        {
            [DataMember] public int id;
            [DataMember] public string question;
            [DataMember] public string answer;
            [DataMember] public string category;

            public QaDump(int id, string question, string answer, string category)
            {
                this.id = id;
                this.question = question;
                this.answer = answer;
                this.category = category;
            }
        }

        public class QaObject
        {
            public int len { get; set; }
            public DateTime created { get; set; }
            public IEnumerable<QaDump> qas { get; set; }
        }

        public void Dump()
        {
            var elements = _repo.GetAllElements();
            var categories = _repo.GetAllCategories();
            var dumpQas = elements.Select(x => new
                { x.Id, x.Question, x.Answer, CategoryName = categories.First(y => y.Id == x.Category.Id).Name }).ToList();
            var dir = @$"С:\qa_db\qa_db_dump\";
            var serializeObject = JsonConvert.SerializeObject(dumpQas);
            System.IO.File.WriteAllText(dir + $"qa-dump-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.json", serializeObject);
        }

        public void ReadDump()
        {

        }
    }
}