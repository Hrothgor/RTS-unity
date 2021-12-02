using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditor;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] public Transform SkillInfo;
    [SerializeField] Button buttonPrefab;
    List<Button> buttons = new List<Button>();
    EventTrigger eventTrigger = null;
    private Unit selectedUnit;
    [Header("MiddleInfo")]
    public TextMeshProUGUI nameInfo;
    public RawImage iconInfo;
    public TextMeshProUGUI textHPInfo;
    public Image maxHPInfo;
    public Image currentHPInfo;
    [Header("MiddleCreationInfo")]
    public RawImage unitCreationIcon;
    public TextMeshProUGUI nameCreationIcon;
    public Image timeMaxCreationIcon;
    public Image timeCurrentCreationIcon;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DeselectUnitMenu();
    }
    void Update()
    {
        if (selectedUnit) {
            if (selectedUnit is Building)
            {
                Building building = selectedUnit as Building;
                if (building.unitCreating) {
                    showCreationInfo(true);
                    timeCurrentCreationIcon.fillAmount = building.currentTimeCreating / building.maxTimeCreating;
                    nameCreationIcon.text = building.unitCreating.unitName;
                    unitCreationIcon.texture = AssetPreview.GetAssetPreview(building.unitCreating.gameObject);
                } else {
                    showCreationInfo(false);
                }
            }
            currentHPInfo.fillAmount = selectedUnit.damageable.health / selectedUnit.damageable.totalHealth;
            textHPInfo.text = selectedUnit.damageable.health + "/" + selectedUnit.damageable.totalHealth;
        }
    }
    void showCreationInfo(bool value)
    {
        unitCreationIcon.gameObject.SetActive(value);
        nameCreationIcon.gameObject.SetActive(value);
        timeMaxCreationIcon.gameObject.SetActive(value);
        timeCurrentCreationIcon.gameObject.SetActive(value);
    }
    void showInfo(bool value)
    {
        nameInfo.gameObject.SetActive(value);
        iconInfo.gameObject.SetActive(value);
        textHPInfo.gameObject.SetActive(value);
        maxHPInfo.gameObject.SetActive(value);
        currentHPInfo.gameObject.SetActive(value);
    }
    public void DeselectUnitMenu()
    {
        selectedUnit = null;
        foreach (Button button in buttons)
            Destroy(button.gameObject);
        buttons.Clear();
        showInfo(false);
        showCreationInfo(false);
    }
    public void SetSelectedUnitMenu(Unit unit)
    {
        DeselectUnitMenu();
        selectedUnit = unit;

        if (!selectedUnit)
            return;

        //SKILL buttons
        if (unit.skills.Count > 0)
        {
            Button b;
            for (int i = 0; i < unit.skills.Count; i++)
            {
                int index = i;
                b = Instantiate(buttonPrefab, transform);
                buttons.Add(b);
                eventTrigger = buttons[index].GetComponent<UnityEngine.EventSystems.EventTrigger>();
                buttons[index].GetComponent<RawImage>().texture = AssetPreview.GetAssetPreview(unit.skills[i].unitReference.gameObject);
                AddEventTrigger(() => onEnterButton(unit.skills[index].unitReference), EventTriggerType.PointerEnter);
                AddEventTrigger(() => onExitButton(), EventTriggerType.PointerExit);
                b.onClick.AddListener(() => selectedUnit.TriggerSkill(index));
            }
        }
        //MIDDLE Info
        showInfo(true);
        
        nameInfo.text = selectedUnit.unitName;
        iconInfo.texture = AssetPreview.GetAssetPreview(selectedUnit.gameObject);
    }
    private void AddEventTrigger(UnityAction action, EventTriggerType triggerType)
    {
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => action());

        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

        eventTrigger.triggers.Add(entry);
    }
    void onEnterButton(Unit unit)
    {
        SkillInfo.gameObject.SetActive(true);
        string infoString = string.Empty;
        infoString += "<b>" + unit.unitName + "</b>";
        infoString += "\nCost : ";
        Dictionary<ResourcesType, int> resources = unit.Cost();
        Dictionary<ResourcesType, string> resourcesNames = new Dictionary<ResourcesType, string>()
        {
            {ResourcesType.WOOD, "Wood"},
            {ResourcesType.STONE, "Stone"},
            {ResourcesType.FOOD, "Food"},
        };
        foreach (var resource in resources)
        {
            infoString += resourcesNames[resource.Key] + " (" + resource.Value + ") ";
        }
        infoString += "\n\n" + unit.unitDescription;
        SkillInfo.GetComponentInChildren<TextMeshProUGUI>().text = infoString;
    }
    void onExitButton()
    {
        SkillInfo.gameObject.SetActive(false);
    }
}
