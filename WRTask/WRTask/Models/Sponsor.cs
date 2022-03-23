using System;
using System.Collections.Generic;

#nullable disable

namespace WRTask.Models
{
    public partial class Sponsor
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public bool Authorized { get; set; }
        public int? CreatedByPersonId { get; set; }
        public int? AuthorizedByPersonId { get; set; }

        public virtual Person AuthorizedByPerson { get; set; }
        public virtual Person CreatedByPerson { get; set; }
    }
}
