using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GloballendingViews.Models
{
    public class LoanSocialModel
    {
        public int ID { get; set; }
        public int Loan_Fk { get; set; }
        public SocialName Social { get; set; }

    }
    public enum SocialName
    {
        FaceBook = 1,
        Twitter = 2,
        Instagram = 3
    }
}