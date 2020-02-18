using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button createnewButton;

    private RegisterUser registerUI;

    private void Awake() {
        registerUI = GameObject.Find("Canvas").transform.Find("RegisterUser").gameObject.GetComponent<RegisterUser>();
        //로그인 성공 이벤트 구독
        
    }

    void Start()
    {
        Main.Instance.web.LoginSuccess += LoginSuccess;
        loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(Main.Instance.web.Login(usernameInput.text, passwordInput.text));
        });

        createnewButton.onClick.AddListener(() => {
            registerUI.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        });
    }

    public void LoginSuccess() {
        Main.Instance.login.gameObject.SetActive(false);
        Main.Instance.userProfile.SetActive(true);

        //GameObject.Find("Canvas").transform.Find("UserProfile").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Inventory").gameObject.SetActive(true);
        //this.gameObject.SetActive(false);

        //Main.Instance.Inventory.transform.GetComponent<Items>().CreateItems();
    }
}
