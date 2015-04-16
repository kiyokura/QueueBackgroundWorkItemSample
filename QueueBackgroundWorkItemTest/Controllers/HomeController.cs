using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace QueueBackgroundWorkItemTest.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {

      // 実行時コンテキストが取れないので必要なものは全部渡しておいてやる必要がある点は注意。
      var jobs = new Model.Jobs();
      jobs.SaveFile();

      ViewBag.Message = "Your contact page.";

      Debug.WriteLine("クライアントにレスポンス返した");
      return View();
    }

  }
}