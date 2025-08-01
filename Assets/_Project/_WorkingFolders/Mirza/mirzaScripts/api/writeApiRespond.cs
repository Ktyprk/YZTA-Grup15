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
                chatUi.SetActive(openClose);

                warningSign.SetActive(false);
            }
            else
            {
                openClose = false;
                chatUi.SetActive(openClose);
            }
            RequesText.text = promptResond;

        }
        else if(!control)
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
            press_E_Scene.SetActive(false);

            control = false;
        }
    }
}
