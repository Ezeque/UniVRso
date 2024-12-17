using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator; 
    private TeleportPointController teleportPoint;
    private bool hasEntered = false;
    public bool canEnterCave;          
    public Vector3 cavePosition;
    public GameObject dungeonObj;

    private XRIDefaultInputActions inputActions;

    void Awake()
    {
        dungeonGenerator = GameObject.Find("DungeonGenerator").GetComponent<DungeonGenerator>();
        inputActions = new XRIDefaultInputActions();
        teleportPoint = gameObject.GetNamedChild("TeleportPoint").GetComponent<TeleportPointController>();
    }

    void SetupCave()
    {
        dungeonObj = dungeonGenerator.StartCaveCreation();
        cavePosition = dungeonObj.transform.Find("InitialSpawnPoint").gameObject.transform.position;
        teleportPoint.cavePosition = cavePosition;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !hasEntered && !dungeonObj)
        {
            hasEntered = true;
            teleportPoint.canEnterCave = true;
            SetupCave();  
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            hasEntered = false;
            canEnterCave = false;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
