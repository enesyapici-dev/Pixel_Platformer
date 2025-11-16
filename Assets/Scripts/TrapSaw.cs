using System.Collections;
using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float cooldown = 1;
    private Vector3[] wayPointPositions;
    public int wayPointIndex = 1;
    private int moveDirection = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {
        UpdateWayPointsInfo();
        transform.position = wayPointPositions[0];

    }

    private void UpdateWayPointsInfo()
    {
        wayPointPositions = new Vector3[wayPoints.Length];

        for (int i = 0; i < wayPoints.Length; i++)
        {
            wayPointPositions[i] = wayPoints[i].position;
        }
    }

    private void Update()
    {
        anim.SetBool("active", canMove);
        if (!canMove)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPointPositions[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPositions[wayPointIndex]) < 0.1f)
        {
            if (wayPointIndex == wayPointPositions.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopMovement(cooldown));
            }
            wayPointIndex = wayPointIndex + moveDirection;
        }

    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
