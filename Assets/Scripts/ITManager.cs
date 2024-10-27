using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ITManager : MonoBehaviour
{
    public bool isITSceneActive = false;
    // Singleton instance
    public static ITManager Instance { get; private set; }
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

    
    public const float TXTSPEED_SECONDS_PER_CHAR = 0.05f; // seconds per character

    // Define a simple class to hold the dialogue string and float
    // dialogue.sentence, dialogue.extra_duration
    public class Dialogue
    {
        public string sentence;
        public float extra_duration;

        public Dialogue(string dialogueString, float dialogueFloat)
        {
            this.sentence = dialogueString;
            this.extra_duration = dialogueFloat;
        }
    }

    [SerializeField]
    public GameObject ITPhoneObject;
    [SerializeField]
    private GameObject dialogueBoxTextTag;
    private bool displayingALine = false;

    private bool receivingPlayerInputFlag = false;

    private string receivedPlayerInput = "";
    private const string ANSWER_SEQUENCE = "7567234198";

    private int failedAttempts = 0;

    public void resetPlayerInput()
    {
        receivedPlayerInput = "";
    }

    private void runGameOver()
    {
        Debug.Log("GAME OVER");
        StartCoroutine(coroutineDisplayLoop(success_message));
        // switch to game over scene.
    }

    private void runVictory()
    {
        Debug.Log("VICTORY");
        // switch to scene and run bsod sequence.
    }

    private IEnumerator failedAttempt()
    {
        receivingPlayerInputFlag = false;
        resetPlayerInput();
        if (failedAttempts <= 2)
        {
            yield return StartCoroutine(coroutineDisplayLine(miss_messages[failedAttempts]));
            failedAttempts++;
            yield return StartCoroutine(replayNumberSequence());
        }
        else
        {
            yield return StartCoroutine(coroutineDisplayLoop(loss_messages));
            runGameOver();    
        }
    }

    private void verifyCorrectSequence()
    {
        if (receivedPlayerInput == ANSWER_SEQUENCE)
        {
            runVictory();
        }
        // for current length of receivedPlayerInput, check each char against the corresponding char in answer
        for (int i = 0; i < receivedPlayerInput.Length; i++)
        {
            if (receivedPlayerInput[i] != ANSWER_SEQUENCE[i])
            {
                // if any char is incorrect, display a miss message and reset player input
                StartCoroutine(failedAttempt());
                return;
            }
        }
    }

    private IEnumerator replayNumberSequence()
    {
        receivingPlayerInputFlag = false;
        yield return StartCoroutine(coroutineDisplayLoop(number_sequence_a));
        receivingPlayerInputFlag = true;
        
    }
    public void receivePlayerInput(char input)
    {
        if (input == '#')
        {
            // replay the sequence and reset player input
            failedAttempts++;
            receivedPlayerInput = "";
            StartCoroutine(replayNumberSequence());
        } else if (input == null)
        {
            Debug.Log("player input null received.");
            return;
        } else
        {
            receivedPlayerInput += input;
            dialogueBoxTextTag.GetComponent<TMPro.TextMeshProUGUI>().text = receivedPlayerInput;
            verifyCorrectSequence();
        }
    }

    private Dialogue[] initialDialogueStrings = new Dialogue[] {
        new Dialogue("Hello, this is Windows 98 tech support.", 2.0f),
        new Dialogue("We see you're having trouble with your personal computer, ", 1.0f),
        new Dialogue("and we're here to help.", 2.0f),
        new Dialogue("Please wait while we connect you to a technician.", 3.0f),
        new Dialogue(".", 1.0f),
        new Dialogue("..", 1.0f),
        new Dialogue("...", 1.0f),
        new Dialogue(".", 1.0f),
        new Dialogue("..", 1.0f),
        new Dialogue("...", 1.0f),
    };

    private Dialogue[] technician_a = new Dialogue[]
    {
        new Dialogue("Hi, this is Matt. How might I assist you today?", 2.0f),
        new Dialogue("You: . . .", 3.0f),
        new Dialogue("I see.", 0.7f),
        new Dialogue("I've identified the problem and I'll do my best to help you out.", 2.0f),
        new Dialogue("Please follow my instructions carefully.", 2.0f),
        new Dialogue("When you are ready, I will display a sequence of numbers.", 2.0f),
        new Dialogue("Please repeat those numbers back to me using the buttons on your phone.", 2.0f),
        new Dialogue("Ready?", 1.0f),
        new Dialogue("Here we go.", 1.0f),
    };

    private Dialogue[] number_sequence_a = new Dialogue[] // 7567234198
    {
        new Dialogue("7", 0.5f),
        new Dialogue("5", 0.5f),
        new Dialogue("6", 0.5f),
        new Dialogue("7", 0.5f),
        new Dialogue("2", 0.5f),
        new Dialogue("3", 0.5f),
        new Dialogue("4", 0.5f),
        new Dialogue("1", 0.5f),
        new Dialogue("9", 0.5f),
        new Dialogue("8", 0.5f),
        new Dialogue("Did you get that? Press # if you need to hear it again.", 2.0f),
    };

    private Dialogue[] miss_messages = new Dialogue[]
    {
        new Dialogue("That was the wrong sequence, but let's try that again. Ready?", 3.0f),
        new Dialogue("Valued customer, please pay close attention and let's get it right this time.", 2.0f),
        new Dialogue("One more time...", 1.0f), // third times the charm.
    };

    private Dialogue[] loss_messages = new Dialogue[]
    {
        new Dialogue("No! FUCK. How many times do I have to repeat myself!?", 1.0f), // hang up dialogue pt1
        new Dialogue("You know what? ", 1.0f), // pt2
        new Dialogue("I'm done.", 0.7f),
        new Dialogue("You have a nice day. *clack* ", 2.0f),
        new Dialogue(" *dial tone*", 4.0f), // pt3
    };

    private Dialogue[] success_message = new Dialogue[]
    {
        new Dialogue("Well done!", 2.0f),
        new Dialogue("We'll take care of a few things on our end first, ", 1.0f),
        new Dialogue("then once you update and restart,", 1.0f),
        new Dialogue("things should be good to go!", 2.0f),
        new Dialogue("Thank you for being a valued Windows customer, and have a great day.", 3.0f)
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator coroutineDisplayLine(Dialogue line)
    {
        displayingALine = true;
        string tmp = "";

        for (int i = 0; i < line.sentence.Length; i++)
        {
            tmp += line.sentence[i];
            dialogueBoxTextTag.GetComponent<TMPro.TextMeshProUGUI>().text = tmp;
            yield return new WaitForSeconds(TXTSPEED_SECONDS_PER_CHAR);
        }

        yield return new WaitForSeconds(line.extra_duration);
        displayingALine = false;
    }
    public IEnumerator coroutineDisplayLoop(Dialogue[] dialogueSet)
    {
        //dialogueBoxTextTag is a tmp text mesh pro object. change the text inside of it
       for (int i = 0;i < dialogueSet.Length;i++)
       {
            // Wait until displayOneLine completes for each dialogue
            yield return StartCoroutine(coroutineDisplayLine(dialogueSet[i]));
       }
    }


    public void startITScene()
    {
        Debug.Log("IT scene started");
        // make the ITPhone object visible.
        ITPhoneObject.SetActive(true);
        // Start a coroutine to display initialDialogueStrings, and once it's done, display technician_a
        StartCoroutine(displaySequences());
    }

    public bool receivingPlayerInput()
    {
        return receivingPlayerInputFlag;
    }

    private IEnumerator displaySequences()
    {
        // Wait for initialDialogueStrings to complete
        yield return StartCoroutine(coroutineDisplayLoop(initialDialogueStrings));

        // After initialDialogueStrings completes, start technician_a
        yield return StartCoroutine(coroutineDisplayLoop(technician_a));

        yield return StartCoroutine(coroutineDisplayLoop(number_sequence_a));

        receivingPlayerInputFlag = true;
    }
}
