using System.Net;

namespace Domain.Services.Implementations;

public class RequestStore {
    public HttpRequest? Request { get; set; }

    public IPAddress? IpAddress { get; set; }
}