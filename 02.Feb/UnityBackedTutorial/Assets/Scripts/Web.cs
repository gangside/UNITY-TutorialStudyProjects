using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour
{
    string loginUri = "http://localhost/UnityBackendTutorial/Login.php";
    string RegisterUri = "http://localhost/unitybackendtutorial/RegisterUser.php";
    string getItemsIDs = "http://localhost/unitybackendtutorial/GetItemsIDs.php";
    string getItem = "http://localhost/unitybackendtutorial/GetItem.php";
    string sellItem = "http://localhost/unitybackendtutorial/SellItem.php";

    public GuideMessage guideMessage;

    //로그인 성공 이벤트
    public event System.Action LoginSuccess;

    //public void ShowUserItems() {
    //    StartCoroutine(GetItemsIDs(Main.Instance.userInfo.userID));
    //}

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

                //로그인 성공 여부를 어떻게 판단할것인가?
                Debug.Log("아이디를 설정합니다" + username + "," + password);
                Main.Instance.userInfo.SetCredentials(username, password);
                Main.Instance.userInfo.SetID(www.downloadHandler.text[www.downloadHandler.text.Length - 1].ToString());

                Debug.Log("Login Success : " + www.downloadHandler.text.Contains("Success"));
   
                if (www.downloadHandler.text.Contains("Success")) {
                    LoginSuccess();
                    guideMessage.ChangeMessage(www.downloadHandler.text.Remove(www.downloadHandler.text.Length - 1));
                }
                else {
                    guideMessage.ChangeMessage(www.downloadHandler.text);
                }

                guideMessage.gameObject.SetActive(true);


                //정확하게 로그인했다면.. 튜토리얼꺼
                //Main.Instance.userProfile.SetActive(true);
                //Main.Instance.login.gameObject.SetActive(false);

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

    public IEnumerator GetItemsIDs(string userID, System.Action<string> callback) {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(getItemsIDs, form)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log("현재 유저의 아이디에 해당하는 아이템ID를 가져왔습니다...");

            string jsonArray = webRequest.downloadHandler.text;

            //Call callback function to pass results
            callback(jsonArray);
        }
    }

    public IEnumerator GetItem(string itemID, System.Action<string> callback) {
        WWWForm form = new WWWForm();
        form.AddField("itemID", itemID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(getItem, form)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log("아이템 정보를 받아왔습니다...");

            string jsonArray = webRequest.downloadHandler.text;
            //Call callback function to pass results
            callback(jsonArray);
        }
    }

    public IEnumerator SellItem(string itemID, string userID) {
        WWWForm form = new WWWForm();
        form.AddField("itemID", itemID);
        form.AddField("userID", userID);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(sellItem, form)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            Debug.Log(webRequest.downloadHandler.text);
            Debug.Log("물건이 판매됐습니다");
        }
    }
}
