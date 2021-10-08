using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SauktiniaiUI.Models
{
    [DataContract]
    public class EnlistedPerson
    {
        [DataMember(Name = "pos")]
        public int Position { get; set; }

        [DataMember(Name = "number")]
        public string Number { get; set; }

        [DataMember(Name = "name")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastname")]
        public string LastName { get; set; }

        [DataMember(Name = "bdate")]
        public int BirthYear { get; set; }

        [DataMember(Name = "department")]
        public string Department { get; set; }

        [DataMember(Name = "info")]
        public string Info { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get => DateTime.Now.Date; }

        [DataMember(Name = "region")]
        public string Region { get => RegionNumber.ToString(); }

        [DataMember(Name = "regionNo")]
        public City RegionNumber { get; set; }
    }
}
