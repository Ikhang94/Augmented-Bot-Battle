using TMPro;
using UnityEngine;

public class ArSurfaceDetect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI debugText;
    public Vector3 detectCoord;
    public GameObject prefabPreview;
    public LayerMask detectedLayer;
    public Camera particleCam, cam;
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame

    /// <summary>
    /// Function that checks if the terrain scan has a valid placement for the arena
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    void Update()
    {
        particleCam.fieldOfView = cam.fieldOfView;
        RaycastHit hit;
        
        // Does the ray intersect any objects in "detectedLayer"
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, detectedLayer))
        {
            debugText.text = "true";
            debugText.color = Color.green;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            detectCoord = hit.point;
            prefabPreview.transform.position = detectCoord;
            prefabPreview.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            debugText.text = "false";
            debugText.color = Color.red;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 100, Color.red);
        }
    }
}
