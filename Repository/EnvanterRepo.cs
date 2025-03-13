using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace EnvanterApiProjesi;
public class EnvanterRepo
{
    private readonly string? _connectionString;
    public EnvanterRepo(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    public string AssetSNMatcher(string asset, string seriNo)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {

            int count = 0;
            string finder = "SELECT COUNT(*) FROM EnvanterTablosu WHERE SeriNo = @seriNo";
            using (SqlCommand finderCmd = new SqlCommand(finder, conn))
            {
                conn.Open();
                finderCmd.Parameters.AddWithValue("@seriNo", seriNo);
                count = (int)finderCmd.ExecuteScalar();

                if (count <= 0)
                {
                    string query = "INSERT INTO EnvanterTablosu (Asset, SeriNo) VALUES (@asset, @seriNo)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@asset", asset);
                    cmd.Parameters.AddWithValue("@seriNo", seriNo);
                    cmd.ExecuteNonQuery();
                    return "Belirtilen seri no zaten veritabanında mevcut değil. Belirtilen asset numarası ile veritabanına eklenmiştir.";
                }
                else
                {
                    string query = "UPDATE EnvanterTablosu SET Asset = @asset WHERE SeriNo LIKE @seriNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@asset", asset);
                    cmd.Parameters.AddWithValue("@seriNo", seriNo);
                    cmd.ExecuteNonQuery();
                    return "Belirtilen seri no zaten veritabanında mevcut. Asset numarası değiştirildi.";

                }

            }
        }
    }
}