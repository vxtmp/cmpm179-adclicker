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
    private GameObject cpuUsageTextTag;
    [SerializeField]
    private GameObject mouseCursor;

    // make a list of the current ads
    public List<GameObject> adPool = new List<GameObject>();

    private int adsClosed = 0;
    private int currentAds = 0;
    private float cpuUsage = 0;
    private const float CPU_USAGE_PER_AD = 1.0f;

    private int queueLengthBasedOnCPU = 0;

    private float timeSinceLastSpawn = 0.0f;
    private float adInterval = 5.0f; // seconds

    private float timeSinceLastCursorUpdate = 0.0f;
    private const float mouseTickRate = 0.0167f; // 60 fps

    // create a queue of vector2 points storing the cursor location
    private Queue<Vector2> cursorLocQueue = new Queue<Vector2>();


    

    private  void recalculateCpuUsage()
    {
        // set cpuUsage = length of adPool * constant
        cpuUsage = adPool.Count * CPU_USAGE_PER_AD;
        if (cpuUsage > 100.0f)
        {
            cpuUsage = 100.0f;
        }
        // cpuUsageTextTag is a TextMeshPro object. change the text inside it
        cpuUsageTextTag.GetComponent<TMPro.TextMeshProUGUI>().text = "CPU Usage: " + cpuUsage + "%";

        // set queueLengthBasedOnCPU = cpuUsage / 10
        queueLengthBasedOnCPU = (int)(cpuUsage / 10);
        Debug.Log("queueLengthBasedOnCPU: " + queueLengthBasedOnCPU);
        
    }
    private void checkCpuOverload()
    {
        if (cpuUsage >= 100)
        {
            Debug.Log("CPU Overload");
        }
    }

    private void setRenderPriorityCanvasObjects()
    {

        cpuUsageTextTag.transform.SetAsLastSibling();
        mouseCursor.transform.SetAsLastSibling();
    }

    public void spawnRandomAd()
    {
        // check if null, then choose a random ad and spawn a random ad. loop a random number of times between 1 and 4
        if (adPrefabs != null)
        {
            int numAds = UnityEngine.Random.Range(1, 2) + UnityEngine.Random.Range(1, 2) - 1;
            Debug.Log("tried to spawn: " + numAds + " ads");
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
        setRenderPriorityCanvasObjects();

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

    private void exitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
        #else
                Application.Quit();
        #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        //subscribe triggerClosedAd to the adclosed event
        AdClosed += spawnRandomAd;

        //Cursor.visible = false;
        // null check cpu tag and mouse cursor
        if (cpuUsageTextTag == null)
        {
            Debug.LogError("CPU Usage Text Tag not found");
            exitGame();
        }
        if (mouseCursor == null)
        {
            Debug.LogError("Mouse Cursor not found");
            UnityEditor.EditorApplication.isPlaying = false;
            exitGame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= adInterval)
        {
            timeSinceLastSpawn = 0.0f;
            // set adInterval to a random number between 2 and 5
            adInterval = UnityEngine.Random.Range(2.0f, 5.0f);
            spawnRandomAd();
        }
        
        timeSinceLastCursorUpdate += Time.deltaTime;
        if (timeSinceLastCursorUpdate >= mouseTickRate)
        {
            timeSinceLastCursorUpdate = 0.0f;
            updCursorLoc();
        }
        
    }

    void updCursorLoc()
    {
        //// push current mouseloc to the queue
        Vector2 mouseLoc = Input.mousePosition;

        Debug.Log("mouseLoc: " + mouseLoc);

        cursorLocQueue.Enqueue(mouseLoc);


        // if the queue is longer than queueLengthBasedOnCPU, pop the front of the queue
        if (cursorLocQueue.Count > queueLengthBasedOnCPU)
        {
            // set mouseCursor canvas object's rect position to the front of the queue
            Vector2 front = cursorLocQueue.Dequeue();
            Debug.Log ("dequeued front: " + front); 
            mouseCursor.transform.position = front;


        }
    }

    public void TriggerAdClosed()
    {
        Debug.Log("TriggerAdClosed");
        AdClosed?.Invoke();
    }

}
