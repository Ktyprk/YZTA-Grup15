using UnityEngine;

public enum MushroomTypes { damage, heal, speed }
public class MushroomBlowUp : MonoBehaviour
{
    
    public MushroomTypes mushroomTypes;

    public Transform visualMushroom; // Scale deðiþecek child objeyi buraya atayacaðýz
    private Vector3 oldPlayerPosition;
    private float oldDistanceBefore;
    [SerializeField] private float positionScaleRate = 0.01f;
    [SerializeField] private float scaleRate = 1f;
    [SerializeField] private int damage_heal_rate = 25;
    private Vector3 scaleChange, scaleChangeTwo, MaxScale, MinScale,positionMinScale;
    [SerializeField] private MushroomColorChange colorChange;
    Collider collider;
    void Start()
    {
        if (mushroomTypes == null)
            mushroomTypes = MushroomTypes.damage;
        colorChange.getMetarials(mushroomTypes);
        oldPlayerPosition = visualMushroom.position;
         scaleChange = new Vector3(-scaleRate , -scaleRate , -scaleRate );
        scaleChangeTwo = new Vector3(scaleRate, scaleRate, scaleRate);
        MaxScale = new Vector3(visualMushroom.localScale.x +15f, visualMushroom.localScale.y + 15f, visualMushroom.localScale.z+15f );
        MinScale = new Vector3(visualMushroom.localScale.x, visualMushroom.localScale.y, visualMushroom.localScale.z);
        positionMinScale = new Vector3(visualMushroom.localPosition.x, visualMushroom.localPosition.y, visualMushroom.localPosition.z);
        collider = gameObject.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            oldDistanceBefore = Vector3.Distance(other.transform.position, transform.position);
            //Debug.Log("Oyuncu girdi");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            //Debug.Log("Oyuncu yakýn: old = " + oldDistanceBefore + " | new = " + distance);

            ScaleVisual(distance);
     
            if (distance <= 2f)
            {
                collider.enabled = false;
                StartCoroutine(colorChange.FlashForOneSecond(oldPlayerPosition, mushroomTypes, damage_heal_rate));
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
            {
                visualMushroom.localScale = MinScale;
                visualMushroom.localPosition = positionMinScale;
            }
            else if (visualMushroom.localScale.x > MinScale.x)
            {
                oldDistanceBefore = newDistance;
                visualMushroom.localScale += scaleChange;
             
            
            }
       
        }
        float scaleT = Mathf.InverseLerp(MinScale.y, MaxScale.y, visualMushroom.localScale.y); // 0..1
        float newY = Mathf.Lerp(positionMinScale.y, positionMinScale.y + 0.5f, scaleT); // örnek: 1  1.5 arasý
        visualMushroom.localPosition = new Vector3(positionMinScale.x, newY, positionMinScale.z);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            oldDistanceBefore = Vector3.Distance(other.transform.position, transform.position);
            
            visualMushroom.localScale = MinScale;
        }
    }
}
