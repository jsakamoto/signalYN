using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AskTheAudienceNow.Models
{
    public class Room
    {
        public int RoomID { get; set; }

        [Index("IX_RoomNumber", IsUnique = true)]
        public int RoomNumber { get; set; }

        public string Url { get; set; }

        public string ShortUrl { get; set; }

        public string OwnerUserID { get; set; }

        public DateTime CreatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Option> Options { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }

        public Room()
        {
            this.CreatedAt = DateTime.UtcNow;
        }
    }
}