using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using AskTheAudienceNow.Models;
using Microsoft.AspNet.SignalR;

namespace AskTheAudienceNow.Controllers
{
    public class Data
    {
        public string label { get; set; }

        public int value { get; set; }
    }

    [Authorize]
    public class DefaultHub : Hub
    {
        public object EnterRoom(int roomNumber)
        {
            Groups.Add(Context.ConnectionId, roomNumber.ToString());

            var db = new AppDbContext();
            var room = FindRoom(roomNumber, db);
            UpdateTotaling(room);

            var userID = Context.User.Identity.Name;
            var myAnswer = room.Answers
                .FirstOrDefault(a => a.AnsweredUserID == userID) ?? new Answer();
            var options = room.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new
                {
                    text = o.Text,
                    selected = o.Text == myAnswer.ChosedOptionText
                })
                .ToArray();
            
            return options;
        }

        public void PostAnswer(int roomNumber, string answer)
        {
            var db = new AppDbContext();
            var room = FindRoom(roomNumber, db);

            var userID = Context.User.Identity.Name;
            ClearMyAnswers(db, room, userID);

            room.Answers.Add(new Answer
            {
                AnsweredUserID = userID,
                ChosedOptionText = answer
            });
            db.SaveChanges();

            UpdateTotaling(room);
        }

        public void RevokeAnswer(int roomNumber, string answer)
        {
            var db = new AppDbContext();
            var room = FindRoom(roomNumber, db);

            var userID = Context.User.Identity.Name;
            ClearMyAnswers(db, room, userID);
            db.SaveChanges();

            UpdateTotaling(room);
        }

        public void Reset(int roomNumber)
        {
            var db = new AppDbContext();
            var room = FindRoom(roomNumber, db);
            room.Answers.ToList().ForEach(a =>
            {
                db.Answers.Remove(a);
            });
            db.SaveChanges();

            Clients.Group(roomNumber.ToString()).Reset();
        }

        private static Room FindRoom(int roomNumber, AppDbContext db)
        {
            var room = db.Rooms
                .Include("Answers")
                .Include("Options")
                .First(r => r.RoomNumber == roomNumber);
            return room;
        }

        private static void ClearMyAnswers(AppDbContext db, Room room, string userID)
        {
            room.Answers
                .Where(a => a.AnsweredUserID == userID)
                .ToList()
                .ForEach(a =>
                {
                    room.Answers.Remove(a);
                    db.Answers.Remove(a);
                });
        }

        private void UpdateTotaling(Room room)
        {
            var data = room.Options
                .Select(o => new Data
                {
                    label = o.Text,
                    value = room.Answers.Count(a => a.ChosedOptionText == o.Text)
                })
                .ToArray();
            if (data.Any(d => d.value > 0))
            {
                Clients.Group(room.RoomNumber.ToString()).UpdateTotaling(data);
            }
            else
            {
                Clients.Group(room.RoomNumber.ToString()).Reset();
            }
        }
    }
}