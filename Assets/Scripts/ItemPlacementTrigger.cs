using UnityEngine;

public class ItemPlacementTrigger : MonoBehaviour
{
    public bool canPlace = false;

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canPlace = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPlace = true;
    }
}
