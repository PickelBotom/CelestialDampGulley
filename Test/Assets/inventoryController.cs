using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryController : MonoBehaviour
{

    [SerializeField] GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){
            panel.SetActive(!panel.activeInHierarchy);
        }
    }
}
