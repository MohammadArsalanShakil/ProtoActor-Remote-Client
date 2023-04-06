using System;
using System.Net.Sockets;
using System.Threading;
namespace ProtoActor_Remote_Client.Utils;
using TcpClient = NetCoreServer.TcpClient;
public class TcpProtoClient : TcpClient
{
    public TcpProtoClient(string address, int port) : base(address, port) { }

    public bool ConnectAndStart()
    {
        // System.Diagnostics.Debug.WriteLineLine($"TCP protocol client starting a new session with Id '{Id}'...");

        StartReconnectTimer();
        return ConnectAsync();
    }

    public bool DisconnectAndStop()
    {
        //System.Diagnostics.Debug.WriteLineLine($"TCP protocol client stopping the session with Id '{Id}'...");

        StopReconnectTimer();
        DisconnectAsync();
        return true;
    }

    public override bool Reconnect()
    {
        return ReconnectAsync();
    }

    private Timer _reconnectTimer;
   
    public void StartReconnectTimer()
    {
        
        //Start the reconnect timer
        _reconnectTimer = new Timer(state =>
        {
            //System.Diagnostics.Debug.WriteLineLine($"TCP reconnect timer connecting the client session with Id '{Id}'...");
            ConnectAsync();
        }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    }

  

    public void StopReconnectTimer()
    {
        // Stop the reconnect timer
        _reconnectTimer?.Dispose();
        _reconnectTimer = null;
    }

    public void send(byte[] buffer)
    {
        this.SendAsync(buffer);
    }

    public delegate void ConnectedHandler();
    public event ConnectedHandler Connected = () => { };

    protected override void OnConnected()
    {
        //System.Diagnostics.Debug.WriteLineLine($"TCP protocol client connected a new session with Id '{Id}' to remote address '{Address}' and port {Port}");

        Connected?.Invoke();
    }

    public delegate void DisconnectedHandler();
    public event DisconnectedHandler Disconnected = () => { };

    protected override void OnDisconnected()
    {
        // System.Diagnostics.Debug.WriteLineLine($"TCP protocol client disconnected the session with Id '{Id}'");

        // Setup and asynchronously wait for the reconnect timer
        _reconnectTimer?.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

        Disconnected?.Invoke();
    }

    public delegate void ReceivedHandler(byte[] buffer, long offset, long size);
    public event ReceivedHandler Received = (buffer, offset, size) =>
    {

    };

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        Received?.Invoke(buffer, offset, size);
    }

    public delegate void ErrorHandler(SocketError error);
    public event ErrorHandler Error = (error) =>
    {
        
    };

    protected override void OnError(SocketError error)
    {
        //System.Diagnostics.Debug.WriteLineLine($"TCP protocol client caught a socket error: {error}");

        Error?.Invoke(error);
    }

    #region IDisposable implementation

    // Disposed flag.
    private bool _disposed;

    protected override void Dispose(bool disposingManagedResources)
    {
        if (!_disposed)
        {
            if (disposingManagedResources)
            {
                // Dispose managed resources here...
                StopReconnectTimer();
            }

            // Dispose unmanaged resources here...

            // Set large fields to null here...

            // Mark as disposed.
            _disposed = true;
        }

        // Call Dispose in the base class.
        base.Dispose(disposingManagedResources);
    }

    // The derived class does not have a Finalize method
    // or a Dispose method without parameters because it inherits
    // them from the base class.

    #endregion
}