using Riptide;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }


    public GameObject PlayerPrefab => playerPrefab;
    public GameObject BulletPrefab => bulletPrefab;
    public GameObject TeleporterPrefab => teleporterPrefab;
    public GameObject LaserPrefab => laserPrefab;

    [SerializeField] private float roundLengthSeconds;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject teleporterPrefab;
    [SerializeField] private GameObject laserPrefab;
    
    private byte activeScene;

    private void Awake()
    {
        Singleton = this;
    }
}
