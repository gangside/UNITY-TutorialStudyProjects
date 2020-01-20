using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    private void Start() {
        GenerateMap();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GenerateMap();
        }
    }

    private void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++) {
            SmoothMap();
        }

        //작은 벽들과 독립된 공간의 한계 크기를 조절한다
        ProcessMap();

        int borderSize = 1;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++) {
            for (int y = 0; y < borderedMap.GetLength(1); y++) {
                //아래부분에서 기존 부분과 똑같이 하려는데 왜 x < width + borderSize 를 하는지 이해가 잘 안간다...(?)
                // : 아마도 경계선을 늘리는 과정에서 위로 오른쪽으로만 커지기 때문에 중간을 맞춰주기 위해서..
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
                    //경계맵의 내부에는 기존 맵의 값을 똑같이 넣어줌
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else {
                    //경계를 벗어난 곳은 무조건 벽이되게 함
                    borderedMap[x, y] = 1;
                }
            }
        }
        
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(borderedMap, 1);
    }

    //작은 벽들과 독립된 공간의 한계 크기를 조절한다
    void ProcessMap() {
        List<List<Coord>> wallRegions = GetRegions(1);
        
        //벽이 시작되는 한도 사이즈
        int wallThreshholdSize = 50;
        
        foreach(List<Coord> wallRegion in wallRegions) {
            if(wallRegion.Count < wallThreshholdSize) {
                foreach (Coord tile in wallRegion) {
                    map[tile.tileX, tile.tileY] = 0; 
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);

        //벽이 시작되는 한도 사이즈
        //ex : 만약 0이라면 동굴이 채워질때 크기에 상관없이 많은 룸이 생성될거임
        //하지만 룸사이즈한계선이 있다면 50이하 크기의 룸지역은 모두 벽으로 채워질것임
        int roomThreshholdSize = 50;

        foreach (List<Coord> roomRegion in roomRegions) {
            if (roomRegion.Count < roomThreshholdSize) {
                foreach (Coord tile in roomRegion) {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
        }
    }

    List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
                    List<Coord> newRegions = GetRegionTiles(x, y);
                    regions.Add(newRegions);

                    foreach(Coord tile in newRegions) {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];

        //아래처럼 타입을 받아오는 식이라면, 물 바다 흙길 등등을 tileType으로 해서 받아와서 설정을 다르게 줄수도 있을듯.
        int tileType = map[startX, startY];
        print("tileType : " + tileType);

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                    if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY)) {
                        if(mapFlags[x,y] == 0 && map[x,y] == tileType) {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    bool IsInMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }


    void RandomFillMap() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        //GetHashCode()를 하면 string 타입의 seed 에서 숫자를 뽑아내는데 렌덤한 숫자라 prng 로서 가치가 있는듯, 자세하게는 더 알아봐야할듯.
        //prng는 pseudoRandomNumberGenerator 의 약자인데, 컴퓨터는 난수를 생성하지 못해서 일반적으로 난수를 위해서 seed가 필요하다.
        //이때 이 난수를 위한 seed를 랜덤하게 생성해주는게 필요한데, 일반적으로 prng 라고 부르는듯하다.
        System.Random prng = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                //아래 조건문은 그림을 그려보던가 해서 이해를 해야겠다... 아마도 정가운데 3*3공간에 1값을 넣어주는 듯 하다.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                }
                else {
                    //랜덤퍼센트보다 낮으면 1을 반환하는데, 이는 1벽을 의미하고 0은 빈 공간을 의미한다.
                    map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiels = GetSurroundingWallCount(x, y);
                //이웃한 타일이 4개 초과면 자신도 벽이된다..
                if (neighbourWallTiels > 4) {
                    map[x, y] = 1;
                }
                //이웃한 타일이 4개 미만이면 뚫린 공간이된다..
                //즉 이웃한 타일이 많은쪽은 점점 많은쪽으로 벽이되고, 없는쪽은 점점 없는쪽으로 빈공간이 되는것.
                else if(neighbourWallTiels < 4) {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        //아래 2중 for문은 gridX의 주면 3*3에 있는 이웃의 값을 가져온다.
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                //아래 이프문은 예방문인듯. 맵의 범위안에 이웃한 X,Y값이 있는지 판별
                if (IsInMapRange(neighbourX, neighbourY)) {
                    //이웃의 값만 가져오기 위해 자기자신의 값을 제외한다
                    //자기 자신을 빼고 이웃한 8개의 타일에서 벽의 개수를 카운트합니다
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else {
                    //이웃값을 계산할때 width, height값을 포함하지 않았으므로 테두리는 모두 벽으로 인정되며 카운트된다;
                    wallCount++;
                }
            }
        }
        return wallCount;
    }


    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }

    //private void OnDrawGizmos() {
    //    if (map != null) {
    //        for (int x = 0; x < width; x++) {
    //            for (int y = 0; y < height; y++) {
    //                Gizmos.color = (map[x, y] == 1) ? Color.red : Color.white;
    //                Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}
}
