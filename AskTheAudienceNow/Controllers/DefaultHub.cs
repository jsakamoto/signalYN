using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace AskTheAudienceNow.Controllers
{
    public class Data
    {
        public string label { get; set; }

        public int value { get; set; }
    }

    public class DefaultHub : Hub
    {
        public void EnterRoom(int roomNumber)
        {
            Groups.Add(Context.ConnectionId, roomNumber.ToString());

            var data = new[] { 
                new Data{ label = "Yes", value = 7},
                new Data{ label = "No", value = 3}
            };
            Clients.Caller.UpdateTotaling(data);
        }

        public void PostAnswer(int roomNumber, string answer)
        {
            var data = answer == "Yes" ?
                new[] { 
                    new Data{ label = "Yes", value = 48},
                    new Data{ label = "No", value = 4}
                }:
                new[] { 
                    new Data{ label = "Yes", value = 48},
                    new Data{ label = "No", value = 14}
                };
            Clients.Group(roomNumber.ToString()).UpdateTotaling(data);
        }

        public void Reset(int roomNumber)
        {
            Clients.Group(roomNumber.ToString()).Reset();
        }
    }
}