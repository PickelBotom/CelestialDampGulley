using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject environmentStatsTab;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] private Button statsButton;

    [SerializeField] private Text trashCountText;
    [SerializeField] private Text environmentHealthText;
    [SerializeField] private Text airQualityText;
    [SerializeField] private Text soilHealthText;
    [SerializeField] private EnvironmentManager environmentManager;

    private bool isEnvironmentStatsOpen = false;

	//private bool tutorialpopup = false;
	[SerializeField] GameObject Menupanel;

	void Start()
    {
        if (statsButton != null)
        {
            statsButton.onClick.AddListener(ToggleEnvironmentStatsPanel);
        }

        // Ensure both panels start as inactive
        panel.SetActive(false);
        environmentStatsTab.SetActive(false);

        // Subscribe to environmental health changes
        if (environmentManager != null)
        {
            environmentManager.onEnvironmentalHealthChange += UpdateEnvironmentStats;
        }
    }

    void Update()
    {
        // Toggle main inventory panel with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Menupanel.activeInHierarchy)
            {
				GameManager.instance.Checktut();

				if (isEnvironmentStatsOpen)
				{
					// Close environment stats and show main inventory if stats panel is open
					CloseEnvironmentStatsPanel();
					panel.SetActive(true);
				}
				else
				{
					// Toggle main inventory and toolbar visibility
					bool isActive = !panel.activeInHierarchy;
					panel.SetActive(isActive);
					toolbarPanel.SetActive(!isActive);
				}
			}           
        }
    }

    // Toggle to show or hide environment stats panel
    public void ToggleEnvironmentStatsPanel()
    {
        Debug.Log("Stats Button Clicked"); // Log when button is clicked
        if (isEnvironmentStatsOpen)
        {
            CloseEnvironmentStatsPanel();
            panel.SetActive(true); // Show main inventory panel
        }
        else
        {
            panel.SetActive(false); // Hide main inventory panel
            //Menupanel.SetActive(false);
            environmentStatsTab.SetActive(true); // Show environment stats tab
            isEnvironmentStatsOpen = true;

            UpdateEnvironmentStats((int)environmentManager.environmentalHealth); // Update stats when panel opens
        }
    }

    // Close the environment stats panel
    public void CloseEnvironmentStatsPanel()
    {
        environmentStatsTab.SetActive(false);
        isEnvironmentStatsOpen = false;
    }

    // Update the environment stats based on environmental health change
    public void UpdateEnvironmentStats(int health)
    {
        // Debug log to check if function is called
        Debug.Log("Updating Environment Stats");

        // Retrieve data from EnvironmentManager
        int trashCount = environmentManager.GetTrashCount();
        float airQuality = environmentManager.AirQuality;
        float soilHealth = environmentManager.SoilHealth;

        // Update text fields with percentage symbol only
        trashCountText.text = trashCount.ToString();
        environmentHealthText.text = health.ToString("F1") + "%"; // Use the passed health
        airQualityText.text = airQuality.ToString("F1") + "%";
        soilHealthText.text = soilHealth.ToString("F1") + "%";
    }

    private void OnDisable()
    {
        // Unsubscribe when the object is disabled to avoid memory leaks
        if (environmentManager != null)
        {
            environmentManager.onEnvironmentalHealthChange -= UpdateEnvironmentStats;
        }
    }
}
