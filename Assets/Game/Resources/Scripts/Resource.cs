using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Resource : MonoBehaviour
{
    [SerializeField] public Unit unit;
    [SerializeField] private ResourcesType resourceType;
    [SerializeField] private Texture2D cursorTexture;
    private void Start()
    {
        unit = GetComponent<Unit>();
    }
    private void Update()
    {
    }
    public void HitResource(int damage, Villager villager)
    {
        int healthLost = unit.damageable.health;
        unit.damageable.Hit(damage);
        healthLost -= unit.damageable.health;
        villager.IncreaseResource(resourceType, healthLost);
        //visual
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
    }
    private void OnMouseEnter()
    {
        if (unit.render.enabled)
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
    private void OnMouseExit()
    {
        if (unit.render.enabled)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
