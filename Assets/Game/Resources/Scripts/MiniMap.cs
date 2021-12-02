using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class MiniMap : MonoBehaviour, IPointerDownHandler
{
    public Vector2 terrainSize;
    private RectTransform uiRect;
    [SerializeField] private RectTransform miniMapPreviewCam;

    private void Start()
    {
        uiRect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        Vector3 middle = Utility.MiddleOfScreenPointToWorld();
        Vector3[] corners = ScreenCornersToWorldPoints();
        Vector3[] uiCorners = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float diff = Mathf.Abs(corners[i].x - middle.x);
            corners[i].x += diff;
            corners[i].z += diff;
            uiCorners[i] = new Vector3 (
                corners[i].x * uiRect.rect.width / terrainSize.x,
                corners[i].z * uiRect.rect.height / terrainSize.y,
                0f);
        }
        float w = uiCorners[1].x - uiCorners[0].x;
        float h = uiCorners[2].y - uiCorners[0].y;
        miniMapPreviewCam.anchoredPosition = uiCorners[0];
        miniMapPreviewCam.sizeDelta = new Vector2(w, h);
    }
    private Vector3[] ScreenCornersToWorldPoints()
    {
        Vector3[] corners = new Vector3[4];
        RaycastHit hit;
        for (int i = 0; i < 4; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector2((i % 2) * Screen.width, (int)(i / 2) * Screen.height));
            if (Physics.Raycast(
                    ray,
                    out hit,
                    1000f,
                    LayerMask.GetMask("Level")
                )) corners[i] = hit.point;
        }
        return corners;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 clickPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out clickPosition)) 
            return;
        clickPosition -= uiRect.rect.position;
        Vector3 realPos = new Vector3(
            clickPosition.x / uiRect.rect.width * terrainSize.x,
            200f,
            clickPosition.y / uiRect.rect.height * terrainSize.y
        );
        CameraController.instance.transform.position = realPos;
    }
}
