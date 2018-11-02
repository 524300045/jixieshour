using StriveEngine.BinaryDemoServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace StriveEngine.BinaryDemoServer.Dal
{
  public    class ClientInfoDal
    {
      public int Insert(ClientIpInfoModel model)
      {
          string sql = "INSERT INTO client_ipinfo (NAME,CODE,ip,PORT,timeouts,STATUS)  VALUES ('{0}','{1}','{2}','{3}',{4},1)  ";
          sql = string.Format(sql, model.Name, model.Code, model.IP, model.Port, model.Timeouts);

          int o=MySqlHelper.ExecuteNonQuery(sql);
          return o;
      }

      public int Update(ClientIpInfoModel model)
      {
          string sql = @"UPDATE client_ipinfo 
set
NAME='{0}',
CODE='{1}',
ip='{2}',
PORT='{3}',
timeouts={4},
STATUS={5}
WHERE id="+model.Id;
          sql = string.Format(sql, model.Name, model.Code, model.IP, model.Port, model.Timeouts,model.Status);

          int o = MySqlHelper.ExecuteNonQuery(sql);
          return o;

      }

      public DataTable GetDS()
      {
          string sql = @"SELECT *,CASE STATUS
 WHEN  1 THEN '正常'
 ELSE '停用'
END statusdes FROM client_ipinfo ";
          DataTable dt=MySqlHelper.ExecuteDataTable(sql);
          return dt;
      }

      public DataTable GetDSById(int id)
      {
          string sql = @"SELECT * FROM client_ipinfo where id="+id;
          DataTable dt = MySqlHelper.ExecuteDataTable(sql);
          return dt;
      }

      public int Delete(int id)
      {
          string sql = " delete from client_ipinfo where id="+id;
          int o = MySqlHelper.ExecuteNonQuery(sql);
          return o;
      }
    }
}
