using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cinemachine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    string currentScene;
    AsyncOperation unload;
    AsyncOperation load;
    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }
    public void InitSwitchScene(string to, Vector3 targetPosition){
        StartCoroutine(Transition(to,targetPosition));
    }

    IEnumerator Transition(string to, Vector3 targetPosition){
        screenTint.Tint();

        yield return new WaitForSeconds(1f / screenTint.speed + 0.1f);
        SwitchScene(to, targetPosition);
        while(load != null & unload != null){
            if(load.isDone){load = null;}
            if(unload.isDone){unload = null;}
            yield return new WaitForSeconds(0.1f);

        }
        
        cameraConfiner.UpdateBounds();
        screenTint.UnTint();
    }
    void Awake()
    {
        instance = this;
    }

    [SerializeField] ScreenTint screenTint;
    [SerializeField] CameraConfiner cameraConfiner;

    public void SwitchScene(string to,Vector3 targetPosition){
        load = SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        unload = SceneManager.UnloadSceneAsync(currentScene);
        currentScene = to;
        Transform playerTransform = GameManager.instance.player.transform;
        Cinemachine.CinemachineBrain currentCamera = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
        currentCamera.ActiveVirtualCamera.OnTargetObjectWarped(
            playerTransform,
            targetPosition - playerTransform.position
        );

        playerTransform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            targetPosition.z
        );
    }

}
