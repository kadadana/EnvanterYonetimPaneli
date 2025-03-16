using EnvanterApiProjesi.Models;
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
    public string AssetSNMatcher(EnvanterModel envanterModel)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {

            int count = 0;
            string finder = "SELECT COUNT(*) FROM EnvanterTablosu WHERE SeriNo = @seriNo";
            using (SqlCommand finderCmd = new SqlCommand(finder, conn))
            {
                conn.Open();
                finderCmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                count = (int)finderCmd.ExecuteScalar();

                try
                {

                    if (count <= 0)
                    {
                        string query = "INSERT INTO EnvanterTablosu (Asset, SeriNo) VALUES (@asset, @seriNo)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        cmd.ExecuteNonQuery();
                        return "Belirtilen seri no zaten veritabanında mevcut değil. Belirtilen asset numarası ile veritabanına eklenmiştir.";
                    }
                    else
                    {
                        string query = "UPDATE EnvanterTablosu SET Asset = @asset WHERE SeriNo LIKE @seriNo";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        cmd.ExecuteNonQuery();
                        return "Belirtilen seri no zaten veritabanında mevcut. Asset numarası değiştirildi.";

                    }
                }
                catch (Exception ex)
                {
                    return "Veritabanında düzenleme yaparken bir sorun oluştu." + ex;
                }

            }
        }
    }
    public string AddToSql(EnvanterModel envanterModel)
    {


        envanterModel.SeriNo = string.IsNullOrEmpty(envanterModel.SeriNo) ? "Bilinmiyor" : envanterModel.SeriNo;
        envanterModel.CompModel = string.IsNullOrEmpty(envanterModel.CompModel) ? "Bilinmiyor" : envanterModel.CompModel;
        envanterModel.CompName = string.IsNullOrEmpty(envanterModel.CompName) ? "Bilinmiyor" : envanterModel.CompName;
        envanterModel.RAM = string.IsNullOrEmpty(envanterModel.RAM) ? "Bilinmiyor" : envanterModel.RAM;
        envanterModel.DiskGB = string.IsNullOrEmpty(envanterModel.DiskGB) ? "Bilinmiyor" : envanterModel.DiskGB;
        envanterModel.MAC = string.IsNullOrEmpty(envanterModel.MAC) ? "Bilinmiyor" : envanterModel.MAC;
        envanterModel.ProcModel = string.IsNullOrEmpty(envanterModel.ProcModel) ? "Bilinmiyor" : envanterModel.ProcModel;
        envanterModel.Username = string.IsNullOrEmpty(envanterModel.Username) ? "Bilinmiyor" : envanterModel.Username;
        envanterModel.DateChanged = string.IsNullOrEmpty(envanterModel.DateChanged) ? "Bilinmiyor" : envanterModel.DateChanged;
        envanterModel.Asset = GetAssetFromSN(envanterModel.SeriNo);



        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            int count;
            string counter = "SELECT COUNT(*) FROM EnvanterTablosu WHERE SeriNo = @seriNo AND Asset = @asset";
            using (SqlCommand counterCmd = new SqlCommand(counter, conn))
            {
                conn.Open();
                counterCmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                counterCmd.Parameters.AddWithValue("@asset", envanterModel.Asset);

                count = (int)counterCmd.ExecuteScalar();
            }
            string creator = $"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{envanterModel.SeriNo}') " +
                                    "BEGIN " +
                                    $"CREATE TABLE \"{envanterModel.SeriNo}\" " +
                                    "(Asset NVARCHAR(Max), " +
                                    "SeriNo NVARCHAR(Max), " +
                                    "CompModel NVARCHAR(Max), " +
                                    "CompName NVARCHAR(Max), " +
                                    "RAM NVARCHAR(Max), " +
                                    "DiskGB NVARCHAR(Max), " +
                                    "MAC NVARCHAR(Max), " +
                                    "ProcModel NVARCHAR(Max), " +
                                    "Username NVARCHAR(Max), " +
                                    "DateChanged NVARCHAR(Max));" +
                                    "END;";

            using (SqlCommand creatorCmd = new SqlCommand(creator, conn))
            {
                creatorCmd.ExecuteNonQuery();
            }
            try
            {
                if (count > 0)
                {
                    Console.WriteLine(count);
                    string updater = "UPDATE EnvanterTablosu " +
                    "SET " +
                    "CompModel = @compModel, " +
                    "CompName = @compName, " +
                    "RAM = @RAM, " +
                    "DiskGB = @diskGB, " +
                    "MAC = @MAC, " +
                    "ProcModel = @procModel, " +
                    "Username = @username, " +
                    "DateChanged = @dateChanged " +
                    "WHERE SeriNo = @seriNo AND Asset = @asset";

                    using (SqlCommand updaterCmd = new SqlCommand(updater, conn))
                    {
                        updaterCmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        updaterCmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        updaterCmd.Parameters.AddWithValue("@compModel", envanterModel.CompModel);
                        updaterCmd.Parameters.AddWithValue("@compName", envanterModel.CompName);
                        updaterCmd.Parameters.AddWithValue("@RAM", envanterModel.RAM);
                        updaterCmd.Parameters.AddWithValue("@diskGB", envanterModel.DiskGB);
                        updaterCmd.Parameters.AddWithValue("@MAC", envanterModel.MAC);
                        updaterCmd.Parameters.AddWithValue("@procModel", envanterModel.ProcModel);
                        updaterCmd.Parameters.AddWithValue("@username", envanterModel.Username);
                        updaterCmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);
                        updaterCmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.SeriNo}]" +
                    "(Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged)" +
                    "VALUES (@asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged)";

                    using (SqlCommand inserter2Cmd = new SqlCommand(inserter2, conn))
                    {
                        inserter2Cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        inserter2Cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        inserter2Cmd.Parameters.AddWithValue("@compModel", envanterModel.CompModel);
                        inserter2Cmd.Parameters.AddWithValue("@compName", envanterModel.CompName);
                        inserter2Cmd.Parameters.AddWithValue("@RAM", envanterModel.RAM);
                        inserter2Cmd.Parameters.AddWithValue("@diskGB", envanterModel.DiskGB);
                        inserter2Cmd.Parameters.AddWithValue("@MAC", envanterModel.MAC);
                        inserter2Cmd.Parameters.AddWithValue("@procModel", envanterModel.ProcModel);
                        inserter2Cmd.Parameters.AddWithValue("@username", envanterModel.Username);
                        inserter2Cmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);
                        inserter2Cmd.ExecuteNonQuery();
                    }

                }
                else
                {


                    string inserter1 = "INSERT INTO EnvanterTablosu" +
                    "(Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged)" +
                    "VALUES (@asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged)";

                    using (SqlCommand inserter1Cmd = new SqlCommand(inserter1, conn))
                    {
                        inserter1Cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        inserter1Cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        inserter1Cmd.Parameters.AddWithValue("@compModel", envanterModel.CompModel);
                        inserter1Cmd.Parameters.AddWithValue("@compName", envanterModel.CompName);
                        inserter1Cmd.Parameters.AddWithValue("@RAM", envanterModel.RAM);
                        inserter1Cmd.Parameters.AddWithValue("@diskGB", envanterModel.DiskGB);
                        inserter1Cmd.Parameters.AddWithValue("@MAC", envanterModel.MAC);
                        inserter1Cmd.Parameters.AddWithValue("@procModel", envanterModel.ProcModel);
                        inserter1Cmd.Parameters.AddWithValue("@username", envanterModel.Username);
                        inserter1Cmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);
                        inserter1Cmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.SeriNo}]" +
                    "(Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged)" +
                    "VALUES (@asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged)";

                    using (SqlCommand inserter2Cmd = new SqlCommand(inserter2, conn))
                    {
                        inserter2Cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                        inserter2Cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                        inserter2Cmd.Parameters.AddWithValue("@compModel", envanterModel.CompModel);
                        inserter2Cmd.Parameters.AddWithValue("@compName", envanterModel.CompName);
                        inserter2Cmd.Parameters.AddWithValue("@RAM", envanterModel.RAM);
                        inserter2Cmd.Parameters.AddWithValue("@diskGB", envanterModel.DiskGB);
                        inserter2Cmd.Parameters.AddWithValue("@MAC", envanterModel.MAC);
                        inserter2Cmd.Parameters.AddWithValue("@procModel", envanterModel.ProcModel);
                        inserter2Cmd.Parameters.AddWithValue("@username", envanterModel.Username);
                        inserter2Cmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);
                        inserter2Cmd.ExecuteNonQuery();
                    }

                }
                return "Veriler veritabanina eklenmistir.";

            }


            catch (Exception ex)
            {
                return "Veriler veritabanina eklenirken bir sorunla karsilasildi." + ex;
            }


        }
    }
    public List<EnvanterModel>? GetEnvanterList(string tableName)
    {
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();
        string query = $"SELECT Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged FROM [{tableName}]";
        envanterList = TableFiller(query);
        return envanterList;
    }
    public string GetAssetFromSN(string seriNo)
    {
        string asset;
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string finder = $"SELECT Asset FROM EnvanterTablosu where SeriNo = '{seriNo}' AND Asset <> 'Bilinmiyor' AND Asset <> 'HATA'";

                using (SqlCommand finderCmd = new SqlCommand(finder, conn))
                {
                    asset = (string)finderCmd.ExecuteScalar();
                    return asset;
                }
            }

        }
        catch
        {
            return "HATA";
        }
    }
    public List<EnvanterModel>? GetSortedByDate(string tableName)
    {
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();
        string sorter = $"SELECT * FROM [{tableName}] ORDER BY CONVERT(DATETIME, DateChanged, 104) DESC";

        envanterList = TableFiller(sorter);
        return envanterList;
    }
    public List<EnvanterModel>? GetOrderedList(string tableName, string columnName, string method)
    {
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();

        switch (columnName)
        {
            case "DateChanged":
                string query = $"SELECT * FROM [{tableName}] ORDER BY CONVERT(DATETIME, DateChanged, 104) {method}";
                envanterList = TableFiller(query);
                break;
            case "RAM":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(REPLACE(RAM, ' GB', ''), ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(REPLACE(RAM, ' GB', ''), ',', '.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(REPLACE(RAM, ' GB', ''), ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(REPLACE(RAM, ' GB', ''), ',', '.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
                envanterList = TableFiller(query);
                break;
            case "DiskGB":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(REPLACE(DiskGB, ' GB', ''), ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(REPLACE(DiskGB, ' GB', ''), ',', '.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(REPLACE(DiskGB, ' GB', ''), ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(REPLACE(DiskGB, ' GB', ''), ',', '.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
                envanterList = TableFiller(query);
                break;
            default:
                query = $"SELECT * FROM [{tableName}] ORDER BY {columnName} {method}";
                envanterList = TableFiller(query);
                break;
        }
        return envanterList;
    }
    public List<EnvanterModel>? TableFiller(string command)
    {
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand fillerCmd = new SqlCommand(command, conn))
                using (SqlDataReader reader = fillerCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        envanterList?.Add(new EnvanterModel
                        {
                            Asset = reader.IsDBNull(0) ? "Bilinmiyor" : reader.GetString(0),
                            SeriNo = reader.IsDBNull(1) ? "Bilinmiyor" : reader.GetString(1),
                            CompModel = reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                            CompName = reader.IsDBNull(3) ? "Bilinmiyor" : reader.GetString(3),
                            RAM = reader.IsDBNull(4) ? "Bilinmiyor" : reader.GetString(4),
                            DiskGB = reader.IsDBNull(5) ? "Bilinmiyor" : reader.GetString(5),
                            MAC = reader.IsDBNull(6) ? "Bilinmiyor" : reader.GetString(6),
                            ProcModel = reader.IsDBNull(7) ? "Bilinmiyor" : reader.GetString(7),
                            Username = reader.IsDBNull(8) ? "Bilinmiyor" : reader.GetString(8),
                            DateChanged = reader.IsDBNull(9) ? "Bilinmiyor" : reader.GetString(9)
                        });
                    }
                }
            }
        }
        catch (Exception)
        {
            return null;
        }
        return envanterList;
    }
    public List<EnvanterModel>? GetSearchedTable(string tableName, string searchedColumn, string searchedValue){
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();
        string query = $"SELECT * FROM [{tableName}] WHERE {searchedColumn} LIKE '%{searchedValue}%'";
        envanterList = TableFiller(query);
        return envanterList;
    }


}