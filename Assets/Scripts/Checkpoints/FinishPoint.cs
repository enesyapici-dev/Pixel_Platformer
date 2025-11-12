using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    void OnTriggerEnter2D(Collider2D collision)
    {


        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            anim.SetTrigger("activate");
            Debug.Log("Level Complere");
        }

    }
}