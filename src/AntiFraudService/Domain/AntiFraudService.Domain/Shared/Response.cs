namespace AntiFraudService.Domain.Shared
{
  public class Response<T> where T : class
  {
    public bool IsSuccess { get; set; } = false;
    public T Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 500;
  }
}
