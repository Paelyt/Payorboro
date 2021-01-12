using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GloballendingViews.Models
{
    public class AgentModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string NextOfKinAddress { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
    }
}