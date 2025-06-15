namespace EnvanterApiProjesi.Helpers
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
            { "Username", "Kullanıcı"},
            { "AssignedUser", "Zimmetli Kişi"},
            { "LastIpAddress", "Son Ip Adresi"},
            { "DateChanged", "Değişiklik Tarihi"}
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
            { "USERNAME", "Kullanıcı"},
            { "ASSIGNED_USER", "Zimmetli Kişi"},
            { "LAST_IP_ADDRESS", "Son Ip Adresi"},
            { "DATE_CHANGED", "Değişiklik Tarihi"}
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