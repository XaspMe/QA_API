using System;
using System.ComponentModel.DataAnnotations;

namespace QA_API.Models
{
    public abstract class AggregateRoot<TCreateCommand, TView> where TCreateCommand: CreateCommand where TView : class, new()
    {
        private AggregateRoot()
        {
            Guid = Guid.NewGuid();
        }

        protected AggregateRoot(TCreateCommand command) : this()
        {

        }

        [Key]
        public Guid Guid { get; protected set; }

        public abstract TView ToView();
    }
}