using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace QA.Models.Models
{
    [DataContract]
    [ComplexType]
    public class QACategory
    {
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [DataMember]
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        public User? Author { get; set; }
    }
}