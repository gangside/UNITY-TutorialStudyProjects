using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour {

	public static LobbyScene Instance { get; set; }
	
	void Start() {
		Instance = this;
	}


	public void OnClickCreateAccount() {
		DisableInputs();

		string username = GameObject.Find("Username").GetComponent<InputField>().text;
		string password = GameObject.Find("Password").GetComponent<InputField>().text;
		string email = GameObject.Find("Email").GetComponent<InputField>().text;

		Client.Instance.SendCreateAccount(username, password, email);
	}

	public void OnClickLoginRequest() {
		DisableInputs();

		string usernameOrEmail = GameObject.Find("LoginUsernameOrEmail").GetComponent<InputField>().text;
		string password = GameObject.Find("LoginPassword").GetComponent<InputField>().text;

		Client.Instance.SendLoginRequest(usernameOrEmail, password);
	}

	public void ChangeWelcomeMessage(string msg) {
		GameObject.Find("Tag Welcome").GetComponentInChildren<Text>().text = msg;

	}

	public void ChangeAuthenticationMessage(string msg) {
		GameObject.Find("Tag Authentication").GetComponentInChildren<Text>().text = msg;
	}

	public void EnableInputs() {
		GameObject.Find("Canvas").GetComponent<CanvasGroup>().interactable = true;
	}

	public void DisableInputs() {
		GameObject.Find("Canvas").GetComponent<CanvasGroup>().interactable = false;
	}
}
