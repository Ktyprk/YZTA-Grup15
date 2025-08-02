using UnityEngine;

public class MarketControlScript : MonoBehaviour
{

    [SerializeField] private GameObject MarketUi;
    [SerializeField] private GameObject press_E_Scene;
    private bool open_Close = false;
    private bool canopen = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E)&& canopen)
        {
            close();
        }
    }
    public void close()
    {
        open_Close = !open_Close;
        MarketUi.SetActive(open_Close);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            press_E_Scene.SetActive(true);
            canopen = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            press_E_Scene.SetActive(false);
            canopen = false;



        }
    }
    private void OnTriggerStay(Collider other)
    {
        canopen = true;
    }
}
