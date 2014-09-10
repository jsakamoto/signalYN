using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Room
    {
        public int RoomID { get; set; }

        public int RoomNumber { get; set; }

        public string OwnerUserID { get; set; }

        public virtual ICollection<Option> Options { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}