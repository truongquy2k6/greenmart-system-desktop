using DTO;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class CauHinhDAL
    {
        public CauHinhDTO GetCauHinh(string maCH)
        {
            CauHinhDTO ch = new CauHinhDTO { MaCH = maCH };
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaCH", maCH)
            };
            
            DataTable dt = DatabaseHelper.ExecuteQuery("sp_GetCauHinh", parameters);
            if (dt.Rows.Count > 0)
            {
                ch.BankId = dt.Rows[0]["BankId"].ToString() ?? "";
                ch.AccountNo = dt.Rows[0]["AccountNo"].ToString() ?? "";
                ch.AccountName = dt.Rows[0]["AccountName"].ToString() ?? "";
            }
            return ch;
        }

        public bool SaveCauHinh(CauHinhDTO ch)
        {
            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@MaCH", ch.MaCH),
                new SqlParameter("@BankId", ch.BankId),
                new SqlParameter("@AccountNo", ch.AccountNo),
                new SqlParameter("@AccountName", ch.AccountName)
            };

            return DatabaseHelper.ExecuteNonQuery("sp_SaveCauHinh", parameters) > 0;
        }
    }
}
