using UnityEngine;

using TMPro;
public class CointText : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;
    void Start()
    {
        m_Text.text =  coinColllector.Instance.coin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void updateText()
    {
        m_Text.text =  coinColllector.Instance.coin.ToString();

    }
}
