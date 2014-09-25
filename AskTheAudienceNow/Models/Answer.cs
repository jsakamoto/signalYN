using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }

        [Index("IX_RoomID")]
        public int RoomID { get; set; }

        public string AnsweredUserID { get; set; }

        public string ChosedOptionText { get; set; }
    }
}