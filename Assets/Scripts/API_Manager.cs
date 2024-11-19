using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class API_Manager : MonoBehaviour
{
    private string JsonString;
    private string APILink;
    public MonitorManager monitorManager;
    private bool isRenderAwake;
    public TextMeshProUGUI outputText;

    void Start()
    {
        APILink = "https://two798-robin-server.onrender.com";
        StartCoroutine(WakeUpRender());
    }

    IEnumerator WakeUpRender()
    {
        Debug.Log("Checking if Render is awake..");
        using UnityWebRequest webRequest = UnityWebRequest.Get(APILink);

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                Debug.Log(webRequest.error);
                break;
            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log(webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.Log(webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Server Awakened");
                isRenderAwake = true;
                break;
        }
        if (isRenderAwake) GetAllImages();
    }

    public void GetAllImages()
    {
        string uri = APILink + "/get_all";
        StartCoroutine(GetAllImagesRequest(uri));
    }
   
    public void RequestMemory(string prompt) {
        print("Received " + prompt);
        outputText.text = "Generating...";
        StartCoroutine(GenerateMemory(prompt));
    }

    IEnumerator GenerateMemory(string memory)
    {
        print("Generating... " + memory);
        int maxRetries = 3;
        int retries = 0;
        bool success = false;

        while (retries < maxRetries && !success) {
            using UnityWebRequest webRequest = UnityWebRequest.Get(APILink + "/generate_memory/" + memory);
            yield return webRequest.SendWebRequest();

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("Connection Error");
                    break;
                    
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.Log("Data Processing Error with Robin API Request");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log("Protocol Error with Robin API Request");
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log(":\nDatabase Data Received: " + webRequest.downloadHandler.text);
                    
                    JsonString = webRequest.downloadHandler.text; //json string
                    try
                    {
                        Artifact newMemory = JsonConvert.DeserializeObject<Artifact>(JsonString);
                        monitorManager.AddArtifact(newMemory);
                        outputText.text = "";
                        success = true; // Request was successful, exit loop
                    }
                    catch (System.Exception exception)
                    {
                        retries++;
                        Debug.Log("ERROR: " + exception);
                    }
                    break;
            }

            retries++;
            if (!success && retries < maxRetries) {
                outputText.text = "Still generating... ";
                yield return new WaitForSeconds(2); // Optional: add a delay before retrying
            }
        }

        if (!success) {
            outputText.text = "Sorry, try again.";
        }
    }

    IEnumerator GetAllImagesRequest(string uri)
    {
        //imagePrompt.StartLoadingText();
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                break;
            case UnityWebRequest.Result.ProtocolError:
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                JsonString = webRequest.downloadHandler.text; //json string

                var results = JsonConvert.DeserializeObject<Root>(JsonString);
                //var result = JsonConvert.DeserializeObject<Artifact[]>(JsonString);

                monitorManager.artifacts = results.artifacts;
                int lastIndex = monitorManager.artifacts.Count - 1;
                monitorManager.left.index = 0;
                monitorManager.right.index = -1;

                if (monitorManager.artifacts.Count > 0)
                    monitorManager.right.SetArtifact(monitorManager.artifacts[lastIndex], lastIndex);
                
                if (monitorManager.artifacts.Count > 1)
                    monitorManager.middle.SetArtifact(monitorManager.artifacts[lastIndex - 1], lastIndex-1);

                if (monitorManager.artifacts.Count > 2)
                    monitorManager.left.SetArtifact(monitorManager.artifacts[lastIndex - 2], lastIndex-2);

                break;
        }
    }
}
