using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketServer
{
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private Socket clientSocket = null;
    private string host;
    private int port;

    public SocketServer(string host, int port)
    {
        this.host = host;
        this.port = port;
        IPEndPoint point = new IPEndPoint(IPAddress.Parse(host), port);
        socket.Bind(point);
        socket.Listen(100);
        clientSocket = socket.Accept();
    }

    public void SendStandardCommand(List<string> attr)
    {
        string command = String.Format("f:{0}#argv:", attr[0]);
        attr.RemoveAt(0);
        for (int i = 0; i < attr.Count; i++)
        {
            command += String.Format("{0},", attr[i]);
        }
        clientSocket.Send(Encoding.UTF8.GetBytes(command));
    }

    public void SendCommand(string cmd)
    {
        clientSocket.Send(Encoding.UTF8.GetBytes(cmd));
    }

    public string Receive()
    {
        byte[] buffer = new byte[1024];
        string value = "";
        clientSocket.Receive(buffer);
        int lenght = (int)Math.Floor((double)Int64.Parse(Encoding.UTF8.GetString(buffer))/1024.0);
        for(int i = 0;i < lenght; i++)
        {
            clientSocket.Receive(buffer);
            value += Encoding.UTF8.GetString(buffer) ;
        }
        buffer = new byte[1024];
        clientSocket.Receive(buffer);
        string index = "";
        for (int i = 0; i < buffer.Length; i++)
        {
            index += Convert.ToChar(buffer[i]);
        }
        Debug.Log(index);
        Debug.Log(buffer.ToString());
        string tmp = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        value += tmp;
        Debug.Log(value.Length);
        return value;
    }

    public void Destroy()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        socket.Close();
    }

}