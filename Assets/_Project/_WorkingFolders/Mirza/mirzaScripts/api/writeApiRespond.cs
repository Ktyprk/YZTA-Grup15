using UnityEngine;
using TMPro;
public class writeApiRespond : MonoBehaviour
{
    public string promptResond;
    APIManager apiManager;
    public GameObject chatUi;
    public TextMeshProUGUI RequesText;
    private bool openClose = false;
    public GameObject warningSign;
    public GameObject press_E_Scene;
    private bool control = false;
    private void Start()
    {
        openClose = false;
        apiManager = FindAnyObjectByType<APIManager>();
        if(RequesText != null )
        apiManager.sendRequest();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&& control)
        {
            Debug.Log("triggered");
            if (openClose == false)
            {
                openClose = true;
                if(chatUi != null) 
                chatUi.SetActive(openClose);

                warningSign.SetActive(false);
            }
            else
            {
                openClose = false;
                if (chatUi != null)
                    chatUi.SetActive(openClose);
            }
            if(RequesText != null)
            RequesText.text = promptResond;

        }
        else if(!control)
            if (chatUi != null)
                chatUi.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            press_E_Scene.SetActive(true);
            control = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            warningSign.SetActive(false);
            press_E_Scene.SetActive(false);

            control = false;
        }
    }
}
