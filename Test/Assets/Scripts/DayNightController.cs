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

  [SerializeField] float timescale = 65f;
[SerializeField] Text text;

float Hours{
     get{ return time / 3600f;}
}
private void Update()
{
    time += Time.deltaTime*timescale;
    text.text= Hours.ToString();
    float v = nightTimeCurve.Evaluate(Hours);
    Color c = Color.Lerp(dayLightColor,nightLightColor,v);
    globalLight.color=c;
    if((time>secondsinDay))
    {
        NextDay();
    }
}
private void NextDay()
{
 time=0;
 days+=1;

}

}
