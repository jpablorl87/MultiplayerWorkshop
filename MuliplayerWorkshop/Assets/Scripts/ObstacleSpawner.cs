using Photon.Pun;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviourPun
{
    [SerializeField] private ObjectPool floorPool;
    [SerializeField] private ObjectPool ceilingPool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 2.5f;
    private float nextSpawnTime;
    private bool isSpawningActive = false;
    private void OnEnable()
    {
        EventManager.OnGameStart += StartSpawning;
        EventManager.OnGameOver += StopSpawning;
    }
    private void OnDisable()
    {
        EventManager.OnGameStart -= StartSpawning;
        EventManager.OnGameOver -= StopSpawning;
    }
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            enabled = false;
            Debug.Log("[ObstaceSpawner] Spawner disabled (i'm not the host)");
            return;
        }
        Debug.Log("[ObstaceSpawner] Spawner enabled (i'm the host)");
    }
    private void StartSpawning(int hostId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isSpawningActive = true;
            nextSpawnTime = Time.time + 1;
            Debug.Log("[ObstaceSpawner] Obstacle generation started");
        }
    }
    private void StopSpawning(int hostId)
    {
        isSpawningActive = false;
        Debug.Log("[ObstaceSpawner] Obstacle generation stopped");
    }
    private void Update()
    {
        if (!isSpawningActive) return;
        if (Time.time > nextSpawnTime)
        {
            DecideAndSpawnObstacle();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }
    private void DecideAndSpawnObstacle()
    {
        /*GameObject newObstacle = null;
        bool isFloorObstacle = Random.Range(0, 2) == 0;//50% of probability
        if (isFloorObstacle)
        {
            newObstacle = floorPool.GetPooledObject();
        }
        else
        {
            newObstacle = ceilingPool.GetPooledObject();
        }
        if (newObstacle != null)
        {
            newObstacle.transform.position = spawnPoint.position;
            Debug.Log($"[ObstaceSpawner] {isFloorObstacle} Floor : Ceiling");
        }*/
        bool isFloorObstacle = Random.Range(0, 2) == 0;
        Vector3 spawnPos = spawnPoint.position;
        photonView.RPC("RPCSyncSpawn", RpcTarget.All, isFloorObstacle, spawnPos);
    }
    [PunRPC]
    public void RPCSyncSpawn(bool isFloor, Vector3 position)
    {
        ObjectPool poolToUse = isFloor ? floorPool : ceilingPool;
        GameObject newObstacle = poolToUse.GetPooledObject();
        if (newObstacle != null)
        {
            newObstacle.transform.position = position;
            newObstacle.SetActive(true);
        }
    }
}
