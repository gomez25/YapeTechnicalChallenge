namespace TransactionService.Domain.Shared
{
  public class Response<T>
  {
    public bool IsSuccess { get; set; } = false;
    public required T Data { get; set; }
    public string Message { get; set; } = string.Empty; 
    public int StatusCode { get; set; } = 500; 
  }
}
