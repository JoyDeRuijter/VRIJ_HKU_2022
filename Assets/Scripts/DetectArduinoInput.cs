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
        sp = new SerialPort(serialPort, 9600);
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


    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                print(sp.ReadByte());
                gameManager.ReceiveInput(sp.ReadByte());
            }
            catch (System.Exception)
            {
                Debug.Log("Could not receive Arduino input, exception error!");
            }
        }
    }
}
