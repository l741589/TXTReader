using System;
namespace TXTReader.Utility {
    public delegate void HttpDelegateEvent(Http http);
    public interface IHttpDelegate {
        HttpDelegateEvent BeforeRequest { get; }
        HttpDelegateEvent AfterRequest { get; }
        HttpDelegateEvent BeforeResponse { get; }
        HttpDelegateEvent AfterResponse { get; }        
    }
}
