namespace EnvanterYonetimPaneli.Helpers
{

    public static class LabelHelper
    {
        public static Dictionary<string, string> Labels => new Dictionary<string, string>
        {
            { "Id", "Id"},
            { "Asset", "Asset"},
            { "SeriNo", "Seri No"},
            { "CompModel", "Bilgisayar Modeli"},
            { "CompName", "Bilgisayar Adı"},
            { "RAM", "RAM"},
            { "DiskGB", "Disk Boyutu"},
            { "MAC", "MAC Adresi"},
            { "ProcModel", "İşlemci Modeli"},
            { "OsName", "İşletim Sistemi"},
            { "OsVer", "İşletim Sistemi Versiyonu"},
            { "Username", "Kullanıcı"},
            { "AssignedUser", "Zimmetli Kişi"},
            { "LastIpAddress", "Son Ip Adresi"},
            { "DateChanged", "Değişiklik Tarihi"},
            { "Log", "Log"}

        };
        public static Dictionary<string, string> SqlLabels => new Dictionary<string, string>
        {
            { "ID", "Id"},
            { "ASSET", "Asset"},
            { "SERI_NO", "Seri No"},
            { "COMP_MODEL", "Bilgisayar Modeli"},
            { "COMP_NAME", "Bilgisayar Adı"},
            { "RAM", "RAM"},
            { "DISK_GB", "Disk Boyutu"},
            { "MAC", "MAC Adresi"},
            { "PROC_MODEL", "İşlemci Modeli"},
            { "OS_NAME", "İşletim Sistemi"},
            { "OS_VER", "İşletim Sistemi Versiyonu"},
            { "USERNAME", "Kullanıcı"},
            { "ASSIGNED_USER", "Zimmetli Kişi"},
            { "LAST_IP_ADDRESS", "Son Ip Adresi"},
            { "DATE_CHANGED", "Değişiklik Tarihi"},
            { "LOG", "Log"}


        };
        public static Dictionary<string, string> LabelsToSqlLabels => new Dictionary<string, string>
        {
            { "Id", "ID"},
            { "Asset", "ASSET"},
            { "SeriNo", "SERI_NO"},
            { "CompModel", "COMP_MODEL"},
            { "CompName", "COMP_NAME"},
            { "RAM", "RAM"},
            { "DiskGB", "DISK_GB"},
            { "MAC", "MAC"},
            { "ProcModel", "PROC_MODEL"},
            { "OsName", "OS_NAME"},
            { "OsVer", "OS_VER"},
            { "Username", "USERNAME"},
            { "AssignedUser", "ASSIGNED_USER"},
            { "LastIpAddress", "LAST_IP_ADDRESS"},
            { "DateChanged", "DATE_CHANGED"},
            { "Log", "LOG"}


        };
        public static Dictionary<string, string> KomutLabels => new Dictionary<string, string>
        {
            { "Id", "Id"},
            { "CompName", "Bilgisayar Adı"},
            { "Command", "Komut"},
            { "Response", "Çıktı"},
            { "User", "Gönderen"},
            { "DateSent", "Gönderim Tarihi"},
            { "DateApplied", "Uygulanma Tarihi"},
            { "IsApplied", "Tamamlanma Durumu"},
        };
        public static Dictionary<string, string> KomutSqlLabels => new Dictionary<string, string>
        {
            { "ID", "Id"},
            { "COMP_NAME", "Bilgisayar Adı"},
            { "COMMAND", "Komut"},
            { "RESPONSE", "Çıktı"},
            { "USER", "Gönderen"},
            { "DATE_SENT", "Gönderim Tarihi"},
            { "DATE_APPLIED", "Uygulanma Tarihi"},
            { "IS_APPLIED", "Tamamlanma Durumu"},
        };

    }

}