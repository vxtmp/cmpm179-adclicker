using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Event for ad closed notification
    public event Action AdClosed;

    // Array of gameobject prefabs for ads
    [SerializeField]
    public GameObject[] adPrefabs;

    public int adsClosed = 0;


    public void spawnRandomAd()
    {
        // check if null, then choose a random ad and spawn a random ad. loop a random number of times between 1 and 4
        if (adPrefabs != null)
        {
            int numAds = UnityEngine.Random.Range(1, 1);
            for (int i = 0; i < numAds; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, adPrefabs.Length);
                GameObject ad = Instantiate(adPrefabs[randomIndex]);
                // get the width and height of the ad chosen
                int adWidth = ad.GetComponent<PopupScript>().getWidth();
                int adHeight = ad.GetComponent<PopupScript>().getHeight();
                // parent the event to the canvas
                ad.transform.SetParent(GameObject.Find("Canvas").transform, false);
                // set the position to a random location INSIDE the 1/4th the camera view to 3/4th the camera view
                // ad.transform.position = new Vector3(UnityEngine.Random.Range(Screen.width / 4, Screen.width * 3 / 4), UnityEngine.Random.Range(Screen.height / 4, Screen.height * 3 / 4), 0);
                // set the position to a random location inside the camera view offset by adWidth and adHeight so it's never offscreen
                ad.transform.position = new Vector3(UnityEngine.Random.Range(adWidth, Screen.width - adWidth), UnityEngine.Random.Range(adHeight, Screen.height - adHeight), 0);
                // ad.transform.position = new Vector3(adWidth / 2, adWidth / 2, 0);
                // log the position
                Debug.Log("screen.width - adWidth is : " + (Screen.width - adWidth));
                Debug.Log("adWidth is :" + (adWidth));
                Debug.Log("screen.width is :" + (Screen.width));
                Debug.Log("screen.height - adHeight is : " + (Screen.height - adHeight));
                Debug.Log(ad.transform.position);
            }
        }
    }
    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
        }
        else
        {
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Optionally persist across scenes
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //subscribe triggerClosedAd to the adclosed event
        AdClosed += spawnRandomAd;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAdClosed()
    {
        Debug.Log("TriggerAdClosed");
        AdClosed?.Invoke();
    }

}
