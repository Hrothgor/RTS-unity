using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Building : Unit
{
    //BuildSystem
    [SerializeField] float height;
    private float originalHeight;
    [SerializeField] int totalWorkToComplete = 100;
    private int currentWork;
    Transform buildingTransform;
    Transform constructionPreview;
    private bool done;
    //Creating Unit
    [HideInInspector] public Unit unitCreating;
    [HideInInspector] public float maxTimeCreating = 0;
    [HideInInspector] public float currentTimeCreating = 0;
    //BuildVisual
    [ColorUsage(true, true)]
    [SerializeField] private Color[] stateColors;
    Cinemachine.CinemachineImpulseSource impulse;
    [SerializeField] private Texture2D cursorTexture;
    override protected void Awake()
    {
        base.Awake();
    }
    override protected void Start()
    {
        base.Start();
        constructionPreview = transform.Find("ConstructionPreview");
        constructionPreview.gameObject.SetActive(true);
        fieldOfView.enableFov(false);
        buildingTransform = transform.GetChild(0);
        impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        currentWork = 0;
        originalHeight = buildingTransform.localPosition.y;
        buildingTransform.localPosition = Vector3.down * height;
    }
    public void CreateUnit(Unit unit)
    {
        if (unitCreating)
            return;
        unitCreating = unit;
        Dictionary<ResourcesType, int> cost = unit.Cost();
        foreach (var resourceCost in cost)
        {
            BuildingManager.instance.currentResources[resourceCost.Key] -= resourceCost.Value;
            BuildingManager.instance.RefreshResources();
        }

        StartCoroutine(StartCreateUnit());

        IEnumerator StartCreateUnit()
        {
            Vector3 buildingPos = transform.position;
            Vector2 randomPosition = Random.insideUnitCircle.normalized * radius;
            buildingPos.x += randomPosition.x;
            buildingPos.z += randomPosition.y;
            while (currentTimeCreating < maxTimeCreating) {
                currentTimeCreating += Time.deltaTime;
                yield return null;
            }
            Instantiate(unitCreating, buildingPos, Quaternion.identity);
            currentTimeCreating = 0;
            unitCreating = null;
        }
    }
    public void Build(int work)
    {
        currentWork += work;
        buildingTransform.localPosition = Vector3.Lerp(Vector3.down * height, new Vector3(0, originalHeight, 0), (float)currentWork / totalWorkToComplete);

        //visual
        buildingTransform.DOComplete();
        buildingTransform.DOShakeScale(.5f, .2f, 10, 90, true);
        BuildingManager.instance.PlayParticle(transform.position);
    }
    public bool IsFinished()
    {
        if (currentWork >= totalWorkToComplete && !done && render)
        {
            done = true;
            constructionPreview.gameObject.SetActive(false);
            fieldOfView.enableFov(true);
            render.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => render.material.DOColor(stateColors[0], "_EmissionColor", .5f));
            BuildingManager.instance.PlayFinishParticle(transform.position);
            if (impulse)
                impulse.GenerateImpulse();
        }
        return currentWork >= totalWorkToComplete;
    }
    override protected void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (render.enabled && !IsFinished())
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    override protected void OnMouseExit()
    {
        base.OnMouseExit();
        if (render.enabled && !IsFinished())
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
