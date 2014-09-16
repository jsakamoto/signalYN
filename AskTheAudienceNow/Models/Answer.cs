using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }

        public int RoomID { get; set; }

        public string AnsweredUserID { get; set; }

        public string ChosedOptionText { get; set; }
    }
}