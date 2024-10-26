using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// add GetMouseWorldPos namespace

public class PopupScript : MonoBehaviour
{
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button adBodyButton;
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    public int getWidth()
    {
        return width;
    }
    public int getHeight()
    {
        return height;
    }

    private void initExitButton()
    {
        Button foundButton = gameObject.transform.Find("X").GetComponent<Button>();
        if (foundButton == null)
        {
            Debug.LogError("Button 'x' not found");
        }
        else
        {
            closeButton = foundButton;
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    private void initAdBodyButton()
    {
        Button foundButton = gameObject.transform.Find("AdBodyButton").GetComponent<Button>();
        if (foundButton == null)
        {
            Debug.LogError("Button 'AdBodyButton' not found");
        }
        else
        {
            adBodyButton = foundButton;
            adBodyButton.onClick.AddListener(OnAdBodyButtonClicked);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // set width and height
        width = (int)gameObject.GetComponent<RectTransform>().rect.width;
        height = (int)gameObject.GetComponent<RectTransform>().rect.height;
        initExitButton();
        initAdBodyButton();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCloseButtonClicked()
    {
        // display a debug log
        Debug.Log("Close button clicked");
        Destroy(gameObject);
        // trigger the gamemanager event
        GameManager.Instance.TriggerAdClosed();
    }

    private void OnAdBodyButtonClicked()
    {
        Debug.Log("Ad body clicked");
    }
}
