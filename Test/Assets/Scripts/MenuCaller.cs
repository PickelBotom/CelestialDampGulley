using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCaller : MonoBehaviour
{
	[SerializeField] GameObject MenuPanel;
	[SerializeField] GameObject InventoryPanel;
	[SerializeField] GameObject environmentStatsTab;
	[SerializeField] GameObject Toolbar;
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!MenuPanel.activeInHierarchy)
			{
				InventoryPanel.SetActive(false);
				environmentStatsTab.SetActive(false);
				Toolbar.SetActive(false);
				MenuPanel.SetActive(true);
			}
			else 
			{				
				MenuPanel.SetActive(false);
				Toolbar.SetActive(true);
			}
		}
	}
}
