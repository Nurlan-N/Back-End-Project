namespace Back_End_Project.Models
{
    public class Order:BaseEntity
    {
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
        public int No { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
