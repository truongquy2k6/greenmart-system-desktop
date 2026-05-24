using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL
{
    /// <summary>
    /// Lớp hỗ trợ tương tác trực tiếp với SQL Server.
    /// Cung cấp các phương thức thực thi Stored Procedure nhanh chóng.
    /// </summary>
    public class DatabaseHelper
    {
        // Chuỗi kết nối (Lưu ý: Trong thực tế nên để ở file app.config hoặc appsettings.json để bảo mật)
        public static readonly string ConnectionString =
            @"Data Source=pyrex.myvnc.com,14330;Initial Catalog=QuanLyHeThongGreenMart;Persist Security Info=True;User ID=user;Password=123;TrustServerCertificate=True;";

        /// <summary>
        /// Khởi tạo và trả về một đối tượng kết nối SQL.
        /// </summary>
        /// <returns>SqlConnection</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Thực thi Stored Procedure trả về một bảng dữ liệu (Dùng cho lệnh SELECT).
        /// </summary>
        /// <param name="spName">Tên Stored Procedure.</param>
        /// <param name="parameters">Mảng các tham số truyền vào (nếu có).</param>
        /// <returns>DataTable chứa kết quả truy vấn.</returns>
        public static DataTable ExecuteQuery(string spName, SqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Thực thi Stored Procedure không trả về dữ liệu (Dùng cho INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="spName">Tên Stored Procedure.</param>
        /// <param name="parameters">Mảng các tham số truyền vào.</param>
        /// <returns>Số dòng bị tác động trong Database.</returns>
        public static int ExecuteNonQuery(string spName, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure trả về một giá trị duy nhất (Dùng cho COUNT, MAX, hoặc lấy ID vừa tạo).
        /// </summary>
        /// <param name="spName">Tên Stored Procedure.</param>
        /// <param name="parameters">Mảng các tham số truyền vào.</param>
        /// <returns>Giá trị đầu tiên của dòng đầu tiên, hoặc null.</returns>
        public static object? ExecuteScalar(string spName, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}