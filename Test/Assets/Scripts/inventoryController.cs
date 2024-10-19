
using UnityEngine;

public class inventoryController : MonoBehaviour
{

    [SerializeField] GameObject panel;
    [SerializeField] GameObject toolbarPanel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(true);
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)){
            panel.SetActive(!panel.activeInHierarchy);
            toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
        }
    }
}
