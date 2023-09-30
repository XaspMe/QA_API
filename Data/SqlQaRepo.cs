using QA_API.Dtos;
using QA_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA_API.Data
{
    public class SqlQaRepo : IQaRepo
    {
        private readonly QAContext _context;

        public SqlQaRepo(QAContext context)
        {
            _context = context;
        }

        public void CreateCategory(QACategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Add(category);
        }

        public QAElement GetElementRandom()
        {
            var collection = _context.Elements.ToList();
            var randomElement = collection[new Random().Next(collection.Count)];
            return randomElement;
        }

        public void CreateElement(QAElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _context.Elements.Add(element);
        }

        public void DeleteCategory(QACategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Remove(category);
        }

        public void DeleteElement(QAElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _context.Elements.Remove(element);
        }

        public IEnumerable<QACategory> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public IEnumerable<QAElement> GetAllElements()
        {
            var modelElem = _context.Elements.ToList();
            return modelElem;
        }

        public QACategory GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }

        public QACategory GetCategoryByName(string name)
        {
            return _context.Categories.FirstOrDefault(x => x.Name == name);
        }

        public QAElement GetElementInGroupByQuestion(string question, int group)
        {
            return _context.Elements.FirstOrDefault(x => x.Question == question && x.Category.Id == group);
        }

        public QAElement GetElementById(int id)
        {
            return _context.Elements.FirstOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCategory(QACategory category)
        {
            // Nothing
        }

        public void UpdateElement(QAElement element)
        {
            // Nothing
        }
    }
}
