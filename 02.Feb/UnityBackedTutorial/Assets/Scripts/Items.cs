using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System;

public class Items : MonoBehaviour
{

    Action<string> _createItemsCallBack;


    // Start is called before the first frame update
    void Start()
    {
        _createItemsCallBack = (jsonArrayString) =>
        {
            StartCoroutine(CreateItemsRoutine(jsonArrayString));
        };

        CreateItems();
    }

    public void CreateItems() {
        string userId = Main.Instance.userInfo.userID;
        Debug.Log("유저고유아이디 번호: " + userId);
        Debug.Log("해당 유저에 해당하는 아이템 목록을 생성합니다");
        StartCoroutine(Main.Instance.web.GetItemsIDs(userId, _createItemsCallBack));
    }

    IEnumerator CreateItemsRoutine(string jsonArrayString) {
        //Parsing json array string as an array

        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

        for (int i = 0; i < jsonArray.Count; i++) {
            //Create local variables
            bool isDone = false; // are we done downloading?
            string itemId = jsonArray[i].AsObject["itemID"];
            JSONObject itemInfoJson = new JSONObject();

            //Create a callback to get the information from web.cs;
            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                JSONArray tempArray = JSON.Parse(itemInfo) as JSONArray;
                itemInfoJson = tempArray[0].AsObject;
            };

            StartCoroutine(Main.Instance.web.GetItem(itemId, getItemInfoCallback));

            //Wait until the callback is called from WEB (INFO finished downloading);
            yield return new WaitUntil(() => isDone);

            //Instantiate GameObject (item prefab)
            GameObject item = Instantiate(Resources.Load("Prefabs/item")) as GameObject;
            item.transform.SetParent(this.transform);

            //Fill Information
            item.transform.Find("name").GetComponent<Text>().text = itemInfoJson["name"];
            item.transform.Find("description").GetComponent<Text>().text = itemInfoJson["description"];
            item.transform.Find("price").GetComponent<Text>().text = itemInfoJson["price"];

            //continue to the next item

        }

        yield return null;
    }
}
