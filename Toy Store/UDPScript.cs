﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPScript : MonoBehaviour
{
    public string result = "init";

    // UDP Code

    [HideInInspector] public bool isTxStarted = false;

    //[SerializeField] string IP = "127.0.0.1"; // local host
    [SerializeField] int rxPort = 8500; // port to receive data from Python on
    //[SerializeField] int txPort = 8001; // port to send data to Python on

    //int i = 0; // DELETE THIS: Added to show sending data from Unity to Python via UDP

    // Create necessary UdpClient objects
    UdpClient client;
    IPEndPoint remoteEndPoint;
    Thread receiveThread; // Receiving Thread

    DateTime pickTime;

    //IEnumerator SendDataCoroutine() // DELETE THIS: Added to show sending data from Unity to Python via UDP
    //{
    //    while (true)
    //    {
    //        SendData("Sent from Unity: " + i.ToString());
    //        i++;
    //        yield return new WaitForSeconds(1f);
    //    }
    //}

    public void Start()
    {
        init();
    }

    public void SendData(string message) // Use to send data to Python
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

   



    void init()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // Receive data, update packets received
    private void ReceiveData()
    {
        client = new UdpClient(rxPort);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                print("Data Received >> " + text);
                result = text;
                if (result.Equals("pick"))
                {
                    pickTime = DateTime.Now;

                }
                ProcessInput(text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private void ProcessInput(string input)
    {
        // PROCESS INPUT RECEIVED STRING HERE

        if (!isTxStarted) // First data arrived so tx started
        {
            isTxStarted = true;
        }
    }

    //Prevent crashes - close clients and threads properly!
    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }

    public string getText()
    {
        return result;
    }

    public DateTime getPickTime()
    {
        return pickTime;
    }

}