using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour
{
    string loginUri = "http://localhost/UnityBackendTutorial/Login.php";
    string RegisterUri = "http://localhost/unitybackendtutorial/RegisterUser.php";

    public GuideMessage guideMessage;

    //로그인 성공 이벤트
    public event System.Action LoginSuccess;

    void Start() {

        // A correct website page.
        //StartCoroutine(GetRequset("http://localhost/UnityBackendTutorial/GetData.php"));
        //StartCoroutine(GetRequset("http://localhost/UnityBackendTutorial/GetUsers.php"));
        //StartCoroutine(Login("testuser1", "1234"));
        //StartCoroutine(Login("testuser2", "1234"));
        //StartCoroutine(Login("testuser3", "1234"));
        //StartCoroutine(RegisterUser("user1", "123456"));

        // A non-existing page.
        //StartCoroutine(GetRequest("https://error.html"));
    }

    //public void Login(string username, string password) {
    //    StartCoroutine(RegisterUser(username, password))
    //}

    IEnumerator GetRequset(string uri) {
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

    IEnumerator GetUsers(string uri) {
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

    public IEnumerator Login(string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUri, form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                guideMessage.ChangeMessage(www.error);
                guideMessage.gameObject.SetActive(true);
                Debug.Log(www.error);
            }
            else {
                if (www.downloadHandler.text == "Login Success.") {
                    LoginSuccess();
                }

                Debug.Log(www.downloadHandler.text);
                guideMessage.ChangeMessage(www.downloadHandler.text);
                guideMessage.gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator RegisterUser(string username, string password, string confirmPass) {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);
        form.AddField("confirmPass", confirmPass);

        using (UnityWebRequest www = UnityWebRequest.Post(RegisterUri, form)) {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                guideMessage.ChangeMessage(www.error);
                guideMessage.gameObject.SetActive(true);
                Debug.Log(www.error);
            }
            else {
                guideMessage.ChangeMessage(www.downloadHandler.text);
                guideMessage.gameObject.SetActive(true);
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
