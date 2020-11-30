using System;
using System.Collections.Generic;
using System.Text;

namespace DMicroservices.DataAccess.Tests.Models
{
    public class ClassModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ICollection<StudentModel> Students { get; set; }
    }
}
