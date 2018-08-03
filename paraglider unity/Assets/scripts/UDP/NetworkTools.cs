using System.Net;
using UnityEngine;

public class NetworkTools {
    public static IPEndPoint[] GetIPEndPointFromHostName(string hostName, int port)
    {
        var addresses = System.Net.Dns.GetHostAddresses(hostName);
        if (addresses.Length == 0)
        {
            Debug.LogError("Unable to retrieve address from specified host name. " + hostName);

        }
        else if (addresses.Length > 1)
        {
            Debug.LogWarning("There is more that one IP address to the specified host. " + hostName);

        }
        IPEndPoint[] endPoints = new IPEndPoint[addresses.Length];
        for (int i = 0; i < endPoints.Length; i++)
            endPoints[i] = new IPEndPoint(addresses[i], port);
        return endPoints; // Port gets validated here.
    }
}
