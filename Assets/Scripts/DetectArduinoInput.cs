using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class DetectArduinoInput : MonoBehaviour
{
    private GameManager gameManager;
    private enum SerialPortName {COM2, COM3, COM4, COM5, COM6}
    [SerializeField] private SerialPortName serialPortName = SerialPortName.COM4;
    private string serialPort;
    private SerialPort sp;

    void Start()
    {
        ConvertSerialPort();
        sp = new SerialPort(serialPort, 4800);
        gameManager = GameManager.instance;
        sp.Open();
        sp.ReadTimeout = 100;
        Debug.Log("Selected port: " + serialPort);
    }

    private void ConvertSerialPort()
    {
        if (serialPortName == SerialPortName.COM2)
            serialPort = "COM2";
        else if (serialPortName == SerialPortName.COM3)
            serialPort = "COM3";
        else if (serialPortName == SerialPortName.COM4)
            serialPort = "COM4";
        else if (serialPortName == SerialPortName.COM5)
            serialPort = "COM5";
        else if (serialPortName == SerialPortName.COM6)
            serialPort = "COM6";
    }

    int lastinput = 0;

    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                string line = sp.ReadLine();
                Debug.Log(line);
                if (int.TryParse(line, out int pipe))
                {
                    if (pipe > -1 && pipe != lastinput) //is controller connected? and is button pressed only once
                        gameManager.ReceiveInput(pipe);
                    lastinput = pipe;
                }
                sp.BaseStream.Flush();
            }
            catch (System.TimeoutException) { }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Could not receive Arduino input, exception error!");
            }
        }
    }

    private void OnApplicationQuit()
    {
        sp.Close();
    }
}
