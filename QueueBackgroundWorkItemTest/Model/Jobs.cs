using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using OfficeOpenXml;
using System.Web.Hosting;

namespace QueueBackgroundWorkItemTest.Model
{
  public class Jobs
  {
    public string Constring { get; set; }
    public Jobs()
    {
      this.Constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    }



    public void SaveFile()
    {
      // 指示をセットする（終了を待つ）
      Debug.WriteLine("指示データ作成開始");
      var index = SetInstruction();

      // ファイル作成処理（バックグラウンドタスクに積んですぐ戻す）
      Debug.WriteLine("バックグラウンドにタスク投入");
      HostingEnvironment.QueueBackgroundWorkItem(token => CreateFile(index, token));
    }

    private int SetInstruction()
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(this.Constring))
      {
        cn.Open();
        using (var tr = cn.BeginTransaction())
        {
          // データを積んでると思って。
          cn.Execute("INSERT INTO HogeHoge (Name) VALUES (N'hogehoge')", transaction: tr);
          tr.Commit();

          Debug.WriteLine("指示データ作成完了");
          return 1; // 指示データのインデックスを返しているつもり
        }
      }
    }



    private async Task CreateFile(int index, CancellationToken cancellationToken)
    {

      await Task.Delay(10000, cancellationToken);

      Debug.WriteLine("バックグランドタスク実行開始");

      using (var cn = new System.Data.SqlClient.SqlConnection(this.Constring))
      {
        try
        {
          cn.Open();
          using (var tr = cn.BeginTransaction())
          {
            // データ取得
            var data = await getData(index, cn, tr);

            // ファイル作成
            var fileBin = CreateExcelFileBin(data);

            // ファイル保存
            await SaveFile(fileBin, cn, tr);

            tr.Commit();
          }
        }
        catch (Exception e)
        {
          Debug.WriteLine("例外発生：" + e.Message);
        }

      }

      Debug.WriteLine("バックグランドタスク実行完了");

    }

    private async Task SaveFile(byte[] fileBin, System.Data.SqlClient.SqlConnection cn, System.Data.SqlClient.SqlTransaction tr)
    {

      await cn.ExecuteAsync("INSERT INTO HogeFiles (Name, Bin) VALUES (@Name, @Bin)",
                            param: new { Name = "hoge.xlsx", Bin = fileBin },
                            transaction: tr);
    }

    private async Task<object> getData(int index, System.Data.IDbConnection cn, System.Data.IDbTransaction tr)
    {
      var result = await cn.QueryAsync("SELECT 'hoge' AS Name", transaction: tr);
      return result.FirstOrDefault().Name;
    }


    private byte[] CreateExcelFileBin(object data)
    {
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("SheetHoge1");

        worksheet.Cells["A1"].Value = "a";

        return package.GetAsByteArray();
      }
    }



  }
}