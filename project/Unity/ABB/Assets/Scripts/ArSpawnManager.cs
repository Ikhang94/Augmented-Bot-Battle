using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class ArSpawnManager : MonoBehaviour
{
    public bool hasSpawned = false;
    public GameObject spawnPrefab;
    public ArSurfaceDetect surfaceDetect;
    public ARPlaneManager planeManager;
    public float spawnSize=0.2f;
    public GameObject networkCanvas;
    public XRDeviceSimulator simulator;
    public GameObject camera, cameraOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        simulator = FindFirstObjectByType<XRDeviceSimulator>();
        simulator.enabled = false;
        simulator.enabled = true;
        if (networkCanvas != null)
        {
            networkCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Function that spawns the arena so the players may fight eachother
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void spawnObject()
    {
        if (!hasSpawned)
        spawnPrefab.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
        Destroy(surfaceDetect.prefabPreview);
        //Instantiate(spawnPrefab,surfaceDetect.detectCoord, Quaternion.Euler(new Vector3(0,surfaceDetect.transform.eulerAngles.y,0)));
        Instantiate(spawnPrefab);
        cameraOffset.transform.position-=surfaceDetect.detectCoord;
        cameraOffset.transform.RotateAround(Vector3.zero, Vector3.up, -camera.transform.eulerAngles.y);
        hasSpawned = true;
        destroyPlanes();
        arFunctionality();
        
       
    }

    /// <summary>
    /// Function that destroys the ar planes to save on ressources
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void destroyPlanes()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane != null)
            {
                Destroy(plane.gameObject);
            }
        }
    }
    /// <summary>
    /// Function that sets up the canvases (ui) after scanning
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void arFunctionality()
    {
        planeManager.enabled = !hasSpawned;
        surfaceDetect.enabled = !hasSpawned;
        if (networkCanvas != null)
        {
            networkCanvas.gameObject.SetActive(hasSpawned);
        }
    }

    /// <summary>
    /// Function that grows the arena
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void grow()
    {
        spawnSize += 0.02f;
        surfaceDetect.prefabPreview.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
    }

    /// <summary>
    /// Function that shrinks the arena
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void shrink()
    {
        spawnSize -= 0.02f;
        surfaceDetect.prefabPreview.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
    }
}
