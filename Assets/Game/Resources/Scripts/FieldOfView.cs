using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Unit unit;
    [HideInInspector] public Transform fovObject;
    [SerializeField] public float fieldOfView = 20;
    [SerializeField] public bool isFov = true;
    //ActiveOrDesactive if in dark
    private Camera _camera;
    private Texture2D currentTexture;
    void Start()
    {
        currentTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        unit = GetComponent<Unit>();
        _camera = GameObject.Find("ExploredAreasCamera").GetComponent<Camera>();
        fovObject = transform.Find("FOV").transform;
        if (fovObject)
            fovObject.localScale = new Vector3(fieldOfView, fieldOfView, 0f);
        enableFov(isFov);
    }
    void Update()
    {
        if (!unit) {
            this.enabled = false;
            return;
        }
        unit.render.enabled = isInVision();
    }
    private bool isInVision()
    {
        if (!_camera)
            return true;

        RenderTexture renderTexture = _camera.targetTexture;

        if (!renderTexture)
            return true;

        Vector3 pixel = _camera.WorldToScreenPoint(transform.position);

        if (pixel.x < 0 || pixel.x > renderTexture.width || pixel.y < 0 || pixel.y > renderTexture.height)
            return true;

        RenderTexture.active = renderTexture;
        currentTexture.ReadPixels(new Rect((int)pixel.x, renderTexture.height - (int)pixel.y, 1, 1), 0, 0);
        RenderTexture.active = null;

        return currentTexture.GetPixel(0, 0) != Color.black;
    }
    public void enableFov(bool enable)
    {
        isFov = enable;
        if (fovObject)
            fovObject.gameObject.SetActive(isFov);
    }
}
