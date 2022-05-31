using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class Move : MonoBehaviour
{

    SerialPort sp = new SerialPort("COM3", 9600);

    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 100;
    }


    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                // Buis 1 wordt in geblazen
                if (sp.ReadByte() == 1)
                {
                    print(sp.ReadByte());
                    transform.Translate(Vector3.left * Time.deltaTime * 5);

                }
                // Buis 2 wordt in geblazen
                if (sp.ReadByte() == 2)
                {
                    print(sp.ReadByte());
                    transform.Translate(Vector3.right * Time.deltaTime * 5);

                }
                // Buis 3 wordt in geblazen
                if (sp.ReadByte() == 3)
                {
                    print(sp.ReadByte());

                    Destroy(gameObject);
                }
                // Buis 4 wordt in geblazen
                if (sp.ReadByte() == 4)
                {
                    print(sp.ReadByte());
                    Destroy(gameObject);
                }
            }
            catch (System.Exception)
            {

            }

        }
    }
}
