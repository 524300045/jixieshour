using StriveEngine.BinaryDemoServer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StriveEngine.BinaryDemoServer.Dal
{
    public  class ClientLogDal
    {

        public int Insert(ClientLogModel model)
        {
            string sql = " INSERT INTO clientlog (NAME,CODE,ip,content)  VALUE ('{0}','{1}','{2}','{3}') ";
            sql = string.Format(sql,model.Name,model.Code,model.IP,model.Content);
            int o = MySqlHelper.ExecuteNonQuery(sql);
            return o;
        }


        public DataTable GetLogDt(int pageIndex, int pageSize, string code, string begintime, string endtime)
        {
            string sql = " SELECT * FROM  clientlog WHERE 1=1";
            if (!string.IsNullOrEmpty(code))
            {
                sql += " and name like '%" + code + "%'";
            }
            sql += " and create_time>='"+begintime+"'";
            sql += " and create_time<='"+endtime+"' ";
            sql += " ORDER BY id DESC ";
            sql += "  LIMIT {0},{1}  ";
            sql = string.Format(sql,pageIndex*pageSize,pageSize);
            DataTable dt = MySqlHelper.ExecuteDataTable(sql);
            return dt;
        }


        public int GetLogDtCount(string code,string begintime,string endtime)
        {
            string sql = " SELECT count(0) FROM  clientlog WHERE 1=1";
            if (!string.IsNullOrEmpty(code))
            {
                sql += " and name like '%" + code + "%'";
            }
            sql += " and create_time>='" + begintime + "'";
            sql += " and create_time<='" + endtime + "' ";
 
            object o = MySqlHelper.ExecuteScalar(sql);
            return Convert.ToInt32(o);
        }

    }
}
