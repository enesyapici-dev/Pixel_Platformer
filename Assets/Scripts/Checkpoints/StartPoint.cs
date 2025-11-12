using UnityEngine;

public class StartPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();

    void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            anim.SetTrigger("activate");
            Debug.Log("Level Complere");
        }
    }
}
