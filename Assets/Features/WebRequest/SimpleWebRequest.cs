using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SimpleWebRequest : MonoBehaviour {
    private void Start() {
        StartCoroutine(PostRequest("https://hsbc-hk.isysedge.com/api/generate_response", "Hallo"));
    }

    private IEnumerator PostRequest(string url, string question) {
        PostData postData = new PostData {
            company = "HSBC Virtual Branch",
            database = "DBB6E1F4AC423E1A35A4EB8C5DAFDF43AB",
            role = "Virtual Customer Service Representative",
            question = question,
            history = new List<string>(),
            version = "v10"
        };

        string json = JsonUtility.ToJson(postData);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Request JSON: " + json);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError(request.error);
        } else {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class PostData {
        public string company;
        public string database;
        public string role;
        public string question;
        public List<string> history;
        public string version;
    }
}
