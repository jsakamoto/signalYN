using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AskTheAudienceNow.Models;
using Newtonsoft.Json.Linq;

namespace AskTheAudienceNow.Controllers
{
    public class DefaultController : Controller
    {
        public AppDbContext Db { get; set; }

        public DefaultController()
        {
            this.Db = new AppDbContext();
        }

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateNewRoom()
        {
            var newRoomNumber = new Random()
                .ToEnumerable(r => r.Next(100, 10000))
                .First(n => this.Db.Rooms.Any(room => room.RoomNumber == n) == false);

            var urlOfThisRoom = Url.AppUrl() + Url.Action("Room", new { id = newRoomNumber });
            var bitly = Bitly.Default;
            var shortUrlOfThisRoom = bitly.Status == Bitly.StatusType.Available ?
                bitly.ShortenUrl(urlOfThisRoom) : "";

            var options = new[] { 
                new Option{ DisplayOrder = 1, Text = "Yes" },
                new Option{ DisplayOrder = 2, Text = "No"},
            }.ToList();

            this.Db.Rooms.Add(new Room
            {
                RoomNumber = newRoomNumber,
                OwnerUserID = this.User.Identity.Name,
                Options = options,
                Url = urlOfThisRoom,
                ShortUrl = shortUrlOfThisRoom
            });
            this.Db.SaveChanges();

            return RedirectToAction("Room", new { id = newRoomNumber });
        }

        public ActionResult Room(int id)
        {
            var room = this.Db.Rooms
                .Include("Options")
                .Single(_ => _.RoomNumber == id);
            return View(room);
        }

        public ActionResult WarmUp()
        {
            var bitly = Bitly.Default;

            var limit = DateTime.UtcNow.AddDays(-7);
            var roomsToSweep = Db.Rooms.Where(room => room.CreatedAt < limit).ToList();
            Db.Rooms.RemoveRange(roomsToSweep);
            Db.SaveChanges();

            return new EmptyResult();
        }
    }
}