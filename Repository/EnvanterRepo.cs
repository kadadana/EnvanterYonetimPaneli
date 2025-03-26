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
        envanterModel.Asset = string.IsNullOrEmpty(envanterModel.Asset) ? string.IsNullOrEmpty(GetCellById("EnvanterTablosu", "Asset", envanterModel.SeriNo)) ? "Bilinmiyor" : GetCellById("EnvanterTablosu", "Asset", envanterModel.SeriNo) : envanterModel.Asset;
        envanterModel.AssignedUser = string.IsNullOrEmpty(envanterModel.AssignedUser) ? string.IsNullOrEmpty(GetCellById("EnvanterTablosu", "AssignedUser", envanterModel.SeriNo)) ? "Bilinmiyor" : GetCellById("EnvanterTablosu", "AssignedUser", envanterModel.SeriNo) : envanterModel.AssignedUser;
        envanterModel.Id = IdDeterminer(envanterModel.Asset, envanterModel.SeriNo);

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            int count;
            string counter = "SELECT COUNT(*) FROM EnvanterTablosu WHERE Id = @id";
            using (SqlCommand counterCmd = new SqlCommand(counter, conn))
            {
                conn.Open();
                counterCmd.Parameters.AddWithValue("@id", envanterModel.Id);

                count = (int)counterCmd.ExecuteScalar();
            }
            string creator = $"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{envanterModel.Id}') " +
                                    "BEGIN " +
                                    $"CREATE TABLE \"{envanterModel.Id}\" " +
                                    "(Asset NVARCHAR(Max), " +
                                    "SeriNo NVARCHAR(Max), " +
                                    "CompModel NVARCHAR(Max), " +
                                    "CompName NVARCHAR(Max), " +
                                    "RAM NVARCHAR(Max), " +
                                    "DiskGB NVARCHAR(Max), " +
                                    "MAC NVARCHAR(Max), " +
                                    "ProcModel NVARCHAR(Max), " +
                                    "Username NVARCHAR(Max), " +
                                    "DateChanged NVARCHAR(Max), " +
                                    "AssignedUser NVARCHAR(MAX), " +
                                    "Id NVARCHAR(Max));" +
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
                    "SeriNo = @seriNo, " +
                    "Asset= @asset, " +
                    "CompModel = @compModel, " +
                    "CompName = @compName, " +
                    "RAM = @RAM, " +
                    "DiskGB = @diskGB, " +
                    "MAC = @MAC, " +
                    "ProcModel = @procModel, " +
                    "Username = @username, " +
                    "DateChanged = @dateChanged, " +
                    "AssignedUser = @assignedUser " +
                    "WHERE Id = @id";

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
                        updaterCmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        updaterCmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);
                        updaterCmd.Parameters.AddWithValue("@id", envanterModel.Id);

                        updaterCmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.Id}]" +
                    "(Id, Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged, AssignedUser)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser)";

                    using (SqlCommand inserter2Cmd = new SqlCommand(inserter2, conn))
                    {
                        inserter2Cmd.Parameters.AddWithValue("@id", envanterModel.Id);
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
                        inserter2Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        inserter2Cmd.ExecuteNonQuery();
                    }

                }
                else
                {


                    string inserter1 = "INSERT INTO EnvanterTablosu" +
                    "(Id, Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged, AssignedUser)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser)";

                    using (SqlCommand inserter1Cmd = new SqlCommand(inserter1, conn))
                    {
                        inserter1Cmd.Parameters.AddWithValue("@id", envanterModel.Id);
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
                        inserter1Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        inserter1Cmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.Id}]" +
                    "(Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged, AssignedUser)" +
                    "VALUES (@asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser)";

                    using (SqlCommand inserter2Cmd = new SqlCommand(inserter2, conn))
                    {
                        inserter2Cmd.Parameters.AddWithValue("@id", envanterModel.Id);
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
                        inserter2Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
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
    public List<EnvanterModel>? GetRowById(string id, string tableName)
    {
        List<EnvanterModel>? envanterList;
        string query = $"SELECT Asset, SeriNo, CompModel, CompName, RAM, DiskGB, MAC, ProcModel, Username, DateChanged, AssignedUser, Id FROM [{tableName}] WHERE Id = '{id}'";
        envanterList = ListFillerByTable(query);
        return envanterList;
    }
    public string GetCellById(string tableName, string column, string id)
    {
        string cell;
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string finder = $"SELECT [{column}] FROM [{tableName}] WHERE Id = '{id}'";

                using (SqlCommand finderCmd = new SqlCommand(finder, conn))
                {
                    cell = (string)finderCmd.ExecuteScalar();
                    return cell;
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
        List<EnvanterModel>? envanterList;
        string sorter = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DateChanged, 104) DESC";

        envanterList = ListFillerByTable(sorter);
        return envanterList;
    }
    public List<EnvanterModel>? GetOrderedList(string tableName, string columnName, string method)
    {
        List<EnvanterModel>? envanterList;

        switch (columnName)
        {
            case "DateChanged":
                string query = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DateChanged, 104) {method}";
                envanterList = ListFillerByTable(query);
                break;
            case "RAM":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
                envanterList = ListFillerByTable(query);
                break;
            case "DiskGB":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(DiskGB, ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(DiskGB, ',', '.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(DiskGB, ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(DiskGB, ',', '.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
                envanterList = ListFillerByTable(query);
                break;
            default:
                query = $"SELECT * FROM [{tableName}] ORDER BY {columnName} {method}";
                envanterList = ListFillerByTable(query);
                break;
        }
        return envanterList;
    }
    public List<EnvanterModel>? ListFillerByTable(string command)
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
                            DateChanged = reader.IsDBNull(9) ? "Bilinmiyor" : reader.GetString(9),
                            AssignedUser = reader.IsDBNull(10) ? "Bilinmiyor" : reader.GetString(10),
                            Id = reader.IsDBNull(11) ? "Bilinmiyor" : reader.GetString(11)
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

    public List<EnvanterModel>? GetSearchedTable(string tableName, string searchedColumn, string searchedValue1, string? searchedValue2)
    {
        List<EnvanterModel>? envanterList;
        string query;
        if (searchedColumn == "RAM" || searchedColumn == "DiskGB")
        {
            query = $"SELECT * FROM [{tableName}] WHERE TRY_CAST(REPLACE({searchedColumn}, ',','.') AS FLOAT) >= {searchedValue1} and TRY_CAST(REPLACE({searchedColumn},',','.') AS FLOAT) <= {searchedValue2}";
            envanterList = ListFillerByTable(query);
            return envanterList;
        }
        else if (searchedColumn == "DateChanged")
        {
            query = $"SELECT * FROM [{tableName}] WHERE TRY_CONVERT(datetime, DateChanged, 104) BETWEEN '{searchedValue1}' AND '{searchedValue2}'";
            envanterList = ListFillerByTable(query);
            return envanterList;
        }
        else
        {
            query = $"SELECT * FROM [{tableName}] WHERE {searchedColumn} LIKE '%{searchedValue1}%'";
            envanterList = ListFillerByTable(query);
            return envanterList;
        }

    }

    public EnvanterModel? GetModelFromList(List<EnvanterModel>? envanterList)
    {

        EnvanterModel? envanterModel = new EnvanterModel();

        envanterModel.Id = envanterList?[0].Id;
        envanterModel.Asset = envanterList?[0].Asset;
        envanterModel.SeriNo = envanterList?[0].SeriNo;
        envanterModel.CompModel = envanterList?[0].CompModel;
        envanterModel.CompName = envanterList?[0].CompName;
        envanterModel.RAM = envanterList?[0].RAM;
        envanterModel.DiskGB = envanterList?[0].DiskGB;
        envanterModel.MAC = envanterList?[0].MAC;
        envanterModel.ProcModel = envanterList?[0].ProcModel;
        envanterModel.Username = envanterList?[0].Username;
        envanterModel.DateChanged = envanterList?[0].DateChanged;
        envanterModel.AssignedUser = envanterList?[0].AssignedUser;


        return envanterModel;

    }

    public string IdDeterminer(string asset, string seriNo)
    {
        int result;
        string id = "1";
        string counter = $"SELECT COUNT(*) FROM EnvanterTablosu WHERE Asset = @asset AND SeriNo = @seriNO";
        string getId = "SELECT Id FROM EnvanterTablosu WHERE Asset = @asset AND SeriNo = @seriNO";
        string getMaxId = "SELECT MAX(Id) FROM EnvanterTablosu";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand counterCmd = new SqlCommand(counter, conn))
            using (SqlCommand getIdCmd = new SqlCommand(getId, conn))
            using (SqlCommand getMaxIdCmd = new SqlCommand(getMaxId, conn))
            {
                counterCmd.Parameters.AddWithValue("@asset", asset);
                counterCmd.Parameters.AddWithValue("@seriNo", seriNo);
                result = Convert.ToInt32(counterCmd.ExecuteScalar());

                if (result >= 1)
                {
                    getIdCmd.Parameters.AddWithValue("@asset", asset);
                    getIdCmd.Parameters.AddWithValue("@seriNo", seriNo);
                    object idResult = getIdCmd.ExecuteScalar();
                    id = idResult != DBNull.Value ? Convert.ToInt32(idResult).ToString() : "1";
                }
                else
                {
                    object maxIdResult = getMaxIdCmd.ExecuteScalar();
                    int maxId = (maxIdResult != DBNull.Value && maxIdResult != null) ? Convert.ToInt32(maxIdResult) + 1 : 1;

                    id = maxId.ToString();
                }
            }

        }
        return id;
    }

}