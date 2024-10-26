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

    [SerializeField]
    public GameObject cpuUsageTextTag;

    // make a list of the current ads
    public List<GameObject> adPool = new List<GameObject>();

    private int adsClosed = 0;
    private int currentAds = 0;
    private float cpuUsage = 0;

    private float timeSinceLastSpawn = 0.0f;
    private float adInterval = 5.0f; // seconds

    

    private  void recalculateCpuUsage()
    {
        // set cpuUsage = length of adPool * constant
        cpuUsage = adPool.Count * 1.0f;
        if (cpuUsage > 100.0f)
        {
            cpuUsage = 100.0f;
        }
        // cpuUsageTextTag is a TextMeshPro object. change the text inside it
        cpuUsageTextTag.GetComponent<TMPro.TextMeshProUGUI>().text = "CPU Usage: " + cpuUsage + "%";
        
    }
    private void checkCpuOverload()
    {
        if (cpuUsage >= 100)
        {
            Debug.Log("CPU Overload");
        }
    }
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
                adPool.Add(ad);
                ad.transform.SetParent(GameObject.Find("Canvas").transform, false);

                recalculateCpuUsage();
                checkCpuOverload();

                // parent the event to the canvas
                RectTransform adRectTransform = ad.GetComponent<RectTransform>();
                
                // get the width and height of the ad chosen
                float adWidth = adRectTransform.rect.width;
                float adHeight = adRectTransform.rect.height;
                float randomX = UnityEngine.Random.Range(adWidth / 2, Screen.width - adWidth / 2);
                float randomY = UnityEngine.Random.Range(adHeight / 2, Screen.height - adHeight / 2);
                adRectTransform.position = new Vector3(randomX, randomY, 0);
            }
        }

        cpuUsageTextTag.transform.SetAsLastSibling();
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
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= adInterval)
        {
            timeSinceLastSpawn = 0.0f;
            spawnRandomAd();
        }
        
    }

    public void TriggerAdClosed()
    {
        Debug.Log("TriggerAdClosed");
        AdClosed?.Invoke();
    }

}
