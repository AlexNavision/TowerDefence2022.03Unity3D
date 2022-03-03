using UnityEngine;

public class cameramove : MonoBehaviour
 {
    [SerializeField] private float zoomOutMin = 1;
    [SerializeField] private float zoomOutMax = 6;
    [SerializeField] private float SpeedMove = 3f;
    private Vector3 newPosition, touchStart, dragCurrentPosition;

    // Update is called once per frame
    private void Start()
    {
        newPosition = transform.position;
    }
    void OnEnable()
    {
        newPosition = transform.position;
    }
    void Update()
    {
        if (Input.touchCount == 2)
            MobileZoom();
        else Movement();
        zoom(Input.GetAxis("Mouse ScrollWheel") * 3);//для мышки
    }
    void Movement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            dragCurrentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition = new Vector3(transform.position.x + touchStart.x - dragCurrentPosition.x, transform.position.y + touchStart.y - dragCurrentPosition.y, -10);
            if (newPosition.x > 0)
                newPosition.x = 0;
            if (newPosition.x < -40)
                newPosition.x = -40;
            if (newPosition.y > 50)
                newPosition.y = 50;
            if (newPosition.y < -10)
                newPosition.y = -10;
        }
            transform.position = Vector3.Lerp(Camera.main.transform.position, newPosition, Time.deltaTime * SpeedMove);
    }
    void MobileZoom()
    {

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        zoom(difference * 0.001f);
    }
    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}