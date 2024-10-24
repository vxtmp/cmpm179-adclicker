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

    // Start is called before the first frame update
    void Start()
    {
        // set width and height
        width = (int)gameObject.GetComponent<RectTransform>().rect.width;
        height = (int)gameObject.GetComponent<RectTransform>().rect.height;
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);

        }
        else
        {
            Debug.LogError("Button 'x' not found");
        }
        // add an onclick listener to the panel itself
        // gameObject.GetComponent<Button>().onClick.AddListener(OnCloseButtonClicked);
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
}
