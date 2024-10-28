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

    public bool spawningAdsFlag = true;

    //private int adsClosed = 0;
    //private int currentAds = 0;
    private float cpuUsage = 0;
    private const float CPU_USAGE_PER_AD = 5.0f;

    private int queueLengthBasedOnCPU = 0;

    private float timeSinceLastSpawn = 0.0f;
    private float adInterval = 3.0f; // seconds
    private const float AD_INTERVAL_DECREMENT_PENALTY = 0.5f;
    private const float MINIMUM_AD_INTERVAL = 1.0f;

    private float timeSinceLastCursorUpdate = 0.0f;
    private const float mouseTickRate = 0.0333f; // 30 fps

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
        if (ITManager.Instance.isITSceneActive)
        {
            cpuUsage += 20.0f;
        }
        // cpuUsageTextTag is a TextMeshPro object. change the text inside it
        cpuUsageTextTag.GetComponent<TMPro.TextMeshProUGUI>().text = "CPU Usage: " + cpuUsage + "%";

        // set queueLengthBasedOnCPU = cpuUsage / 10
        queueLengthBasedOnCPU = (int)(cpuUsage / 10);
        //Debug.Log("queueLengthBasedOnCPU: " + queueLengthBasedOnCPU);
        
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
        ITManager.Instance.ITPhoneObject.transform.SetAsLastSibling();
        mouseCursor.transform.SetAsLastSibling();
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

    public void exitGame()
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

    public void raycastClickFromMouseObject()
    {
        Debug.Log("clicked");
        // use graphic raycaster to get the object underneath the mouseCursor object
        UnityEngine.EventSystems.PointerEventData pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pointerData.position = new Vector2 (mouseCursor.transform.position.x - 5, mouseCursor.transform.position.y + 8);
                                            // offset by sprite position
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);
        foreach (UnityEngine.EventSystems.RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);
            if (ITManager.Instance.receivingPlayerInput() == true)
            {
                switch (result.gameObject.name)
                {
                    case "Phone_Button_1":
                        ITManager.Instance.receivePlayerInput('1');
                        break;
                    case "Phone_Button_2":
                        ITManager.Instance.receivePlayerInput('2');
                        break;
                    case "Phone_Button_3":
                        ITManager.Instance.receivePlayerInput('3');
                        break;
                    case "Phone_Button_4":
                        ITManager.Instance.receivePlayerInput('4');
                        break;
                    case "Phone_Button_5":
                        ITManager.Instance.receivePlayerInput('5');
                        break;
                    case "Phone_Button_6":
                        ITManager.Instance.receivePlayerInput('6');
                        break;
                    case "Phone_Button_7":
                        ITManager.Instance.receivePlayerInput('7');
                        break;
                    case "Phone_Button_8":
                        ITManager.Instance.receivePlayerInput('8');
                        break;
                    case "Phone_Button_9":
                        ITManager.Instance.receivePlayerInput('9');
                        break;
                    case "Phone_Button_0":
                        ITManager.Instance.receivePlayerInput('0');
                        break;
                    case "Phone_Button_Pound":
                        ITManager.Instance.receivePlayerInput('#');
                        break;

                }
            }
            if (result.gameObject.name == "ITSupportButton")
            {
                Debug.Log("ITSupportButton clicked");
                // trigger the ITSupportButton event
                //ITSupportButton?.Invoke();
                //spawningAdsFlag = false;
                if (ITManager.Instance.isITSceneActive == false)
                {
                    ITManager.Instance.isITSceneActive = true;
                    nukeAds();
                    ITManager.Instance.startITScene();
                    recalculateCpuUsage();
                }
                break;
            }
            else
            if (result.gameObject.name == "AdBodyButton")
            {
                Debug.Log("AdBodyButton clicked");
                adInterval -= AD_INTERVAL_DECREMENT_PENALTY;
                if (adInterval < MINIMUM_AD_INTERVAL)
                {
                    adInterval = MINIMUM_AD_INTERVAL;
                }
                Debug.Log("adInterval now: " + adInterval);
                break;
            }
            else if (result.gameObject.name == "X")
            {
                Debug.Log("X clicked");
                // remove the ad from the adPool
                adPool.Remove(result.gameObject.transform.parent.gameObject);
                Destroy(result.gameObject.transform.parent.gameObject);
                recalculateCpuUsage();
                break;
            }
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
            //adInterval = UnityEngine.Random.Range(2.0f, 5.0f);
            spawnRandomAd();
        }
        
        timeSinceLastCursorUpdate += Time.deltaTime;
        if (timeSinceLastCursorUpdate >= mouseTickRate)
        {
            timeSinceLastCursorUpdate = 0.0f;
            updCursorLoc();
        }

        // Check for a global left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            raycastClickFromMouseObject();  
        }

    }

    void updCursorLoc()
    {
        //// push current mouseloc to the queue
        Vector2 mouseLoc = Input.mousePosition;

        //Debug.Log("mouseLoc: " + mouseLoc);

        cursorLocQueue.Enqueue(mouseLoc);

        // if the queue is longer than queueLengthBasedOnCPU, pop the front of the queue
        if (cursorLocQueue.Count > queueLengthBasedOnCPU)
        {
            // set mouseCursor canvas object's rect position to the front of the queue
            Vector2 front = cursorLocQueue.Dequeue();
            //Debug.Log ("dequeued front: " + front); 
            mouseCursor.transform.position = new Vector2(front.x + 5, front.y - 8);
        }
    }

    public void nukeAds()
    {
        foreach (GameObject ad in adPool)
        {
            Destroy(ad);
        }
        adPool.Clear();
        //recalculateCpuUsage();
    }


    public void TriggerAdClosed()
    {
        Debug.Log("TriggerAdClosed");
        AdClosed?.Invoke();
    }

    public void spawnRandomAd()
    {
        if (spawningAdsFlag == false) return;
        // check if null, then choose a random ad and spawn a random ad. loop a random number of times between 1 and 4
        if (adPrefabs != null)
        {
            int numAds = UnityEngine.Random.Range(1, 3);
            //Debug.Log("tried to spawn: " + numAds + " ads");
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

}
