using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Option
    {
        public int OptionID { get; set; }

        [Index("IX_RoomID")]
        public int RoomID { get; set; }

        public int DisplayOrder { get; set; }

        public string Text { get; set; }
    }
}