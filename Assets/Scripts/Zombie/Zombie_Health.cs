using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Zombie_Health : NetworkBehaviour
{
    private int health = 30;

    [SerializeField]
    private GameObject destroyFX;

    public void DeductHealth(int dmg)
    {
        health -= dmg;
        CheckHealth();
    }

    void CheckHealth()
    {
        if(health <= 0)
        {
            health = 0;
            CmdTellServerSpawnDestroyFX();
            
        }
    }

    [Command]
    void CmdTellServerSpawnDestroyFX()
    {
        RpcSpawnDestroyFX();
    }

    [ClientRpc]
    void RpcSpawnDestroyFX()
    {
        GameObject.Instantiate(destroyFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
