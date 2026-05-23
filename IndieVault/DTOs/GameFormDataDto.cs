namespace IndieVault.DTOs
{
    public class GameFormDataDto
    {
        public List<LookupDto> Genres { get; set; } = new List<LookupDto>();
        public List<LookupDto> Engines { get; set; } = new List<LookupDto>();
        public List<LookupDto> Platforms { get; set; } = new List<LookupDto>();
        public List<LookupDto> Tags { get; set; } = new List<LookupDto>();
    }
}
