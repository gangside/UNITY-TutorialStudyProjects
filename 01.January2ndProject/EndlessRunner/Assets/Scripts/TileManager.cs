using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;

    Transform playerTransform;

    float safeZone = 20f;
    float nextSpawnPositionZ = -10.0f;
    float tileLength = 10f;

    int tileAmountOnScreen = 8;
    int lastPrefabIndex = 0;

    Queue<GameObject> activeTiles = new Queue<GameObject>();

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerMotor>().transform;

        for (int i = 0; i < tileAmountOnScreen; i++) {
            if(i <= 3) {
                SpawnTile(0);
            }
            else {
                SpawnTile();
            }
        }
    }

    void Update()
    {
        float allTileLength = tileAmountOnScreen * tileLength;
        if (playerTransform.position.z - safeZone > (nextSpawnPositionZ - allTileLength)) {
            SpawnTile();
            DeleteTile();
        }
    }

    void SpawnTile(int prefabIndex = -1) {
        GameObject tile;

        if (prefabIndex == -1) {
            tile = Instantiate(tilePrefabs[RandomPrefabIndex()]) as GameObject;
        }
        else {
            tile = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        }

        tile.transform.SetParent(transform);
        tile.transform.position = Vector3.forward * nextSpawnPositionZ - Vector3.up * 0.5f;

        nextSpawnPositionZ += tileLength;

        //튜토리얼에선 List를 쓰지만, 타일의 생성 방식이 Queue 하고 닮아있기때문에 Queue를 쓰는게 훨씬 자연스럽다
        activeTiles.Enqueue(tile);
    }

    void DeleteTile() {
        Destroy(activeTiles.Dequeue());
    }

    int RandomPrefabIndex() {
        if(tilePrefabs.Length < 0) {
            return 0;
        }

        int randomIndex = lastPrefabIndex;
        //연속해서 중복생성되는걸 막기위해서인데, 다른 좋은 방법도 있을것같은데 흠
        while (randomIndex == lastPrefabIndex) {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
}
