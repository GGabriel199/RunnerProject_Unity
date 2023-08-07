using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public CharMovement player;
    void OnCollisionEnter(Collision col){
        if(col.transform.CompareTag("Player")){
            player.OnCharacterColliderHit(col.collider);
            return;
        }
    }
}
