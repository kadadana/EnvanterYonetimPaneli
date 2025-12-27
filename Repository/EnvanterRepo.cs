using System.Reflection;
using EnvanterYonetimPaneli.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using EnvanterYonetimPaneli.Helpers;
using System.Globalization;

namespace EnvanterYonetimPaneli;

public class EnvanterRepo
{

    private readonly string _connectionString;

    public EnvanterRepo(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string bulunamadı");
    }
    public string AssetSNMatcher(EnvanterModel envanterModel)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            int count = 0;
            string finder = "SELECT COUNT(*) FROM ENVANTER WHERE SERI_NO = @seriNo";
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

        ModelFiller(envanterModel);

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            DiskAdder(envanterModel, conn);

            try
            {
                string query = "INSERT INTO ENVANTER " +
                    "(ENVANTER_ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB, MAC, " +
                    "PROC_MODEL, OS_NAME, OS_VER, USERNAME, ASSIGNED_USER, LAST_IP_ADDRESS, LOG_TEXT)" +
                    "VALUES (@id, @asset, @seriNo, @compModel, @compName, @RAM, @diskGB, @MAC, " +
                    "@procModel, @osName, @osVer, @username, @assignedUser, @lastIpAddress, @log)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", envanterModel.Id);
                    cmd.Parameters.AddWithValue("@asset", envanterModel.Asset);
                    cmd.Parameters.AddWithValue("@seriNo", envanterModel.SeriNo);
                    cmd.Parameters.AddWithValue("@compModel", envanterModel.CompModel);
                    cmd.Parameters.AddWithValue("@compName", envanterModel.CompName);
                    cmd.Parameters.AddWithValue("@RAM", envanterModel.RAM);
                    cmd.Parameters.AddWithValue("@diskGB", envanterModel.DiskGB);
                    cmd.Parameters.AddWithValue("@MAC", envanterModel.MAC);
                    cmd.Parameters.AddWithValue("@procModel", envanterModel.ProcModel);
                    cmd.Parameters.AddWithValue("@osName", envanterModel.OsName);
                    cmd.Parameters.AddWithValue("@osVer", envanterModel.OsVer);
                    cmd.Parameters.AddWithValue("@username", envanterModel.Username);
                    cmd.Parameters.AddWithValue("@lastIpAddress", envanterModel.LastIpAddress);
                    cmd.Parameters.AddWithValue("@assignedUser", envanterModel.AssignedUser);
                    cmd.Parameters.AddWithValue("@log", envanterModel.Log);

                    cmd.ExecuteNonQuery();
                }
                return "Veriler veritabanina eklenmistir.";

            }


            catch (Exception ex)
            {
                return "Veriler veritabanina eklenirken bir sorunla karsilasildi." + ex;
            }


        }
    }
    private void ModelFiller(EnvanterModel envanterModel)
    {
        foreach (PropertyInfo prop in envanterModel.GetType().GetProperties())
        {
            if (!prop.CanWrite) continue;

            if (prop.PropertyType == typeof(string))
            {
                var value = prop.GetValue(envanterModel) as string;
                if (string.IsNullOrWhiteSpace(value))
                {
                    foreach (var label in LabelHelper.ModelLabelsToSqlLabels)
                    {
                        if (prop.Name == label.Key)
                        {
                            var val2 = "";
                            switch (prop.Name)
                            {
                                case "Id":
                                    prop.SetValue(envanterModel, IdDeterminer(envanterModel.SeriNo));
                                    break;
                                default:
                                    val2 = GetCellById<string>(label.Value, envanterModel.Id!);
                                    System.Console.WriteLine("Val2: " + val2);
                                    prop.SetValue(envanterModel, string.IsNullOrWhiteSpace(val2) ? "Bilinmiyor" : val2);
                                    break;
                            }
                        }
                    }
                }
            }
            else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
            {
                var doubleValue = prop.GetValue(envanterModel) as double?;
                if (doubleValue == null || doubleValue == 0)
                {
                    //TODO: sql den al
                    prop.SetValue(envanterModel, null);
                }
            }
        }
    }
    public EnvanterModel? GetLatestRowById(string id)
    {
        EnvanterModel envanterModel = new EnvanterModel();
        try
        {
            string query = "SELECT TOP 1 ENVANTER_ID, ASSET, SERI_NO, COMP_MODEL, COMP_NAME, RAM, DISK_GB," +
        " MAC, PROC_MODEL, OS_NAME, OS_VER, USERNAME, DATE_CHANGED, ASSIGNED_USER, LAST_IP_ADDRESS," +
        " LOG_TEXT FROM ENVANTER WHERE ENVANTER_ID = @id ORDER BY DATE_CHANGED DESC";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            ModelFillerFromReader(envanterModel, reader);
                        return envanterModel;

                    }
                }

            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Hata: " + e.Message);
            return null;
        }

    }
    public T? GetCellById<T>(string column, string id)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = $"SELECT [{column}] FROM ENVANTER WHERE ENVANTER_ID = @id ORDER BY DATE_CHANGED DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        return default;
                    }
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Hata ({column}): " + e.Message);
            return default;
        }
    }
    public T? GetCellBySeriNo<T>(string column, string? seriNo)
    {
        try
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = $"SELECT [{column}] FROM ENVANTER WHERE SERI_NO = @seriNo ORDER BY DATE_CHANGED DESC";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add("@seriNo", SqlDbType.VarChar, 50).Value = seriNo;
            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception e)
        {
            System.Console.WriteLine($"Hata ({column}): " + e.Message);
            return default;
        }
    }
    public List<EnvanterModel>? GetSortedByDate(string tableName)
    {
        List<EnvanterModel>? envanterList;
        SqlCommand query = new SqlCommand($"SELECT * FROM [{tableName}] ORDER BY " +
        "DATE_CHANGED DESC", new SqlConnection(_connectionString));

        envanterList = ListFillerFromTable(query);
        return envanterList;
    }
    public List<EnvanterModel>? GetOrderedList(string modelColumnName, string method)
    {

        method = method?.ToUpper() == "DESC" ? "DESC" : "ASC";

        if (!LabelHelper.ModelLabelsToSqlLabels.TryGetValue(modelColumnName, out string? sqlColumn))
        {
            sqlColumn = "DATE_CHANGED";
        }

        string orderBySql;

        if (sqlColumn == "DATE_CHANGED")
        {
            orderBySql = $"DATE_CHANGED {method}";
        }
        else if (sqlColumn == "RAM" || sqlColumn == "DISK_GB")
        {
            orderBySql = $"{sqlColumn} {method}";
        }
        else
        {
            orderBySql = $"{sqlColumn} {method}";
        }

        string query = $@"
            WITH CTE AS (
                SELECT *, 
                    ROW_NUMBER() OVER (PARTITION BY ENVANTER_ID ORDER BY DATE_CHANGED DESC) as SiraNo
                FROM ENVANTER
            )
            SELECT * FROM CTE 
            WHERE SiraNo = 1 
            ORDER BY {orderBySql}";
        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(query, connection);

        return ListFillerFromTable(cmd);
    }
    public List<EnvanterModel>? ListFillerFromTable(SqlCommand cmd)
    {
        List<EnvanterModel>? envanterList = new List<EnvanterModel>();
        try
        {
            cmd.Connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    EnvanterModel envanterModel = new EnvanterModel();
                    ModelFillerFromReader(envanterModel, reader);
                    envanterList!.Add(envanterModel);
                }

            }
            cmd.Connection.Close();
        }
        catch (Exception)
        {
            return null;
        }
        return envanterList;
    }
    public List<EnvanterModel>? GetSearchedTable(
    string searchedColumn,
    string searchedValue1,
    string? searchedValue2)
    {

        using SqlConnection connection = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = connection;

        string whereSql;

        switch (searchedColumn)
        {
            case "RAM":
            case "DISK_GB":
                whereSql = $"{searchedColumn} BETWEEN @val1 AND @val2";

                cmd.Parameters.Add("@val1", SqlDbType.Decimal).Value =
                    decimal.Parse(searchedValue1, CultureInfo.InvariantCulture);

                cmd.Parameters.Add("@val2", SqlDbType.Decimal).Value =
                    decimal.Parse(searchedValue2!, CultureInfo.InvariantCulture);
                break;

            case "DATE_CHANGED":
                whereSql = @"DATE_CHANGED BETWEEN @date1 AND @date2";

                cmd.Parameters.Add("@date1", SqlDbType.DateTime2).Value =
                    DateTime.Parse(searchedValue1);

                cmd.Parameters.Add("@date2", SqlDbType.DateTime2).Value =
                    DateTime.Parse(searchedValue2!);
                break;

            case "ASSET":
            case "COMP_NAME":
            case "USERNAME":
            case "SERI_NO":
            case "MAC":
                whereSql = $"{searchedColumn} LIKE @text";

                cmd.Parameters.Add("@text", SqlDbType.NVarChar, 100)
                    .Value = $"%{searchedValue1}%";
                break;

            default:
                throw new ArgumentException("Geçersiz arama kolonu");
        }
        cmd.CommandText = $@"
            WITH CTE AS (
                SELECT *, 
                    ROW_NUMBER() OVER (PARTITION BY ENVANTER_ID ORDER BY DATE_CHANGED DESC) as SiraNo
                FROM ENVANTER
            )
            SELECT * FROM CTE 
            WHERE SiraNo = 1 AND {whereSql}";


        return ListFillerFromTable(cmd);
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
        envanterModel.OsName = envanterList?[0].OsName;
        envanterModel.OsVer = envanterList?[0].OsVer;
        envanterModel.Username = envanterList?[0].Username;
        envanterModel.DateChanged = envanterList?[0].DateChanged;
        envanterModel.AssignedUser = envanterList?[0].AssignedUser;
        envanterModel.LastIpAddress = envanterList?[0].LastIpAddress;
        envanterModel.Log = envanterList?[0].Log;

        return envanterModel;

    }
    public string IdDeterminer(string? seriNo)
    {
        int result;
        string id;

        string counter = "SELECT COUNT(*) FROM ENVANTER WHERE SERI_NO = @seriNo";
        string getId = "SELECT ENVANTER_ID FROM ENVANTER WHERE SERI_NO = @seriNo";
        string getMaxId = "SELECT MAX(CAST(ENVANTER_ID AS INT)) FROM ENVANTER";

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

                    int existingId = (idResult != DBNull.Value && idResult != null)
                        ? Convert.ToInt32(idResult)
                        : 1;

                    id = existingId.ToString("D8");
                }
                else
                {
                    object maxIdResult = getMaxIdCmd.ExecuteScalar();

                    int maxId = (maxIdResult != DBNull.Value && maxIdResult != null)
                        ? Convert.ToInt32(maxIdResult) + 1
                        : 1;

                    id = maxId.ToString("D8");
                }
            }
        }

        return id;
    }
    public void DiskAdder(EnvanterModel envanterModel, SqlConnection conn)
    {
        if (envanterModel.Drives == null || envanterModel.Drives.Count == 0)
            return;

        using var tran = conn.BeginTransaction();

        try
        {
            using (SqlCommand deleteCmd = new SqlCommand(
                "DELETE FROM DISKS WHERE ENVANTER_ID = @envanterId",
                conn, tran))
            {
                deleteCmd.Parameters.AddWithValue("@envanterId", envanterModel.Id);
                deleteCmd.ExecuteNonQuery();
            }

            string insertSql = @"
            INSERT INTO DISKS 
            (ENVANTER_ID, NAME, TOTAL_SIZE_GB, TOTAL_FREE_SPACE_GB)
            VALUES 
            (@envanterId, @name, @totalSize, @freeSpace)";

            foreach (var disk in envanterModel.Drives)
            {
                using SqlCommand insertCmd = new SqlCommand(insertSql, conn, tran);

                insertCmd.Parameters.AddWithValue("@envanterId", envanterModel.Id);
                insertCmd.Parameters.AddWithValue("@name", (object?)disk.Name ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@totalSize", (object?)disk.TotalSizeGB ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@freeSpace", (object?)disk.TotalFreeSpaceGB ?? DBNull.Value);

                insertCmd.ExecuteNonQuery();
            }

            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }
    public List<DriveInfoModel>? GetDiskListById(string id)
    {
        string query = $"SELECT NAME, TOTAL_SIZE_GB, TOTAL_FREE_SPACE_GB FROM DISKS WHERE ENVANTER_ID = @id";
        List<DriveInfoModel>? diskList = new List<DriveInfoModel>();
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diskList?.Add(new DriveInfoModel
                            {
                                Name = reader.IsDBNull(0) ? "Bilinmiyor" : reader.GetString(0),
                                TotalSizeGB = reader.IsDBNull(1) ? 0.00 : reader.GetDouble(1),
                                TotalFreeSpaceGB = reader.IsDBNull(2) ? 0.00 : reader.GetDouble(2)
                            });
                        }
                    }
                }

            }
            return diskList;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Diskleri alırken hata oluştu: " + e.Message);
            return null;
        }
    }
    public EnvanterModel GetEnvanterModelById(string id)
    {
        EnvanterModel envanterModel = new EnvanterModel();
        try
        {
            string query = $"SELECT * FROM ENVANTER WHERE ENVANTER_ID = @id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            ModelFillerFromReader(envanterModel, reader);

                    }
                }
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Envanter modeli alınırken hata oluştu: " + e.Message);
        }
        return envanterModel;
    }
    private static void ModelFillerFromReader(EnvanterModel envanterModel, SqlDataReader reader)
    {
        foreach (var label in LabelHelper.ModelLabelsToSqlLabels)
        {
            string propertyName = label.Key;
            string columnName = label.Value;

            int ordinal = reader.GetOrdinal(columnName);
            var prop = typeof(EnvanterModel).GetProperty(propertyName);

            if (prop == null || !prop.CanWrite)
                continue;

            if (reader.IsDBNull(ordinal))
            {
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(envanterModel, "Bilinmiyor");

                else if (prop.PropertyType == typeof(decimal?))
                    prop.SetValue(envanterModel, null);

                else if (prop.PropertyType == typeof(DateTime?))
                    prop.SetValue(envanterModel, null);
            }
            else
            {
                object value = reader.GetValue(ordinal);
                prop.SetValue(envanterModel, value);
            }
        }
    }
    public List<EnvanterModel> GetEnvanterHistoryById(string id)
    {
        List<EnvanterModel> historyList = new List<EnvanterModel>();
        string query = $"SELECT * FROM ENVANTER WHERE ENVANTER_ID = @id ORDER BY DATE_CHANGED DESC";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50).Value = id;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EnvanterModel envanterModel = new EnvanterModel();
                        ModelFillerFromReader(envanterModel, reader);
                        historyList.Add(envanterModel);
                    }
                }
            }
        }
        return historyList;
    }
}