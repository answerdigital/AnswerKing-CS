#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerKing.Core.Entities
{
    public class ItemEntity : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public List<CategoryEntity> Categories { get; set; }
    }
}
