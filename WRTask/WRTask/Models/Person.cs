using System;
using System.Collections.Generic;

#nullable disable

namespace WRTask.Models
{
    public partial class Person
    {
        public Person()
        {
            SponsorsAuthorizedByPerson = new HashSet<Sponsor>();
            SponsorSCreatedByPerson = new HashSet<Sponsor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Sponsor> SponsorsAuthorizedByPerson { get; set; }
        public virtual ICollection<Sponsor> SponsorSCreatedByPerson { get; set; }
    }
}
