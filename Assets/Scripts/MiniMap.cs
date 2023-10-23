using Photon.Pun;
using UnityEngine;

public class MiniMap : MonoBehaviourPunCallbacks
{
    //kaydırma hızı
    [SerializeField] private float scroolSpeed = 1f;
    //minimumu zoom seviyesi
    [SerializeField] private float minValue = 10f;
    //maksimum zoom seviyesi
    [SerializeField] private float maxValue = 50f;
    //şu an ki zoom seviyesi
    private float currentValue = 20f;

    private void Start()
    {
        if(!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //fare tekerliğinin hareketini kontrol ettiğimiz kısım
        float scroolDelta = Input.GetAxis("Mouse ScrollWheel");
        //tekerleğin hareketine göre değer ayarladığımız kısım
        if(scroolDelta > 0)
        {
            currentValue += scroolSpeed;
        }
        else if(scroolDelta < 0)
        {
            currentValue -= scroolSpeed;
        }
        //sınırlandırma
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        gameObject.GetComponent<Camera>().orthographicSize = currentValue;
    }
}
