using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ResourcesType 
{
    WOOD,
    STONE,
    FOOD
}
// [RequireComponent(typeof(Damageable))]
public class Unit : MonoBehaviour
{
    [SerializeField] public string unitName;
    [SerializeField] public string unitDescription = "";
    [SerializeField] private SerializedDictionary<ResourcesType, int> resourceCost;
    public List<Skill> skills = new List<Skill>();
    [SerializeField] public float radius = 4;
    [HideInInspector] public bool isHover = false;
    public Damageable damageable;
    [HideInInspector] public FieldOfView fieldOfView;
    //HoverVisual
    [HideInInspector] public Renderer render;
    private Color emissionColor;

    virtual protected void Awake()
    {
        render = GetComponentInChildren<Renderer>();
        if (render)
            emissionColor = render.material.GetColor("_EmissionColor");
    }
    virtual protected void Start()
    {   
        fieldOfView = GetComponent<FieldOfView>();
        damageable = GetComponent<Damageable>();
    }
    void Update()
    {
    }
    public bool CanCreate(Dictionary<ResourcesType, int> resources)
    {
        bool canCreate = true;
        foreach (var resource in resources)
        {
            if (resourceCost.TryGetValue(resource.Key, out int value))
            {
                if (resource.Value < value)
                {
                    canCreate = false;
                    return canCreate;
                }
            }
        }
        return canCreate;
    }
    public void TriggerSkill(int index, Unit target = null)
    {
        skills[index].Trigger(this, target);
    }
    public Dictionary<ResourcesType, int> Cost()
    {
        return resourceCost;
    }
    virtual protected void OnMouseEnter()
    {
        isHover = true;
        if (render)
            render.material.SetColor("_EmissionColor", Color.grey);
    }
    virtual protected void OnMouseExit()
    {
        isHover = false;
        if (render)
            render.material.SetColor("_EmissionColor", emissionColor);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
