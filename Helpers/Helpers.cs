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
    }
}