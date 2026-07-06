using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering; // Necessario per AsyncGPUReadback

public class CameraSaver : MonoBehaviour
{
    public Camera camera1; // Trascina qui la camera del giocatore nell'inspector
    public int width = 1920;
    public int height = 1080;

    public void SalvaImmagine() //funzionne che salva in Desktop/project/Demo1/Screenshot una immagine catturata da camera1 da settare nell'inspector
    {
        // 1. Crea una RenderTexture temporanea
        RenderTexture rt = new RenderTexture(width, height, 24);
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
        Destroy(rt);

        // 4. Codifica in PNG e salva
        byte[] bytes = screenShot.EncodeToJPG();
        string nomeFile = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".jpg";
        string percorso = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "project", "Demo1", "Screenshot"), nomeFile);
        
        File.WriteAllBytes(percorso, bytes);

        // 5. Immagine RAW da inviare ROS
        byte[] rawData = screenShot.GetRawTextureData();

        Destroy(screenShot);

        Debug.Log("Immagine salvata in: " + percorso);
        Debug.Log(rawData.Length);//stampo la lunghezza dell'array
        // Debug.Log(string.Join(", ", rawData));//stampo l'array (pesante)

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) { //esegue il codice solo una volta quando premo il tasto V, se tengo premuto esegue il codice solo l'update in cui ho iniziato a premere il tasto
            SalvaImmagine();
        }
    }
}
