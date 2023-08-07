using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformSpawner : MonoBehaviour
{
    public List<GameObject> platforms = new List<GameObject>();
    public List<Transform> currentPlatforms = new List<Transform>();
    private int offset;
    private Transform player;
    private Transform currentPlatformPoint;
    private int platformIndex;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        PlatformStart();
    }

    void Update()
    {
        float distance = player.position.z - currentPlatformPoint.position.z;

        if(distance >= 5){
            Reposition(currentPlatforms[platformIndex].gameObject);
            platformIndex++;

            if(platformIndex >= currentPlatforms.Count - 1){
                platformIndex = 0;
            }

            currentPlatformPoint = currentPlatforms[platformIndex].GetComponent<NewPlatform>().point;
        }
    }

    void PlatformStart(){
        for(int i = 0; i < platforms.Count; i++){
            Transform p = Instantiate(platforms[i], new Vector3(0, 0, i * 40f), transform.rotation).transform;
            currentPlatforms.Add(p);
            offset += 40;
        }

        currentPlatformPoint = currentPlatforms[platformIndex].GetComponent<NewPlatform>().point;
    }
    public void Reposition(GameObject platform){
        platform.transform.position = new Vector3(0,0, offset);
        offset += 40;
    }
    
}
