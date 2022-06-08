using System.IO.Ports;
using UnityEngine;


public class DetectArduinoInput : MonoBehaviour
{
    private GameManager gameManager;
    private enum SerialPortName { COM2, COM3, COM4, COM5, COM6 }
    [SerializeField] private SerialPortName serialPortName = SerialPortName.COM4;
    private string serialPort;
    private SerialPort sp;

    void Start()
    {
        ConvertSerialPort();
        sp = new SerialPort(serialPort, 19200);
        gameManager = GameManager.instance;
        sp.Open();
        sp.ReadTimeout = 100;
        Debug.Log("Selected port: " + serialPort);
        InvokeRepeating("SerialDataReading", 0f, 0.01f);
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
            if (int.TryParse(receivedString, out int pipe))
            {
                if (pipe > -1 && pipe != lastinput) //is controller connected? and is button pressed only once
                    gameManager.ReceiveInput(pipe);
                lastinput = pipe;
                sp.BaseStream.Flush();
            }
        }
    }

    private void OnApplicationQuit()
    {
        sp.Close();
    }


    private string receivedString;
    private string SerialDataReading()
    {

        try
        {
            receivedString = sp.ReadLine();
        }
        catch (System.TimeoutException) { }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            Debug.Log("Could not receive Arduino input, exception error!");
        }
        Debug.Log(receivedString);
        return receivedString;
    }
}
