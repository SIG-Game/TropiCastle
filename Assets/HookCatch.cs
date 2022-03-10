using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookCatch : MonoBehaviour
{
    public GameObject canvas;
    bool canCatch = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        canCatch = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        canCatch = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && canCatch) {
            Debug.Log("Fish Caught");
            canvas.SetActive(false);
        }
    }
}
