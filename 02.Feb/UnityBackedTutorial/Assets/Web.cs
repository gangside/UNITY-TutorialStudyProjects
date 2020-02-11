using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour
{
    void Start() {
        // A correct website page.
        StartCoroutine(GetRequest("http://localhost/UnityBackendTutorial/GetData.php"));

        // A non-existing page.
        //StartCoroutine(GetRequest("https://error.html"));
    }

    IEnumerator GetRequest(string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //uri 주소를 / 기준으로 짜르면 페이지 목록을 확인 할 수 있지
            string[] pages = uri.Split('/');
            //페이지의 넘버
            int page = pages.Length - 1;

            if (webRequest.isNetworkError) {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                byte[] results = webRequest.downloadHandler.data; //데이터를 바이트 형식으로 받아온다.
            }
        }
    }
}
