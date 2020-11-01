using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Data.Common;

public class RoomManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int xAnchor;
    public int yAnchor;
    public int columns = 8;
    public int rows = 8;
    public int length = 10;
    public int height = 10;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    public Transform roomHolder;
    
    private List<Vector3> gridPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitialiseList(int xStart, int yStart)
    {
        gridPositions.Clear();

        for (int x = xStart + 1; x < xStart + columns - 1; x++)
        {
            for (int y = yStart + 1; y < yStart + rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup(int xStart, int yStart)
    {
        roomHolder = new GameObject("Board").transform;

        for (int x = xStart - 1; x < xStart + columns + 1; x++)
        {
            for (int y = yStart - 1; y < yStart + rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == xStart - 1 || x == xStart + columns || y == yStart - 1 || y == yStart + rows)
                {
                    if (x == xStart + (columns / 2) - 1 || x == xStart + (columns / 2) || y == yStart + (rows / 2) - 1 || y == yStart + (rows / 2))
                    {
                        if (x == -length - 1 || y == -height - 1 || x == length + columns || y == height + rows)
                            toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }
                    else
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(roomHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupRoom()
    {

    }
}
