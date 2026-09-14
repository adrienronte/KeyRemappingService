namespace KeyRemappingService.Models
{
    public class MappingRequest
    {
        public List<MappingItem> Mappings { get; set; } = new();
    }

    public class MappingItem
    {
        public int From { get; set; }

        public int To { get; set; }
    }
}