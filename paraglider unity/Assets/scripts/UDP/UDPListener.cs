using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class UDPListener
{
    private UdpClient listener;
    private int port;
    private System.Threading.Thread thread;
    private IPEndPoint groupEP;
    public string encoding = "ascii";

    public event EventHandler<string> MessageReceived;


    public bool Start(int port)
    {
        // Create UDP client on port address
        this.port = port;
        listener = new UdpClient(port);
        groupEP = new IPEndPoint(IPAddress.Any, port);

        // Start background thread
        thread = new System.Threading.Thread(_ThreadRun);
        thread.Priority = System.Threading.ThreadPriority.Normal;
        thread.Start();

        return true;
    }

    protected void _ThreadRun()
    {
        string data = "";
        byte[] bytes;

        while (true)
        {
            bytes = listener.Receive(ref groupEP);
            if (encoding == "ascii") 
                data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            else if(encoding == "utf8")
                data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            OnMessageReceived(data);
        }

    }

    protected virtual void OnMessageReceived(string txt)
    {
        EventHandler<string> handler = MessageReceived;
        if (handler != null)
        {
            handler(this, txt);
        }
    }

    public void SetEncoding(string code)
    {
        encoding = code;
    }

    public void Close()
    {
        if (thread != null && thread.IsAlive)
            thread.Abort();

        if (listener != null)
            listener.Close();

        listener = null;
    }
}
