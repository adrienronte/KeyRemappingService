namespace KeyRemappingService.Models
{
    public class KeyMapping
    {
        public int Id { get; set; }

        public int SourceKeyCode { get; set; }

        public int TargetKeyCode { get; set; }

        public int KeyboardId { get; set; }

        public Keyboard Keyboard { get; set; } = null!;
    }
}
