using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ActorManager : MonoBehaviour
{
    private readonly MoveToIndicator moveToIndicatorPointer = new MoveToIndicator();
    [SerializeField] private GameObject moveToIndicatorObject;
    [SerializeField] private float moveToIndicatorScale;
    public static ActorManager instance;
    [SerializeField] LayerMask actorLayer = default;
    [SerializeField] Transform selectionArea = default;
    public List<Actor> allActors = new List<Actor>();
    [HideInInspector] public List<Actor> selectedActors = new List<Actor>();
    Camera mainCamera;
    Vector3 startDrag;
    Vector3 endDrag;
    Vector3 dragCenter;
    Vector3 dragSize;
    bool dragging;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        mainCamera = Camera.main;
        foreach (Actor actor in GetComponentsInChildren<Actor>())
        {
            allActors.Add(actor);
        }

        selectionArea.gameObject.SetActive(false);

        moveToIndicatorPointer.PreparePointer(moveToIndicatorObject, moveToIndicatorScale);
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            dragging = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            startDrag = Utility.MouseToTerrainPosition();
            endDrag = startDrag;
            DeselectActors();
        }
        else if (Input.GetMouseButton(0))
        {
            endDrag = Utility.MouseToTerrainPosition();

            if (Vector3.Distance(startDrag, endDrag) > 1)
            {
                selectionArea.gameObject.SetActive(true);
                dragging = true;
                dragCenter = (startDrag + endDrag) / 2;
                dragSize = (endDrag - startDrag);
                selectionArea.transform.position = dragCenter;
                selectionArea.transform.localScale = dragSize + Vector3.up;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                SelectMultipleActors();
            } else {
                SelectActor();
            }
            dragging = false;
            selectionArea.gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            MoveOrSetTask();
        }
    }

    void MoveOrSetTask()
    {
        if (selectedActors.Count == 0)
            return;

        Collider collider = Utility.CameraRay().collider;
    
        if (collider.CompareTag("Terrain"))
        {
            foreach (Actor actor in selectedActors)
            {
                actor.StopTask();
                actor.SetDestination(Utility.MouseToTerrainPosition(), 0);
            }
            moveToIndicatorPointer.ShowPointer(Utility.MouseToTerrainPosition());
        } else {
            foreach (Actor actor in selectedActors)
                actor.SetTask(collider);
        }
    }
    void SelectMultipleActors()
    {
        DeselectActors();
        dragSize.Set(Mathf.Abs(dragSize.x / 2), 1, Mathf.Abs(dragSize.z / 2));
        RaycastHit[] hits = Physics.BoxCastAll(dragCenter, dragSize, Vector3.up, Quaternion.identity, 0, actorLayer.value);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Actor actor))
            {
                UIManager.instance.SetSelectedUnitMenu(actor);
                selectedActors.Add(actor);
                actor.visualHandler.Select();
            }
        }
    }
    void SelectActor()
    {
        DeselectActors();
        foreach (Actor actor in allActors)
        {
            if (actor.isHover) {
                UIManager.instance.SetSelectedUnitMenu(actor);
                selectedActors.Add(actor);
                actor.visualHandler.Select();
                return;
            }
        } 
    }
    public void DeselectActors()
    {
        foreach (Actor actor in selectedActors)
            actor.visualHandler.Deselect();
        selectedActors.Clear();
    }

    private void OnDrawGizmos()
    {
        Vector3 center = (startDrag + endDrag) / 2;
        Vector3 size = (endDrag - startDrag);
        size.y = 1;
        Gizmos.DrawWireCube(center, size);
    }
}
