using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;

public class RosPublisher : MonoBehaviour
{  
    // Oggetto per la connessione al ROS-TCP-Connector
    private ROSConnection ros;

    // Stringa che contiene il nome del topic
    public string topicName = "chatter";

    // Stringhe che vogliemo inviare
    public string messageToSend = "START";
    public string messageToSend2 = "UPDATE";

    // Start is called before the first frame update
    void Start()
    {
        // Ottieni la connessione ROS
        ros = ROSConnection.GetOrCreateInstance();

        // Registriamo il publisher sul topic
        ros.RegisterPublisher<StringMsg>(topicName);

        // Creiamo un messaggio StringMsg
        StringMsg msg = new StringMsg(messageToSend);

        // Lo pubblichiamo su ROS
        ros.Publish(topicName, msg);

    }

    // Update is called once per frame
    void Update()
    {
        // Creiamo un messaggio StringMsg
        StringMsg msg = new StringMsg(messageToSend2);

        // Lo pubblichiamo su ROS
        ros.Publish(topicName, msg);
    }
}