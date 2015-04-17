using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace QueueBackgroundWorkItemTest
{
  public class NotificationHub : Hub
  {
    public void SendMessage(string message)
    {
      Clients.All.sendMessage(message);
    }
  }
}