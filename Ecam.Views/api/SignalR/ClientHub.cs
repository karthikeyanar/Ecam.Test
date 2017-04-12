using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace Ecam.Views.SignalR {
    public class ClientHub:Hub {
        public void Send(string name,string message) {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name,message);
        }
    }
}