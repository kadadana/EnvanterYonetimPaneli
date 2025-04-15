namespace EnvanterApiProjesi.Models
{
    public class EnvanterModel
    {
        public string? Id { get; set; }
        public string? Asset { get; set; }
        public string? SeriNo { get; set; }
        public string? CompModel { get; set; }
        public string? CompName { get; set; }
        public string? RAM { get; set; }
        public string? DiskGB { get; set; }
        public string? MAC { get; set; }
        public string? ProcModel { get; set; }
        public string? Username { get; set; }
        public string? DateChanged { get; set; }
        public string? AssignedUser { get; set; }
        public string? LastIpAddress { get; set; }
        public List<DriveInfoModel>? Drives { get; set; }

    }

    public class DriveInfoModel
    {
        public string? Name { get; set; }
        public string? TotalSizeGB { get; set; }
        public string? TotalFreeSpace { get; set; }
    }
}
