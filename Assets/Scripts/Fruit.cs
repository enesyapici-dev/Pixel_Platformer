using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public enum FruitType { Apple, Banana, Cherry, Kiwi, Melon, Orange, Pineapple, Strawberry }
public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject PickupVFX;
    private GameManager gameManager;
    private Animator anim;


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }
    private void SetRandomLookIfNeeded()
    {
        if (gameManager.FruitsHaveRandomLook() == false)
        {
            UpdateFruitVisual();
            return;
        }

        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }
    private void UpdateFruitVisual() => anim.SetFloat("fruitIndex", (int)fruitType);
    void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);

            GameObject newFx = Instantiate(PickupVFX, transform.position, Quaternion.identity);
            Destroy(newFx, .5f);
        }
    }

}
