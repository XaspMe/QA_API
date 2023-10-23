using QA_API.Dtos;
using QA_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace QA_API.Data
{
    public interface IQaRepo
    {
        bool SaveChanges();
        int ElementsCount();

        IEnumerable<QACategory> GetAllCategories();
        QACategory GetCategoryById(int id);
        QACategory GetCategoryByName(string name);
        void CreateCategory(QACategory category);
        void UpdateCategory(QACategory category);
        void DeleteCategory(QACategory category);

        IEnumerable<QAElement> GetAllElements();
        QAElement GetElementById(int id);
        QAElement GetElementRandom();
        public QAElement GetElementRandomInCategory(int category);
        void CreateElement(QAElement element);
        void UpdateElement(QAElement element);
        void DeleteElement(QAElement element);
        QAElement GetElementInGroupByQuestion(string question, int group);
    }
}