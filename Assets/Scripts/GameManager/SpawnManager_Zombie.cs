using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnManager_Zombie : NetworkBehaviour
{
    [SerializeField]
    GameObject zombiePrefab;
    [SerializeField]
    private int maxNumberOfZombies = 50;

    private GameObject[] zombieSpawns;
    private int counter;
    private int numberOfZombie = 5;

    
    private float waveRate = 5;
    private bool isSpawnActivated = true;

    public override void OnStartServer()
    {
        zombieSpawns = GameObject.FindGameObjectsWithTag("ZombieSpawnPoint");
        StartCoroutine(ZombieSpawner());
    }

    IEnumerator ZombieSpawner()
    {
        for(;;)
        {
            yield return new WaitForSeconds(waveRate);
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            if(zombies.Length < maxNumberOfZombies)
            {
                CommenceSpawn();
            }
        }
    }

    void CommenceSpawn()
    {
        if(isSpawnActivated)
        {
            for(int i = 0; i < numberOfZombie; i++)
            {
                int randomint = Random.Range(0, zombieSpawns.Length);
                SpawnZombies(zombieSpawns[randomint].transform.position);
            }
        }
    }

    void SpawnZombies(Vector3 spawnPos)
    {
        counter++;
        GameObject go = GameObject.Instantiate(zombiePrefab, spawnPos, Quaternion.identity) as GameObject;
        go.GetComponent<Zombie_ID>().zombieID = "Zombie " + counter;
        NetworkServer.Spawn(go);
    }
}
