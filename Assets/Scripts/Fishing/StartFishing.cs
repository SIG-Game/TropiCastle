using UnityEngine;

public class StartFishing : MonoBehaviour
{
    public GameObject Hook;
    public GameObject fish;
    // Start is called before the first frame update
    void Start()
    {
        fish.GetComponent<RectTransform>().anchoredPosition = new Vector3(180,0,0);
    }
}
