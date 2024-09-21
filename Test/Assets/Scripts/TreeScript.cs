using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
     public float Health {
        set {
            health = value;

            if(health <= 0) {
                RemoveTree();
            }
        }
        get {
            return health;
        }
    }

    public float health = 1;

    // private void Start() {
    //     animator = GetComponent<Animator>();
    // }

    // public void Defeated(){
    //     animator.SetTrigger("Defeated");
    // }

    public void RemoveTree() {
        Destroy(gameObject);
    }
}
