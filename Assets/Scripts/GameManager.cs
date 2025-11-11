using System.Collections;
using Unity.Mathematics;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player")]

    public Player player;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;

    [Header("Fruit Management")]

    [SerializeField] private bool fruitsHaveRandomLook;
    public int fruitsCollected;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
    public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, quaternion.identity);
        player = newPlayer.GetComponent<Player>();
    }
    public void AddFruit() => fruitsCollected++;
    public bool FruitsHaveRandomLook() => fruitsHaveRandomLook;



}
