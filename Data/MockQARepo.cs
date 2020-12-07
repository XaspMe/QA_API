using QA_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA_API.Data
{
    public class MockQARepo //: IQaRepo
    {
        public void CreateCategory(QACategory category)
        {
            throw new NotImplementedException();
        }

        public void GetElementInGroupByQuestion(string question, int group)
        {
            throw new NotImplementedException();
        }

        public void CreateElement(QAElement element)
        {
            throw new NotImplementedException();
        }

        public void DeleteCategory(QACategory category)
        {
            throw new NotImplementedException();
        }

        public void DeleteElement(QAElement element)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QACategory> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QAElement> GetAllElements()
        {
            throw new NotImplementedException();
        }

        public QACategory GetCategoryById(int id)
        {
            throw new NotImplementedException();
        }

        public QACategory GetCategoryByName(string name)
        {
            throw new NotImplementedException();
        }

        public QAElement GetElementByQuestion(string question)
        {
            throw new NotImplementedException();
        }

        public QAElement GetElementById(int id)
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void UpdateCategory(QACategory category)
        {
            throw new NotImplementedException();
        }

        public void UpdateElement(QAElement element)
        {
            throw new NotImplementedException();
        }
    }
}
