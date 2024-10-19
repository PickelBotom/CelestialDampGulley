using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class DayNightController : MonoBehaviour
{
    const float secondsInDay = 86400f;

    [SerializeField] Color nightLightColor;
    [SerializeField] AnimationCurve nightTimeCurve;
    [SerializeField] Color dayLightColor = Color.white;

    [SerializeField] Light2D globalLight;
    [SerializeField] EnvironmentManager environmentManager; // Reference to EnvironmentManager

    float time;
    int days = 0;

    [SerializeField] float timeScale = 3000f;
    [SerializeField] float startTime = 28800f;
    float phaseInSec = secondsInDay / 2;

    List<TimeAgent> agents;

    private void Awake()
    {
        agents = new List<TimeAgent>();
    }

    void Start()
    {
        time = startTime;
    }

    public void Sub(TimeAgent tagent)
    {
        agents.Add(tagent);
    }

    public void UnSub(TimeAgent tagent)
    {
        agents.Remove(tagent);
    }

    float Hours
    {
        get { return time / 3600f; }
    }

    private void Update()
    {
        time += Time.deltaTime * timeScale;
        DayLightCalc();
        TimeAgents();

        if (time > secondsInDay)
        {
            NextDay();
        }
    }

    void DayLightCalc()
    {
        float v = nightTimeCurve.Evaluate(Hours);
        Color c = Color.Lerp(dayLightColor, nightLightColor, v);
        globalLight.color = c;
    }

    int oldPhase = 0;
    void TimeAgents()
    {
        int currentPhase = (int)(time / phaseInSec);
        if (oldPhase != currentPhase)
        {
            oldPhase = currentPhase;
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Invoke();
            }
        }
    }

    private void NextDay()
    {
        time = 0;
        days += 1;

        // Call the NewDayCycle method in EnvironmentManager
        if (environmentManager != null)
        {
            environmentManager.NewDayCycle(); // Spawn trash at the start of a new day
        }

        // Optionally invoke any agents here if needed
    }
}
