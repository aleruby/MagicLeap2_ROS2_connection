using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class Ros_Sander_One_Camera : MonoBehaviour
{
    // Oggetto per la connessione al ROS-TCP-Connector
    private ROSConnection ros;
    // Stringa con il nome del topic
    public string topicName = "camera1/image/compressed";
    private bool flag_button = false;


    public Camera camera1; // Trascina qui la camera del giocatore nell'inspector
    public int width = 640;
    public int height = 480;
    public int jpgQuality = 75;

    private float timer = 0f;
    public int fps = 30;
    private float delta_frame;
    private bool is_processing = false;

    public byte[] CatturaImmagine() //funzionne che salva in Desktop/project/Demo1/Screenshot una immagine catturata da camera1 da settare nell'inspector
    {
        is_processing = true;

        // 1. Crea una RenderTexture temporanea
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
        camera1.targetTexture = rt;

        // 2. Forza la camera a renderizzare sulla texture
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        camera1.Render();
        
        // 3. Leggi i pixel dalla RenderTexture
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();
        camera1.targetTexture = null;
        RenderTexture.active = null; 
        RenderTexture.ReleaseTemporary(rt);

        // 4. Codifica in PNG e salva. Non ci serve in questo codice.
        // byte[] bytes = screenShot.EncodeToJPG();
        // string nomeFile = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".jpg";
        // string percorso = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "project", "Demo1", "Screenshot"), nomeFile);
        
        // File.WriteAllBytes(percorso, bytes);

        // 5. Immagine RAW da inviare ROS
        byte[] compressedData = screenShot.EncodeToJPG(jpgQuality);
        Destroy(screenShot);
        return compressedData;
    }

    public void PubbliacaImmagine(byte[] compressedBytes)
    {
        var message = new CompressedImageMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg 
            { 
                frame_id = "third_person_camera_frame",
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg()
            },
            format = "jpeg",
            data = compressedBytes
        };

        ros.Publish(topicName, message);
        is_processing = false;
    }

     // Start is called before the first frame update
    void Start()
    {   
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // Ottiene la connessione ROS
        ros = ROSConnection.GetOrCreateInstance();
        // Registra il publischer sul topic
        ros.RegisterPublisher<CompressedImageMsg>(topicName);

        delta_frame = 1 / (float)fps;
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && flag_button == false) {
            flag_button = true;
        }
        else if (Input.GetKeyDown(KeyCode.N) && flag_button == true) {
            flag_button = false;
        }
        timer += Time.deltaTime;
        if (flag_button == true && (timer >= delta_frame) && is_processing == false) {
            byte[] rawData = CatturaImmagine();
            PubbliacaImmagine(rawData);
            timer -= delta_frame; 
        }
    }
}
