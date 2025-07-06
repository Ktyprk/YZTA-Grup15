using UnityEngine;

public class MushroomBlowUp : MonoBehaviour
{
    public Transform visualMushroom; // Scale deðiþecek child objeyi buraya atayacaðýz
    private Vector3 oldPlayerPosition;
    private float oldDistanceBefore;
    private Vector3 scaleChange, scaleChangeTwo, MaxScale, MinScale;
    [SerializeField] private MushroomColorChange colorChange;
    Collider collider;
    void Start()
    {
        scaleChange = new Vector3(-0.02f, -0.02f, -0.02f);
        scaleChangeTwo = new Vector3(0.02f, 0.02f, 0.02f);
        MaxScale = new Vector3(2f, 2f, 2f);
        MinScale = new Vector3(1f, 1f, 1f);
        collider = gameObject.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            oldDistanceBefore = Vector3.Distance(other.transform.position, transform.position);
            Debug.Log("Oyuncu girdi");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            Debug.Log("Oyuncu yakýn: old = " + oldDistanceBefore + " | new = " + distance);

            ScaleVisual(distance);
     
            if (distance <= 1.5f)
            {
                collider.enabled = false;
                StartCoroutine(colorChange.FlashForOneSecond());
            }
        }
    }

    private void ScaleVisual(float newDistance)
    {
        if (oldDistanceBefore > newDistance)
        {
            visualMushroom.localScale += scaleChangeTwo;
            oldDistanceBefore = newDistance;
            if (visualMushroom.localScale.x > MaxScale.x)
            {
                visualMushroom.localScale = MaxScale;
            }
            else if (visualMushroom.localScale.x < MaxScale.x)
            {
                oldDistanceBefore = newDistance;
                visualMushroom.localScale += scaleChangeTwo;
            }

        }
        else if (oldDistanceBefore < newDistance)
        {
            oldDistanceBefore = newDistance;
            visualMushroom.localScale += scaleChange;
            if (visualMushroom.localScale.x < MinScale.x)
                visualMushroom.localScale = MinScale;
            else if (visualMushroom.localScale.x < MinScale.x)
            {
                oldDistanceBefore = newDistance;
                visualMushroom.localScale += scaleChange;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            oldDistanceBefore = Vector3.Distance(other.transform.position, transform.position);
            Debug.Log("Oyuncu girdi");
            visualMushroom.localScale = MinScale;
        }
    }
}
