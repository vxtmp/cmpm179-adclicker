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
    public int currentAds = 0;


    public void spawnRandomAd()
    {
        // check if null, then choose a random ad and spawn a random ad. loop a random number of times between 1 and 4
        if (adPrefabs != null)
        {
            int numAds = UnityEngine.Random.Range(1, 20);
            for (int i = 0; i < numAds; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, adPrefabs.Length);
                GameObject ad = Instantiate(adPrefabs[randomIndex]);

                // parent the event to the canvas
                RectTransform adRectTransform = ad.GetComponent<RectTransform>();
                // get the width and height of the ad chosen
                float adWidth = adRectTransform.rect.width;
                float adHeight = adRectTransform.rect.height;
                float randomX = UnityEngine.Random.Range(adWidth / 2, Screen.width - adWidth / 2);
                float randomY = UnityEngine.Random.Range(adHeight / 2, Screen.height - adHeight / 2);

                ad.transform.SetParent(GameObject.Find("Canvas").transform, false);
                // set its recttransform position to a random position on the screen offset by width and height

                // log all values for debugging, adwidth, height, randomx and y
                Debug.Log(adWidth);
                Debug.Log(adHeight);
                Debug.Log(randomX);
                Debug.Log(randomY);
                adRectTransform.position = new Vector3(randomX, randomY, 0);
                Debug.Log(adRectTransform.position);
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
