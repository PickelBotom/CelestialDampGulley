using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType{
    Warp,
    Scene
}
public class Transition : MonoBehaviour
{
    [SerializeField] TransitionType transitionType;
    [SerializeField] string sceneNameToTransition;
    Transform destination;

    internal void InitiateTransition(Transform toTransition)
    {

        Cinemachine.CinemachineBrain currentCamera = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();

        switch(transitionType){
            case TransitionType.Warp:
                currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(
                    toTransition,
                    destination.position - toTransition.position);
                toTransition.position = new Vector3(
                    destination.position.x,
                    destination.position.y,
                    toTransition.position.z);
                break;
            case TransitionType.Scene:
                currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(
                    toTransition,
                    destination.position - toTransition.position);
                GameSceneManager.instance.SwitchScene(sceneNameToTransition);
            break;
        }

    }

    void Start()
    {
        destination = transform.GetChild(1);
    }

}
