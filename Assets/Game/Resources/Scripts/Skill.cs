using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    INSTANTIATE_CHARACTER,
    INSTANTIATE_BUILDING
}
[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public SkillType type;
    public Unit unitReference;
    public float castTime;
    public float cooldown;

    public AudioClip onStartSound;
    public AudioClip onEndSound;
    public void Trigger(Unit source, Unit target = null)
    {
        switch (type)
        {
            case SkillType.INSTANTIATE_CHARACTER:
                {
                    if (source is Building)
                    {
                        Building building = source as Building;
                        building.maxTimeCreating = castTime;
                        building.CreateUnit(unitReference);
                    }
                }
                break;
            case SkillType.INSTANTIATE_BUILDING:
                {
                    Building building = unitReference as Building;
                    BuildingManager.instance.SelectBuilding(building);
                }
                break;
            default:
                break;
        }
    }
}
