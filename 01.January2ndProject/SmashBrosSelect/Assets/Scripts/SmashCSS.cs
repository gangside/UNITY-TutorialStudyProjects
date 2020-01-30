using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmashCSS : MonoBehaviour
{
    public List<Character> characters = new List<Character>();
    public GameObject characterCellPrefab;

    private void Start() {
        //모든 캐릭터를 생성해줍니다. SpawnCharacterCell() 그럼 리스트를 반복문으로 돌려야함
        //그리고 돌리면서 캐릭터 데이터를 차례차례 입력해야줘야하고

        foreach (Character character in characters) {
            SpawnCharacterCell(character);
        }
    }

    // 캐릭터를 생성하고
    // 캐릭터 데이터를 집어넣습니다.
    private void SpawnCharacterCell(Character characterInfo) {
        GameObject characterCell = Instantiate(characterCellPrefab, transform);

        Image artwork = characterCell.transform.Find("Artwork").GetComponent<Image>();
        TextMeshProUGUI text = characterCell.transform.Find("Name Tag").GetComponentInChildren<TextMeshProUGUI>();

        artwork.sprite = characterInfo.sprite;
        text.text = characterInfo.name;

        //화면에 보일 artwork의 피봇값을 uiPivot(UI에 보이는 피봇)으로 설정해야한다.
        //uiPivot은 텍스쳐 피봇과 다르다. UI에선 스케일로 피봇을 책정하는데, 텍스쳐는 픽셀단위라 화면이 넓을 경우 피봇을 변경해도 UI에서 티가 안난다~
        //고로 텍스쳐의 피봇위치를 스케일로 변환해야하는데, 이는 각 피봇에서 텍스쳐의 가로세로 길이를 나누면 스케일 피봇이 나온다.
        Vector2 pixelSize = new Vector2(artwork.sprite.texture.width, artwork.sprite.texture.height);
        Vector2 pixelPivot = artwork.sprite.pivot;
        Vector2 uiPivot = new Vector2(pixelPivot.x / pixelSize.x, pixelPivot.y / pixelSize.y);


        artwork.GetComponent<RectTransform>().pivot = uiPivot;
        artwork.GetComponent<RectTransform>().sizeDelta *= characterInfo.artworkScale;
    }
}
