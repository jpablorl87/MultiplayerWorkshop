using Photon.Pun;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviourPun
{
    [SerializeField] private ObjectPool floorPool;
    [SerializeField] private ObjectPool ceilingPool;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 2.5f;
    private double nextSpawnPhotonTime;
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
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[ObstaceSpawner] I'm the host");
        }
    }
    private void StartSpawning(int hostId)
    {
        isSpawningActive = true;
        nextSpawnPhotonTime = PhotonNetwork.Time + 2.0f;
        Debug.Log("[ObstacleSpawner] Obstacle generation started");
    }
    private void StopSpawning(int hostId)
    {
        isSpawningActive = false;
        Debug.Log("[ObstaceSpawner] Obstacle generation stopped");
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isSpawningActive) return;

        double currentTime = PhotonNetwork.Time;

        if (currentTime > nextSpawnPhotonTime)
        {
            DecideAndSpawnObstacle();
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnPhotonTime = currentTime + interval;
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPCSyncSpawn", RpcTarget.All, isFloorObstacle, spawnPos);
        }
        else
        {
            RPCSyncSpawn(isFloorObstacle, spawnPos);
        }
    }
    [PunRPC]
    public void RPCSyncSpawn(bool isFloor, Vector3 position)
    {
        ObjectPool poolToUse = isFloor ? floorPool : ceilingPool;

        if (poolToUse == null)
        {
            Debug.LogError("[ObstacleSpawner] Pool not assigned in inspector");
            return;
        }

        GameObject newObstacle = poolToUse.GetPooledObject();

        if (newObstacle != null)
        {
            newObstacle.transform.position = position;
            newObstacle.SetActive(true);
            Debug.Log($"[ObstacleSpawner] Obstacle spawned in: {position}");
        }
    }
}
