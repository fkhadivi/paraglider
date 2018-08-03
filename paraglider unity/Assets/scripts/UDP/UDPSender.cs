using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class UDPSender
{

    public static bool SendUDPStringASCII(string ip, int port, string send)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Encoding.ASCII.GetBytes(send);
            IPEndPoint to = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.SendTo(data, to);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }
    public static bool SendUDPByte(string ip, int port, System.Object data)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint to = new IPEndPoint(IPAddress.Parse(ip), port);

            if (data is BinaryData)
            {
                socket.SendTo(((BinaryData)data).data, to);
            }
            else if (data is byte[])
            {
                socket.SendTo((byte[])data, to);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }
    public static bool SendUDPStringASCIIHostname(string hostname, int port, string send)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Encoding.ASCII.GetBytes(send);
            IPEndPoint[] endPoints = NetworkTools.GetIPEndPointFromHostName(hostname, port);
            for (int i = 0; i < endPoints.Length; i++)
                socket.SendTo(data, endPoints[i]);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }

    public static bool BroadcastUdpASCII(int port, string message, string subnet)
    {
        if (subnet == null) { subnet = "255.255.255.255"; }
        try
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(subnet), port);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }

    public static bool SendUDPStringUTF8(string ip, int port, string send)
    {
        try
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Encoding.UTF8.GetBytes(send);
            IPEndPoint to = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.SendTo(data, to);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }

    public static bool BroadcastUdpUTF8(int port, string message, string subnet)
    {
        if (subnet == null) { subnet = "255.255.255.255"; }
        try
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(subnet), port);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }

        return true;
    }
}


