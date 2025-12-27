namespace EnvanterYonetimPaneli.Models
{
    public class EnvanterModel
    {
        public string? Id { get; set; }
        public string? Asset { get; set; }
        public string? SeriNo { get; set; }
        public string? CompModel { get; set; }
        public string? CompName { get; set; }
        public double? RAM { get; set; }
        public double? DiskGB { get; set; }
        public string? MAC { get; set; }
        public string? ProcModel { get; set; }
        public string? OsName { get; set; }
        public string? OsVer { get; set; }

        public string? Username { get; set; }
        public DateTime? DateChanged { get; set; }
        public string? AssignedUser { get; set; }
        public string? LastIpAddress { get; set; }
        public string? Log { get; set; }
        public List<DriveInfoModel>? Drives { get; set; }

    }


}
