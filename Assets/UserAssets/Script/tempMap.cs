using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempMap : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("생성할 모델")]
    [SerializeField] GameObject HexTilePrefab;

    [Header("Info")]
    [Tooltip("X축 개수")]
    [Range(2, 50)]
    [SerializeField] int mapWidth;

    [Tooltip("Z축 개수")]
    [Range(2, 50)]
    [SerializeField] int mapHeight;

    [Tooltip("X축 간격")]
    [SerializeField] float tileXOffset;

    [Tooltip("Z축 간격")]
    [SerializeField] private float tileZOffset;
    void Start()
    {
        CreateHexTileMap();
    }

    public void CreateHexTileMap()
    {
        int mapXMin = -mapWidth / 2;
        int mapXMax = mapWidth / 2;

        int mapZMin = -mapHeight / 2;
        int mapZMax = mapHeight / 2;

        Vector3 _pos;
        GameObject TempGo;
        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                TempGo = Instantiate(HexTilePrefab, transform);
                if (z % 2 == 0) _pos = new Vector3(transform.position.x + x * tileXOffset, 0,
                                                   transform.position.z + z * tileZOffset);
                else _pos = new Vector3(transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                        transform.position.z + z * tileZOffset);
               // TempGo.transform.parent = transform;
                TempGo.transform.position = _pos;
            }
        }
    }
}
