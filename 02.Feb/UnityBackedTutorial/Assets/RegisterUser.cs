using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUser : MonoBehaviour
{
    //인풋필드 3개, 아이디 한 개, 비번 두 개, 비번 두 개의 경우 서로 일치해야 등록할수있다.
    //SUBMIT 버튼을 누를 경우 뒤로 Web.cs의 RegisterUser로 등록한다. (비번 일치 검사는 이때 조건문으로 실시한다)
    //그리고 BACK버튼을 누를 경우 기존 로그인 화면으로 이동한다.
    //유저 등록이 완료됐을 경우, 안내 메세지 송출 후 로그인 화면으로 이동한다.

    public InputField username;
    public InputField password;
    public InputField confirmedPassword;
    public Button newCreateButton;
    public Button backButton;



    void Start()
    {
        newCreateButton.onClick.AddListener(() =>
        {
            StartCoroutine(Main.Instance.web.RegisterUser(username.text, password.text, confirmedPassword.text));
        });

        backButton.onClick.AddListener(() =>
        {
            GameObject.Find("Canvas").transform.Find("Login").gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        });
    }
}
