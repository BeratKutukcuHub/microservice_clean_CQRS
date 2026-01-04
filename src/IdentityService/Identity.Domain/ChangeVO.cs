namespace IdentityService.Identity.Domain
{
    public class ChangeDetail
    {
        public string Field { get; set; }       
        public string? OldValue { get; set; }   
        public string? NewValue { get; set; }   
    }
}