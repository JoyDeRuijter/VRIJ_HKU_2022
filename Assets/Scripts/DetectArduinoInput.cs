using System.Collections;
using System.IO.Ports;
using System.ServiceProcess;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.Win32;

public class DetectArduinoInput : MonoBehaviour
{
    private GameManager gameManager;

    string port;
    [SerializeField] int baudRate;

    private Thread thread;

    private Queue outputQueue;    // From Unity to Arduino
    private Queue inputQueue;    // From Arduino to Unity

    SerialPort stream;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Start()
    {
        port = AutodetectArduinoPort();
        StartThread();
    }

    private void Update()
    {
        stream.DiscardOutBuffer();
        stream.DiscardInBuffer();
        string receivedString = ReadFromArduino();
        if (receivedString != null)
        {
            if (int.TryParse(receivedString, out int pipe))
            {
                gameManager.ReceiveInput(pipe);
            }
        }
    }

    private void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());

        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    private void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    public string ReadFromArduino()
    {
        if (inputQueue.Count == 0)
            return null;
        return (string)inputQueue.Dequeue();
    }

    private string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            Debug.LogException(e, this);
            return null;
        }
    }

    private void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    private void ThreadLoop()
    {
        // Opens the connection on the serial port
        stream = new SerialPort(port, baudRate);
        stream.ReadTimeout = 10000;
        stream.Open();
        // Looping
        while (IsLooping())
        {
            // Send to Arduino
            if (outputQueue.Count != 0)
            {
                string command = (string)outputQueue.Dequeue();
                WriteToArduino(command);
            }
            // Read from Arduino
            string result = ReadFromArduino(10000);
            if (result != null)
                inputQueue.Enqueue(result);
        }
        stream.Close();
    }

    private bool looping = true;
    private bool IsLooping()
    {
        lock (this)
        {
            return looping;
        }
    }

    public void StopThread()
    {
        lock (this)
        {
            looping = false;
        }
    }

    private void OnDisable()
    {
        StopThread();
    }

    public static string AutodetectArduinoPort()
    {
        List<string> comports = new List<string>();
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        string temp;
        foreach (string s3 in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s3);
            foreach (string s in rk3.GetSubKeyNames())
            {
                if (s.Contains("VID") && s.Contains("PID"))
                {
                    RegistryKey rk4 = rk3.OpenSubKey(s);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null && temp.Contains("Arduino"))
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }

        return "COM9";
    }
}
