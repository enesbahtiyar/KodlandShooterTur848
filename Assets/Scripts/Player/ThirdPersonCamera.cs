using UnityEditor;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    //oyuncunun mevcut durumun kontrol edecek olan bir boolean
    public bool isSpectator;
    //serbest kamera hızı
    [SerializeField] float speed = 50f;
    [SerializeField] GameObject player;
    [SerializeField]
    [Range(0.5f, 2f)]
    float mouseSense = 1;
    [SerializeField]
    [Range(-20, -10)]
    int lookUp = -15;
    [SerializeField]
    [Range(15, 25)]
    int lookDown = 20;
    Animator anim;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        float rotateX = Input.GetAxis("Mouse X") * mouseSense;
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense;

        if (!isSpectator)
        {
            Vector3 rotCamera = transform.rotation.eulerAngles;
            Vector3 rotPlayer = player.transform.rotation.eulerAngles;

            rotCamera.x = (rotCamera.x > 180) ? rotCamera.x - 360 : rotCamera.x;
            rotCamera.x = Mathf.Clamp(rotCamera.x, lookUp, lookDown);
            rotCamera.x -= rotateY;

            rotCamera.z = 0;
            rotPlayer.y += rotateX;

            transform.rotation = Quaternion.Euler(rotCamera);
            player.transform.rotation = Quaternion.Euler(rotPlayer);
        }
        else
        {
            //mevcut kamera açısın çek
            Vector3 rotCamera = transform.rotation.eulerAngles;
            //farenin harekine bağlı olarak kameranın yönünü değiştirecem
            rotCamera.x -= rotateY;
            rotCamera.z = 0;
            rotCamera.y += rotateX;
            transform.rotation = Quaternion.Euler(rotCamera);
            //Wasd tuşlarıyla hareket etmeyi
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //kameranın hareket vector3'ünü ayalrlamaca vakti
            Vector3 direction = transform.right * x + transform.forward * z;
            //kameranın konumunu değiştirmece
            transform.position += direction * speed * Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(Cursor.lockState == CursorLockMode.Locked) 
            {
                //cursorın kilidini aç 
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
