using PracticeWebApp.Models;

public class OrderHistoryDTO
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public Dictionary<string, int> BookInfo { get; set; }
}
