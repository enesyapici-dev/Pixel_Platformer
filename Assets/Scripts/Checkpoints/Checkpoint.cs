using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool canBeReactivated;
    private bool active;
    private Animator anim => GetComponent<Animator>();

    void Start()
    {
        canBeReactivated = GameManager.instance.canReactivate;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && !canBeReactivated)
            return;

        Player player = collision.GetComponent<Player>();
        if (player != null)
            ActivateCheckpoint();

    }

    void ActivateCheckpoint()
    {
        active = true;
        anim.SetTrigger("Activate");
        GameManager.instance.UpdateRespawnPoint(transform);
    }
}
