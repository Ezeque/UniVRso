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
        if (!dungeonObj)
        {
            dungeonObj = dungeonGenerator.StartCaveCreation();
        }
        else
        {
            dungeonObj.SetActive(true);
        }

        cavePosition = dungeonObj.transform.GetChild(0).Find("InitialSpawnPoint").gameObject.transform.position;
        teleportPoint.cavePosition = cavePosition;
    }

    void Update()
    {
        if (dungeonObj && hasEntered)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, teleportPoint.transform.position);

            if (distanceToPlayer > 10f && dungeonObj.activeSelf) 
            {
                dungeonObj.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !hasEntered)
        {
            hasEntered = true;
            teleportPoint.canEnterCave = true;

            if (dungeonObj && !dungeonObj.activeSelf)
            {
                dungeonObj.SetActive(true); 
            }
            else if (!dungeonObj)
            {
                SetupCave(); 
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("Chegou ao onTriggerExit");
        if (collider.gameObject.tag == "Player")
        {
            hasEntered = false;
            canEnterCave = false;
            dungeonObj = GameObject.Find("Dungeon").gameObject;
            dungeonObj.SetActive(false); 
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
