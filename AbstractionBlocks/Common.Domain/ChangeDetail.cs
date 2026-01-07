namespace AbstractionBlocks.Common.Domain
{
    public class ChangeDetail
    {
        public required string Field { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
