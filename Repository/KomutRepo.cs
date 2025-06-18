using EnvanterYonetimPaneli.Models;
using Microsoft.Data.SqlClient;

namespace EnvanterYonetimPaneli;

public class KomutRepo
{
    private readonly string? _connectionString;

    public KomutRepo(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        KomutTableCreator();

    }

    public void KomutTableCreator()
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string creator = "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KOMUT_TABLE') " +
                                "BEGIN " +
                                "CREATE TABLE [KOMUT_TABLE] " +
                                "(ID NVARCHAR(100) UNIQUE, " +
                                "COMP_NAME NVARCHAR(max), " +
                                "COMMAND NVARCHAR(max), " +
                                "RESPONSE NVARCHAR(max), " +
                                "[USER] NVARCHAR(max), " +
                                "DATE_SENT NVARCHAR(max), " +
                                "DATE_APPLIED NVARCHAR(max), " +
                                "IS_APPLIED NVARCHAR(Max)); " +
                                "END;";
            using (SqlCommand creatorCmd = new SqlCommand(creator, conn))
            {
                creatorCmd.ExecuteNonQuery();
            }
        }
    }

    public string AddToSql(KomutModel komutModel, bool isUpdate)
    {
        KomutTableCreator();

        komutModel.CompName = string.IsNullOrWhiteSpace(komutModel.CompName) ? "Bilinmiyor" : komutModel.CompName;
        komutModel.Command = string.IsNullOrWhiteSpace(komutModel.Command) ? "Bilinmiyor" : komutModel.Command;
        komutModel.Response = string.IsNullOrWhiteSpace(komutModel.Response) ? "Bilinmiyor" : komutModel.Response;
        komutModel.User = string.IsNullOrWhiteSpace(komutModel.User) ? "Bilinmiyor" : komutModel.User;
        komutModel.DateSent = string.IsNullOrWhiteSpace(komutModel.DateSent) ? "Bilinmiyor" : komutModel.DateSent;
        komutModel.DateApplied = string.IsNullOrWhiteSpace(komutModel.DateApplied) ? "Bilinmiyor" : komutModel.DateApplied;
        komutModel.IsApplied = string.IsNullOrWhiteSpace(komutModel.IsApplied) ? "Bilinmiyor" : komutModel.IsApplied;

        if (!isUpdate)
        {
            komutModel.Id = IdDeterminer();


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string inserter = "INSERT INTO KOMUT_TABLE VALUES " +
                                "(@id, @compName, @command, @response, @user, @dateSent, @dateApplied, @isApplied)";

                    using (SqlCommand inserterCmd = new SqlCommand(inserter, conn))
                    {
                        inserterCmd.Parameters.AddWithValue("@id", komutModel.Id);
                        inserterCmd.Parameters.AddWithValue("@compName", komutModel.CompName);
                        inserterCmd.Parameters.AddWithValue("@command", komutModel.Command);
                        inserterCmd.Parameters.AddWithValue("@response", komutModel.Response);
                        inserterCmd.Parameters.AddWithValue("@user", komutModel.User);
                        inserterCmd.Parameters.AddWithValue("@dateSent", komutModel.DateSent);
                        inserterCmd.Parameters.AddWithValue("@dateApplied", komutModel.DateApplied);
                        inserterCmd.Parameters.AddWithValue("@isApplied", komutModel.IsApplied);

                        inserterCmd.ExecuteNonQuery();

                    }
                    return "Komut veritabanina eklenmistir.";

                }
                catch (Exception ex)
                {
                    return "Komut veritabanina eklenirken bir sorunla karsilasildi." + ex;

                }

            }
        }
        else
        {
            komutModel.Id = string.IsNullOrWhiteSpace(komutModel.Id) ? "Bilinmiyor" : komutModel.Id;
            komutModel.IsApplied = "TRUE";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string updater = "UPDATE KOMUT_TABLE " +
                    "SET " +
                    "ID = @id, " +
                    "COMP_NAME = @compName, " +
                    "COMMAND = @command, " +
                    "RESPONSE = @response, " +
                    "[USER] = @user, " +
                    "DATE_SENT = @dateSent, " +
                    "DATE_APPLIED = @dateApplied, " +
                    "IS_APPLIED = @isApplied " +
                    "WHERE ID = @id";

                    using (SqlCommand updaterCmd = new SqlCommand(updater, conn))
                    {
                        updaterCmd.Parameters.AddWithValue("@id", komutModel.Id);
                        updaterCmd.Parameters.AddWithValue("@compName", komutModel.CompName);
                        updaterCmd.Parameters.AddWithValue("@command", komutModel.Command);
                        updaterCmd.Parameters.AddWithValue("@response", komutModel.Response);
                        updaterCmd.Parameters.AddWithValue("@user", komutModel.User);
                        updaterCmd.Parameters.AddWithValue("@dateSent", komutModel.DateSent);
                        updaterCmd.Parameters.AddWithValue("@dateApplied", komutModel.DateApplied);
                        updaterCmd.Parameters.AddWithValue("@isApplied", komutModel.IsApplied);

                        updaterCmd.ExecuteNonQuery();

                    }
                    return "Komut veritabaninda guncellenmistir.";

                }
                catch (Exception ex)
                {
                    return "Komut veritabaninda guncellenirken bir sorunla karsilasildi." + ex;

                }
            }

        }
    }

    public KomutModel GetUnappliedCommandsByCompName(string compName)
    {
        KomutModel komutModel = new KomutModel();
        string query = "SELECT TOP 1 * FROM KOMUT_TABLE WHERE IS_APPLIED != 'TRUE' AND COMP_NAME = @compName";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@compName", compName);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        komutModel = new KomutModel
                        {
                            Id = reader.IsDBNull(0) ? null : reader.GetString(0),
                            CompName = reader.IsDBNull(1) ? null: reader.GetString(1),
                            Command = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Response = reader.IsDBNull(3) ? null : reader.GetString(3),
                            User = reader.IsDBNull(4) ? null : reader.GetString(4),
                            DateSent = reader.IsDBNull(5) ? null : reader.GetString(5),
                            DateApplied = reader.IsDBNull(6) ? null : reader.GetString(6),
                            IsApplied = reader.IsDBNull(7) ? null : reader.GetString(7)

                        };
                        return komutModel;
                    }

                }
            }
        }
        return komutModel;

    }
    public string IdDeterminer()
    {
        int maxId;
        string getMaxId = "SELECT MAX(CAST(Id AS INT)) FROM KOMUT_TABLE";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand getMaxIdCmd = new SqlCommand(getMaxId, conn))
            {
                object maxIdResult = getMaxIdCmd.ExecuteScalar();

                maxId = (maxIdResult != DBNull.Value && maxIdResult != null) ? Convert.ToInt32(maxIdResult) + 1 : 1;
                System.Console.WriteLine(maxId);
                System.Console.WriteLine(maxIdResult);
                return maxId.ToString();
            }
        }
    }
    public List<KomutModel>? GetOrderedList(string tableName, string columnName, string method)
    {
        string query;
        List<KomutModel>? komutList;
        switch (columnName)
        {
            case "DateSent":
                query = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DATE_SENT, 104) {method}";
                komutList = ListFillerByTable(query);
                break;
            case "DateApplied":
                query = $"SELECT * FROM [{tableName}] ORDER BY TRY_CONVERT(DATETIME, DATE_APPLIED, 104) {method}";
                komutList = ListFillerByTable(query);
                break;
            case "Id":
                query = $"SELECT * FROM [{tableName}] ORDER BY CAST({columnName} AS INT) {method}";
                komutList = ListFillerByTable(query);
                break;
            default:
                query = $"SELECT * FROM [{tableName}] ORDER BY {columnName} {method}";
                komutList = ListFillerByTable(query);
                break;
        }
        return komutList;
    }
    public List<KomutModel>? ListFillerByTable(string command)
    {
        List<KomutModel>? komutList = new List<KomutModel>();
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
                        komutList?.Add(new KomutModel
                        {
                            Id = reader.IsDBNull(0) ? "Bilinmiyor" : reader.GetString(0),
                            CompName = reader.IsDBNull(1) ? "Bilinmiyor" : reader.GetString(1),
                            Command = reader.IsDBNull(2) ? "Bilinmiyor" : reader.GetString(2),
                            Response = reader.IsDBNull(3) ? "Bilinmiyor" : reader.GetString(3),
                            User = reader.IsDBNull(4) ? "Bilinmiyor" : reader.GetString(4),
                            DateSent = reader.IsDBNull(5) ? "Bilinmiyor" : reader.GetString(5),
                            DateApplied = reader.IsDBNull(6) ? "Bilinmiyor" : reader.GetString(6),
                            IsApplied = reader.IsDBNull(7) ? "Bilinmiyor" : reader.GetString(7)

                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata oluştu: " + ex.Message);
            Console.WriteLine("Stack Trace: " + ex.StackTrace);
            return null;
        }
        return komutList;
    }
    public List<KomutModel>? GetSearchedTable(string tableName, string searchedColumn, string searchedValue1, string? searchedValue2)
    {
        List<KomutModel>? komutList;
        string query;
        if (searchedColumn == "DATE_SENT")
        {
            query = $"SELECT * FROM [{tableName}] WHERE TRY_CONVERT(datetime, DATE_SENT, 104) BETWEEN '{searchedValue1}' AND '{searchedValue2}'";
            komutList = ListFillerByTable(query);
            return komutList;
        }
        else if (searchedColumn == "DATE_APPLIED")
        {
            query = $"SELECT * FROM [{tableName}] WHERE TRY_CONVERT(datetime, DATE_APPLIED, 104) BETWEEN '{searchedValue1}' AND '{searchedValue2}'";
            komutList = ListFillerByTable(query);
            return komutList;
        }
        else
        {
            query = $"SELECT * FROM [{tableName}] WHERE {searchedColumn} LIKE '%{searchedValue1}%'";
            komutList = ListFillerByTable(query);
            return komutList;
        }

    }
}