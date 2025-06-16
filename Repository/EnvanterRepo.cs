using System.Reflection;
using EnvanterYonetimPaneli.Models;
using Microsoft.Data.SqlClient;

namespace EnvanterYonetimPaneli;

public class EnvanterRepo
{
    private readonly string? _connectionString;

    public EnvanterRepo(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public void EnvanterTableCreator()
    {

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string creator = "IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'ENVANTER_TABLE') " +
                                "BEGIN " +
                                "CREATE TABLE [ENVANTER_TABLE] " +
                                "(ID NVARCHAR(Max), " +
                                "ASSET NVARCHAR(Max), " +
                                "SERI_NO NVARCHAR(Max), " +
                                "COMP_MODEL NVARCHAR(Max), " +
                                "COMP_NAME NVARCHAR(Max), " +
                                "RAM NVARCHAR(Max), " +
                                "DISK_GB NVARCHAR(Max), " +
                                "MAC NVARCHAR(Max), " +
                                "PROC_MODEL NVARCHAR(Max), " +
                                "USERNAME NVARCHAR(Max), " +
                                "DATE_CHANGED NVARCHAR(Max), " +
                                "ASSIGNED_USER NVARCHAR(MAX), " +
                                "LAST_IP_ADDRESS NVARCHAR(Max), " +
                                "LOG NVARCHAR(Max));" +
                                "END;";
            using (SqlCommand creatorCmd = new SqlCommand(creator, conn))
            {
                creatorCmd.ExecuteNonQuery();
            }
        }

    }
    public void SingularTableCreator(SqlConnection conn, EnvanterModel envanterModel)
    {
        string creator = $"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{envanterModel.Id}') " +
                                    "BEGIN " +
                                    $"CREATE TABLE \"{envanterModel.Id}\" " +
                                    "(ID NVARCHAR(Max), " +
                                    "ASSET NVARCHAR(Max), " +
                                    "SERI_NO NVARCHAR(Max), " +
                                    "COMP_MODEL NVARCHAR(Max), " +
                                    "COMP_NAME NVARCHAR(Max), " +
                                    "RAM NVARCHAR(Max), " +
                                    "DISK_GB NVARCHAR(Max), " +
                                    "MAC NVARCHAR(Max), " +
                                    "PROC_MODEL NVARCHAR(Max), " +
                                    "USERNAME NVARCHAR(Max), " +
                                    "DATE_CHANGED NVARCHAR(Max), " +
                                    "ASSIGNED_USER NVARCHAR(MAX), " +
                                    "LAST_IP_ADDRESS NVARCHAR(Max), " +
                                    "LOG NVARCHAR(Max));" +
                                    "END;";
        using (SqlCommand creatorCmd = new SqlCommand(creator, conn))
        {
            creatorCmd.ExecuteNonQuery();
        }
    }

    public void DiskTableCreator(SqlConnection conn, string driveTableName)
    {
        string creator = $"IF NOT EXISTS(SELECT * FROM sys.tables WHERE name = '{driveTableName}') " +
                                        "BEGIN " +
                                        $"CREATE TABLE \"{driveTableName}\" " +
                                        "(ID NVARCHAR(Max), " +
                                        "NAME NVARCHAR(Max), " +
                                        "TOTAL_SIZE_GB NVARCHAR(Max), " +
                                        "TOTAL_FREE_SPACE_GB NVARCHAR(Max), " +
                                        "DATE_CHANGED NVARCHAR(Max));" +
                                        "END;";
        using (SqlCommand creatorCmd = new SqlCommand(creator, conn))
        {
            creatorCmd.ExecuteNonQuery();
        }

    }

    public string AssetSNMatcher(EnvanterModel envanterModel)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            int count = 0;
            string finder = "SELECT COUNT(*) FROM ENVANTER_TABLE WHERE SERI_NO = @seriNo";
            using (SqlCommand finderCmd = new SqlCommand(finder, conn))
            {

                finderCmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                count = (int)finderCmd.ExecuteScalar();

                try
                {

                    if (count <= 0)
                    {
                        AddToSql(envanterModel);
                        return "Belirtilen seri no zaten veritabanında mevcut değil. Belirtilen asset numarası ile veritabanına eklenmiştir.";
                    }
                    else
                    {
                        AddToSql(envanterModel);
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

        EnvanterTableCreator();

        //envanterModel.SeriNo = string.IsNullOrEmpty(envanterModel.SeriNo) ? "Bilinmiyor" : envanterModel.SeriNo;
        //envanterModel.CompModel = string.IsNullOrEmpty(envanterModel.CompModel) ? "Bilinmiyor" : envanterModel.CompModel;
        //envanterModel.CompName = string.IsNullOrEmpty(envanterModel.CompName) ? "Bilinmiyor" : envanterModel.CompName;
        //envanterModel.RAM = string.IsNullOrEmpty(envanterModel.RAM) ? "Bilinmiyor" : envanterModel.RAM;
        //envanterModel.DiskGB = string.IsNullOrEmpty(envanterModel.DiskGB) ? "Bilinmiyor" : envanterModel.DiskGB;
        //envanterModel.MAC = string.IsNullOrEmpty(envanterModel.MAC) ? "Bilinmiyor" : envanterModel.MAC;
        //envanterModel.ProcModel = string.IsNullOrEmpty(envanterModel.ProcModel) ? "Bilinmiyor" : envanterModel.ProcModel;
        //envanterModel.Username = string.IsNullOrEmpty(envanterModel.Username) ? "Bilinmiyor" : envanterModel.Username;
        //envanterModel.DateChanged = string.IsNullOrEmpty(envanterModel.DateChanged) ? "Bilinmiyor" : envanterModel.DateChanged;
        //envanterModel.LastIpAddress = string.IsNullOrEmpty(envanterModel.LastIpAddress) ? "Bilinmiyor" : envanterModel.LastIpAddress;
        //envanterModel.Log = string.IsNullOrEmpty(envanterModel.Log) ? "Bilinmiyor" : envanterModel.Log;
        //envanterModel.Asset = string.IsNullOrEmpty(envanterModel.Asset) ? string.IsNullOrEmpty(GetCellBySeriNo("ENVANTER_TABLE", "Asset", envanterModel.SeriNo)) ? "Bilinmiyor" : GetCellBySeriNo("ENVANTER_TABLE", "Asset", envanterModel.SeriNo) : envanterModel.Asset;
        //envanterModel.Id = IdDeterminer(envanterModel.SeriNo);
        //envanterModel.AssignedUser = string.IsNullOrEmpty(envanterModel.AssignedUser) ?
        //    string.IsNullOrEmpty(GetCellById("ENVANTER_TABLE", "ASSIGNED_USER", envanterModel.Id)) ? "Bilinmiyor" : GetCellById("ENVANTER_TABLE", "ASSIGNED_USER", envanterModel.Id) : envanterModel.AssignedUser;




        foreach (PropertyInfo prop in envanterModel.GetType().GetProperties())
        {
            if (prop.PropertyType == typeof(string) && prop.CanWrite)
            {
                var value = prop.GetValue(envanterModel) as string;

                if (string.IsNullOrWhiteSpace(value))
                {
                    foreach (var label in Helpers.LabelHelper.LabelsToSqlLabels)
                    {
                        if (prop.Name == label.Key)
                        {
                            var val2 = "";
                            switch (prop.Name)
                            {
                                case "Asset":
                                    val2 = GetCellBySeriNo("ENVANTER_TABLE", label.Value, envanterModel.SeriNo);
                                    if (string.IsNullOrWhiteSpace(val2))
                                    {
                                        prop.SetValue(envanterModel, "Bilinmiyor");
                                    }
                                    else
                                    {
                                        prop.SetValue(envanterModel, val2);
                                    }
                                    break;
                                case "Id":
                                    prop.SetValue(envanterModel, IdDeterminer(envanterModel.SeriNo));
                                    break;
                                default:
                                    val2 = GetCellById("ENVANTER_TABLE", label.Value, envanterModel.Id);
                                    if (string.IsNullOrWhiteSpace(val2))
                                    {
                                        prop.SetValue(envanterModel, "Bilinmiyor");
                                    }
                                    else
                                    {
                                        prop.SetValue(envanterModel, val2);
                                    }
                                    break;
                            }
                            prop.GetValue(envanterModel);


                        }
                    }
                }
                else
                {
                    prop.SetValue(envanterModel, value);
                }
            }

        }

        string driveTableName = $"DISK_{envanterModel.Id}";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SingularTableCreator(conn, envanterModel);
            DiskTableCreator(conn, driveTableName);
            DiskAdder(envanterModel, driveTableName, conn);
            int count;
            string counter = "SELECT COUNT(*) FROM ENVANTER_TABLE WHERE ID = @id";
            using (SqlCommand counterCmd = new SqlCommand(counter, conn))
            {
                counterCmd.Parameters.AddWithValue("@id", envanterModel.Id);

                count = (int)counterCmd.ExecuteScalar();
            }

            try
            {
                if (count > 0)
                {
                    string updater = "UPDATE ENVANTER_TABLE " +
                    "SET " +
                    "ID = @id," +
                    "SERI_NO = @seriNo, " +
                    "ASSET= @asset, " +
                    "COMP_MODEL = @compModel, " +
                    "COMP_NAME = @compName, " +
                    "RAM = @RAM, " +
                    "DISK_GB = @diskGB, " +
                    "MAC = @MAC, " +
                    "PROC_MODEL = @procModel, " +
                    "USERNAME = @username, " +
                    "DATE_CHANGED = @dateChanged, " +
                    "ASSIGNED_USER = @assignedUser, " +
                    "LAST_IP_ADDRESS = @lastIpAddress, " +
                    "LOG = @log " +
                    "WHERE ID = @id";

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
                        updaterCmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                        updaterCmd.Parameters.AddWithValue("@log", envanterModel.Log);
                        updaterCmd.Parameters.AddWithValue("@id", envanterModel.Id);

                        updaterCmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.Id}] " +
                    "(ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, PROC_MODEL, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS, LOG)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser, @lastIpAddress, @log)";

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
                        inserter2Cmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                        inserter2Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        inserter2Cmd.Parameters.AddWithValue("@log", envanterModel.Log);

                        inserter2Cmd.ExecuteNonQuery();
                    }

                }
                else
                {
                    string inserter1 = "INSERT INTO ENVANTER_TABLE " +
                    "(ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, PROC_MODEL, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS, LOG)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser, @lastIpAddress, @log)";

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
                        inserter1Cmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                        inserter1Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        inserter1Cmd.Parameters.AddWithValue("@log", envanterModel.Log);

                        inserter1Cmd.ExecuteNonQuery();
                    }

                    string inserter2 = $"INSERT INTO [{envanterModel.Id}] " +
                    "(ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, PROC_MODEL, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS, LOG)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser, @lastIpAddress, @log)";

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
                        inserter2Cmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                        inserter2Cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                        inserter2Cmd.Parameters.AddWithValue("@log", envanterModel.Log);
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
        string query = $"SELECT ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, PROC_MODEL, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS, LOG FROM [{tableName}] WHERE ID = '{id}'";
        envanterList = ListFillerByTable(query);
        return envanterList;
    }
    public string? GetCellById(string tableName, string column, string id)
    {
        string cell;
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string finder = $"SELECT [{column}] FROM [{tableName}] WHERE ID = '{id}'";

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
    public string GetCellBySeriNo(string tableName, string column, string seriNo)
    {
        string cell;
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string finder = $"SELECT [{column}] FROM [{tableName}] WHERE SERI_NO = '{seriNo}'";

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
        string sorter = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DATE_CHANGED, 104) DESC";

        envanterList = ListFillerByTable(sorter);
        return envanterList;
    }
    public List<EnvanterModel>? GetOrderedList(string tableName, string columnName, string method)
    {
        List<EnvanterModel>? envanterList;

        switch (columnName)
        {
            case "DateChanged":
                string query = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DATE_CHANGED, 104) {method}";
                envanterList = ListFillerByTable(query);
                break;
            case "RAM":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(RAM, ',','.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
                envanterList = ListFillerByTable(query);
                break;
            case "DiskGB":
                query = method == "asc" ? $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(DISK_GB, ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(DISK_GB, ',', '.') AS FLOAT) ELSE CAST(1.0E+38 AS FLOAT) END ASC; "
                : $"SELECT * FROM [{tableName}] ORDER BY CASE WHEN TRY_CAST(REPLACE(DISK_GB, ',', '.') AS FLOAT) IS NOT NULL THEN TRY_CAST(REPLACE(DISK_GB, ',', '.') AS FLOAT) ELSE CAST(-1.0E+38 AS FLOAT) END DESC;";
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

                            Id = reader.IsDBNull(0) ? "Bilinmiyor" : reader.GetString(0),
                            Asset = reader.IsDBNull(1) ? "Bilinmiyor" : reader.GetString(1),
                            SeriNo = reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                            CompModel = reader.IsDBNull(3) ? "Bilinmiyor" : reader.GetString(3),
                            CompName = reader.IsDBNull(4) ? "Bilinmiyor" : reader.GetString(4),
                            RAM = reader.IsDBNull(5) ? "Bilinmiyor" : reader.GetString(5),
                            DiskGB = reader.IsDBNull(6) ? "Bilinmiyor" : reader.GetString(6),
                            MAC = reader.IsDBNull(7) ? "Bilinmiyor" : reader.GetString(7),
                            ProcModel = reader.IsDBNull(8) ? "Bilinmiyor" : reader.GetString(8),
                            Username = reader.IsDBNull(9) ? "Bilinmiyor" : reader.GetString(9),
                            DateChanged = reader.IsDBNull(10) ? "Bilinmiyor" : reader.GetString(10),
                            AssignedUser = reader.IsDBNull(11) ? "Bilinmiyor" : reader.GetString(11),
                            LastIpAddress = reader.IsDBNull(12) ? "Bilinmiyor" : reader.GetString(12),
                            Log = reader.IsDBNull(13) ? "Bilinmiyor" : reader.GetString(13)


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
        if (searchedColumn == "RAM" || searchedColumn == "DISK_GB")
        {
            query = $"SELECT * FROM [{tableName}] WHERE TRY_CAST(REPLACE({searchedColumn}, ',','.') AS FLOAT) >= {searchedValue1} and TRY_CAST(REPLACE({searchedColumn},',','.') AS FLOAT) <= {searchedValue2}";
            envanterList = ListFillerByTable(query);
            return envanterList;
        }
        else if (searchedColumn == "DATE_CHANGED")
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
        envanterModel.LastIpAddress = envanterList?[0].LastIpAddress;
        envanterModel.Log = envanterList?[0].Log;

        return envanterModel;

    }

    public string IdDeterminer(string seriNo)
    {
        int result;
        string id = "1";
        string counter = $"SELECT COUNT(*) FROM ENVANTER_TABLE WHERE SERI_NO = @seriNo";
        string getId = "SELECT Id FROM ENVANTER_TABLE WHERE SERI_NO = @seriNo";
        string getMaxId = "SELECT MAX(Id) FROM ENVANTER_TABLE";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand counterCmd = new SqlCommand(counter, conn))
            using (SqlCommand getIdCmd = new SqlCommand(getId, conn))
            using (SqlCommand getMaxIdCmd = new SqlCommand(getMaxId, conn))
            {
                counterCmd.Parameters.AddWithValue("@seriNo", seriNo);
                result = Convert.ToInt32(counterCmd.ExecuteScalar());

                if (result >= 1)
                {
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

    public string EditSql(EnvanterModel envanterModel)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string updater = $"UPDATE ENVANTER_TABLE " +
                    "SET " +
                    "ID = @id," +
                    "SERI_NO = @seriNo, " +
                    "ASSET = @asset, " +
                    "COMP_MODEL = @compModel, " +
                    "COMP_NAME = @compName, " +
                    "RAM = @RAM, " +
                    "DISK_GB = @diskGB, " +
                    "MAC = @MAC, " +
                    "PROC_MODEL = @procModel, " +
                    "USERNAME = @username, " +
                    "DATE_CHANGED = @dateChanged, " +
                    "ASSIGNED_USER = @assignedUser, " +
                    "LAST_IP_ADDRESS = @lastIpAddress, " +
                    "LOG = @log " +
                    "WHERE ID = @id";

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
                updaterCmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                updaterCmd.Parameters.AddWithValue("@log", envanterModel.Log);
                updaterCmd.Parameters.AddWithValue("@id", envanterModel.Id);

                updaterCmd.ExecuteNonQuery();
            }
            string inserter2 = $"INSERT INTO [{envanterModel.Id}]" +
            "(ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, PROC_MODEL, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS, LOG)" +
            "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, @procModel, @username, @dateChanged, @assignedUser, @lastIpAddress, @log)";

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
                inserter2Cmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                inserter2Cmd.Parameters.AddWithValue("@log", envanterModel.Log);

                inserter2Cmd.ExecuteNonQuery();
            }
            return "Envanter basariyla duzenlendi.";
        }
    }

    public void DiskAdder(EnvanterModel envanterModel, string driveTableName, SqlConnection conn)
    {
        if (envanterModel.Drives != null)
        {
            foreach (var disk in envanterModel.Drives)
            {
                disk.Name = string.IsNullOrEmpty(disk.Name) ? "Bilinmiyor" : disk.Name;
                disk.TotalSizeGB = string.IsNullOrEmpty(disk.TotalSizeGB) ? "Bilinmiyor" : disk.TotalSizeGB;
                disk.TotalFreeSpaceGB = string.IsNullOrEmpty(disk.TotalFreeSpaceGB) ? "Bilinmiyor" : disk.TotalFreeSpaceGB;


                int count;
                string counter = $"SELECT COUNT(*) FROM [{driveTableName}] WHERE ID = @id AND NAME = @name";
                using (SqlCommand counterCmd = new SqlCommand(counter, conn))
                {
                    counterCmd.Parameters.AddWithValue("@id", envanterModel.Id);
                    counterCmd.Parameters.AddWithValue("@name", disk.Name);

                    count = (int)counterCmd.ExecuteScalar();
                }

                if (count > 0)
                {
                    string updater = $"UPDATE [{driveTableName}] SET ID = @id, NAME = @name, TOTAL_SIZE_GB = @totalSizeGB, TOTAL_FREE_SPACE_GB = @totalFreeSpace, DATE_CHANGED = @dateChanged WHERE ID = @id AND NAME = @name";
                    using (SqlCommand inserter3Cmd = new SqlCommand(updater, conn))
                    {
                        inserter3Cmd.Parameters.AddWithValue("@id", envanterModel.Id);
                        inserter3Cmd.Parameters.AddWithValue("@name", disk.Name);
                        inserter3Cmd.Parameters.AddWithValue("@totalSizeGB", disk.TotalSizeGB);
                        inserter3Cmd.Parameters.AddWithValue("@totalFreeSpace", disk.TotalFreeSpaceGB);
                        inserter3Cmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);

                        inserter3Cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    string inserter = $"INSERT INTO [{driveTableName}] (ID, NAME, TOTAL_SIZE_GB, TOTAL_FREE_SPACE_GB, DATE_CHANGED) VALUES (@id, @name, @totalSizeGB, @totalFreeSpace, @dateChanged)";
                    using (SqlCommand inserter3Cmd = new SqlCommand(inserter, conn))
                    {
                        inserter3Cmd.Parameters.AddWithValue("@id", envanterModel.Id);
                        inserter3Cmd.Parameters.AddWithValue("@name", disk.Name);
                        inserter3Cmd.Parameters.AddWithValue("@totalSizeGB", disk.TotalSizeGB);
                        inserter3Cmd.Parameters.AddWithValue("@totalFreeSpace", disk.TotalFreeSpaceGB);
                        inserter3Cmd.Parameters.AddWithValue("@dateChanged", envanterModel.DateChanged);


                        inserter3Cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
    public List<DriveInfoModel>? GetDiskListById(string id)
    {


        string query = $"SELECT * FROM [DISK_{id}]";
        List<DriveInfoModel>? diskList = new List<DriveInfoModel>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    diskList?.Add(new DriveInfoModel
                    {
                        Name = reader.IsDBNull(1) ? "Bilinmiyor" : reader.GetString(1),
                        TotalSizeGB = reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                        TotalFreeSpaceGB = reader.IsDBNull(3) ? "Bilinmiyor" : reader.GetString(3)
                    });
                }
            }
        }
        return diskList;


    }


}