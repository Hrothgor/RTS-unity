using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffResources : MonoBehaviour
{
    Building building;
    [SerializeField] List<ResourcesType> resourceTypes;
    [SerializeField] Texture2D cursorTexture;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanDrop(ResourcesType type)
    {
        if (resourceTypes.Contains(type))
            return true;
        return false;
    }

    private void OnMouseEnter()
    {
        if (ActorManager.instance.selectedActors.Count >= 0)
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
