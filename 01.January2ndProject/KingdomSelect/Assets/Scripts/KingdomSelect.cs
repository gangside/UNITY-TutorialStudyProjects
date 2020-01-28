using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KingdomSelect : MonoBehaviour
{
    [Header("Kingdom Point Settings")]
    public List<Kingdom> kingdoms = new List<Kingdom>();

    [Space]
    [Header("Public References")]
    public GameObject kingdomPointPrefab;
    public GameObject kingdomButtonPrefab;
    public Transform modelTransform;
    public Transform kingdomButtonContainer;

    [Space]
    [Header("Tween Settings")]
    public float lookDuration;
    public Ease lookEase;

    [Space]
    [Header("Camera offset")]
    public Vector2 visualOffset;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Kingdom k in kingdoms) {
            SpawnKingdomPoint(k);
        }

        if (kingdoms.Count > 0) {
            LookAtKingdom(kingdoms[0]);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(kingdomButtonContainer.GetChild(0).gameObject);
        }
    }

    private void SpawnKingdomPoint(Kingdom k) {
        GameObject kingdom = Instantiate(kingdomPointPrefab, modelTransform);
        kingdom.transform.localEulerAngles = new Vector3(k.y + visualOffset.y, - k.x - visualOffset.x, 0);
        k.point = kingdom.transform.GetChild(0);


        SpawnKingdomButton(k);
    }

    private void SpawnKingdomButton(Kingdom k) {
        Kingdom kingdom = k;
        Button kingdomButton = Instantiate(kingdomButtonPrefab, kingdomButtonContainer).GetComponent<Button>();
        kingdomButton.onClick.AddListener(() => LookAtKingdom(kingdom));

        kingdomButton.transform.GetChild(0).GetComponentInChildren<Text>().text = kingdom.name;
    }

    public void LookAtKingdom(Kingdom k) {
        Transform cameraParent = Camera.main.transform.parent;
        Transform cameraPivot = cameraParent.parent;

        cameraParent.DOLocalRotate(new Vector3(k.y, 0, 0), lookDuration, RotateMode.Fast).SetEase(lookEase);
        cameraPivot.DOLocalRotate(new Vector3(0, -k.x, 0), lookDuration, RotateMode.Fast).SetEase(lookEase);

        FindObjectOfType<FollowTarget>().target = k.point;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            LookAtKingdom(kingdoms[i]);
            i++;
            if(i == kingdoms.Count) {
                i = 0;
            }
        }
    }

}

[System.Serializable]
public class Kingdom {
    public string name;

    [Range(-180, 180)]
    public float x;
    [Range(-89, 89)]
    public float y;

    public Transform point;
}
