namespace KeyRemappingService.Models
{
    public class Keyboard
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<KeyMapping> Mappings { get; set; } = new();
    }
}