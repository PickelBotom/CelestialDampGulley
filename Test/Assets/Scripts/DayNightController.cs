using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
public class DayNightController : MonoBehaviour
{
  const float secondsinDay = 86400f;

  [SerializeField] Color nightLightColor;
  [SerializeField] AnimationCurve nightTimeCurve;
  [SerializeField] Color dayLightColor = Color.white;
  
  [SerializeField] Light2D globalLight;
  float time;
  int days=0;

  [SerializeField] float timescale = 3000f;
  [SerializeField] float StartTime = 28800f;
//[SerializeField] Text text;

  float phaseinSec = secondsinDay/2;

List<TimeAgent> agents;

private void Awake()
{
  agents = new List<TimeAgent>();
}

void Start()
{
  time = StartTime;
}

public void Sub(TimeAgent tagent)
{
  agents.Add(tagent);
}
public void UnSub(TimeAgent tagent)
{
  agents.Remove(tagent);
}

float Hours{
     get{ return time / 3600f;}
}
private void Update()
{
    time += Time.deltaTime*timescale;
    //text.text= Hours.ToString();
    DayLightCalc();
    TimeAgents();
    
    if((time>secondsinDay))
    {
        NextDay();
    }
}

void DayLightCalc()
 {
    float v = nightTimeCurve.Evaluate(Hours);
    Color c = Color.Lerp(dayLightColor,nightLightColor,v);
    globalLight.color=c;
 } 

 int oldPhase = 0;
  void TimeAgents()
  {
    int currentPhase = (int)(time / phaseinSec);
   if(oldPhase!= currentPhase)
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
 time=0;
 days+=1;
// for (int i = 0; i < agents.Count; i++)
//     {
//     agents[i].Invoke();
//     } ---- for start of new day does item spawn
  
}
}
