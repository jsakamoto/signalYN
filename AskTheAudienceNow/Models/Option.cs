using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Option
    {
        public int OptionID { get; set; }

        public int RoomID { get; set; }

        public int DisplayOrder { get; set; }

        public string Text { get; set; }
    }
}