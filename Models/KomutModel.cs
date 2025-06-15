using System.Text.Json;

namespace EnvanterYonetimPaneli.Models
{
    public class KomutModel
    {
        public string? Id { get; set; }
        public string? CompName { get; set; }
        public string? Command { get; set; }
        public string? Response { get; set; }
        public string? User { get; set; }
        public string? DateSent { get; set; }
        public string? DateApplied { get; set; }
        public string? IsApplied { get; set; }
        public string ToJson()
        {
            var obj = new
            {
                Id,
                CompName,
                Command,
                Response,
                User,
                DateSent,
                DateApplied,
                IsApplied
            };

            return JsonSerializer.Serialize(obj);
        }

    }
}
