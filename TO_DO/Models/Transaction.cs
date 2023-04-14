namespace TO_DO.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Data { get; set; }
    public TransactionStatus Status { get; set; }
}
