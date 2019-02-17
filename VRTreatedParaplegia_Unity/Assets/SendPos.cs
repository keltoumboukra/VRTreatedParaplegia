using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SendPos : MonoBehaviour {


    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion

    public class Coord
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
    }

    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage();
        }
    }
    /// Setup socket connection. 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On yayayay client connect exception " + e);
        }
    }
    /// Runs in background clientReceiveThread; Listens for incomming data. 	   
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("169.254.8.2", 4242);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 		
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket lolololol exception: " + socketException);
        }
    }
    /// Send message to server using socket connection. 	
    private void SendMessage()
    {
        float SRX = GameObject.Find("Sphere Right").transform.position.x;
        float SRY = GameObject.Find("Sphere Right").transform.position.y;
        float SRZ = GameObject.Find("Sphere Right").transform.position.z;

        float SLX = GameObject.Find("Sphere Left").transform.position.x;
        float SLY = GameObject.Find("Sphere Left").transform.position.y;
        float SLZ = GameObject.Find("Sphere Left").transform.position.z;

        //var pts = new Double[] { -0.5, 0.5, 0.5, 1, 0, 0, 0 };
        string clientMessage = "";
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                //clientMessage += "left " + pts[0] + ' ' + pts[1] + ' ' + pts[3] + ' ' + pts[4] + ' ' + pts[5] + ' ' + pts[6] + ' ' + pts[7] + '/';
                clientMessage += "left " + SLX + ' ' + SLY + ' ' + SLZ + '/';
            }
            // Convert string message to byte array.                 
            byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
            // Write byte array to socketConnection stream.                 
            stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            Debug.Log("le client unity a envoyé");
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket cacaccaca exception: " + socketException);
        }
    }
}