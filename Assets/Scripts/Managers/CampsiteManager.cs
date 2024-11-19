using Klaxon.ConversationSystem;
using Klaxon.GOAD;
using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.U2D.Animation;

public class CampsiteManager : MonoBehaviour
{

    public GOAD_Scheduler_NPC campsiteVisitor;
    SpriteResolver chooseSpriteResolver;
    public SpriteLibraryAsset visitorSpriteLibrary;
    public List<CampsiteLocation> campsiteLocations = new List<CampsiteLocation>();
    [HideInInspector]
    public bool hasVisitor;
    [HideInInspector]
    public int lastDay = -1;
    float percentChanceToSpawn = 0.333f;

    GOAD_Scheduler_NPC currentVisitor;
    [HideInInspector]
    public string characterName;
    [HideInInspector]
    public int campIndex = -1;

    private void Start()
    {
        GameEventManager.onDayStateChangeEvent.AddListener(SetCampsiteVisitor);
        GameEventManager.onNewDayEvent.AddListener(RemoveCampsiteVisitor);
        
    }
    private void OnDisable()
    {
        GameEventManager.onDayStateChangeEvent.RemoveListener(SetCampsiteVisitor);
        GameEventManager.onNewDayEvent.RemoveListener(RemoveCampsiteVisitor);

    }

    void RemoveCampsiteVisitor(int day)
    {
        if (!hasVisitor)
            return;

        if (day != lastDay)
            return;

        Destroy(currentVisitor.gameObject);
        hasVisitor = false;
        lastDay = -1;
    }

    void SetCampsiteVisitor()
    {
        // see if a campsiteVisitor can appear (enough campsites open and can happen at all and hasVisitor is false)
        if (hasVisitor)
            return;

        if (RealTimeDayNightCycle.instance.dayState != RealTimeDayNightCycle.DayState.Sunrise)
            return;

        if (Random.Range(0.0f, 1.0f) > percentChanceToSpawn)
            return;


        var availableCamps = ValidCampsites();
        if (availableCamps.Count < 2)
            return;

        StartCoroutine(SetCampsiteVisitorCo(availableCamps));
    }

    IEnumerator SetCampsiteVisitorCo(List<CampsiteLocation> availableCamps)
    {


        // choose a campsite to appear at
        campIndex = Random.Range(0, availableCamps.Count);

        // instantiate visitor
        currentVisitor = Instantiate(campsiteVisitor, availableCamps[campIndex].campsiteNode.transform.position, Quaternion.identity);

        // set visitor home node and bed node
        GOAD_SetCampsiteVisitor csv = currentVisitor.GetComponent<GOAD_SetCampsiteVisitor>();
        csv.SetNodes(availableCamps[campIndex].campsiteNode, availableCamps[campIndex].shelterNode);

        // set visitor spriteResolver
        chooseSpriteResolver = currentVisitor.GetComponentInChildren<SpriteResolver>();
        SpriteSkin spriteSkin = currentVisitor.GetComponentInChildren<SpriteSkin>();
        var spriteNames = visitorSpriteLibrary.GetCategoryLabelNames("Camp");
        int nameIndex = Random.Range(0, spriteNames.Count());
        characterName = spriteNames.ElementAt(nameIndex);
        chooseSpriteResolver.SetCategoryAndLabel("Camp", characterName);
        spriteSkin.autoRebind = false;
        yield return null;
        spriteSkin.autoRebind = true;


        // set visitor name
        SetName();

        // set hasVisitor to true
        hasVisitor = true;

        // set lastDay to be current day plus 3
        lastDay = RealTimeDayNightCycle.instance.currentDayRaw + 3;
        // delete visitor at midnight on lastDay.
    }

    private void SetName()
    {
        InteractableDialogue interactable = currentVisitor.GetComponent<InteractableDialogue>();
        NPC_ConversationSystem convoSystem = currentVisitor.GetComponent<NPC_ConversationSystem>();
        interactable.localizedInteractVerb.TableReference = "NPC-Names";
        interactable.localizedInteractVerb.TableEntryReference = characterName;
        interactable.localizedItemName.TableReference = "NPC-Names";
        interactable.localizedItemName.TableEntryReference = characterName;
        convoSystem.NPC_Name.TableReference = "NPC-Names";
        convoSystem.NPC_Name.TableEntryReference = characterName;
    }

    List<CampsiteLocation> ValidCampsites()
    {
        List<CampsiteLocation> available = new List<CampsiteLocation>();
        foreach (var camp in campsiteLocations)
        {
            if(GOAD_WorldBeliefStates.instance.HasState(camp.campsiteCondition.Condition, camp.campsiteCondition.State))
                available.Add(camp);
        }
        return available;
    }

    public void SetCampsiteFromSave(bool _hasVisitor, int _campIndex, string _characterName, int _lastDay)
    {
        StartCoroutine(SetCampsiteFromSaveCo(_hasVisitor, _campIndex, _characterName, _lastDay));
    }

    IEnumerator SetCampsiteFromSaveCo(bool _hasVisitor, int _campIndex, string _characterName, int _lastDay)
    {
        hasVisitor = _hasVisitor;
        lastDay = _lastDay;
        yield return new WaitForSeconds(1);
        if (hasVisitor)
        {
            campIndex = _campIndex;

            var availableCamps = ValidCampsites();

            // instantiate visitor
            currentVisitor = Instantiate(campsiteVisitor, availableCamps[campIndex].campsiteNode.transform.position, Quaternion.identity);

            // set visitor home node and bed node
            GOAD_SetCampsiteVisitor csv = currentVisitor.GetComponent<GOAD_SetCampsiteVisitor>();
            csv.SetNodes(availableCamps[campIndex].campsiteNode, availableCamps[campIndex].shelterNode);

            // set visitor spriteResolver
            chooseSpriteResolver = currentVisitor.GetComponentInChildren<SpriteResolver>();
            SpriteSkin spriteSkin = currentVisitor.GetComponentInChildren<SpriteSkin>();
            characterName = _characterName;
            chooseSpriteResolver.SetCategoryAndLabel("Camp", characterName);
            spriteSkin.autoRebind = false;
            yield return null;
            spriteSkin.autoRebind = true;

            // set visitor name
            SetName();

        }

    }
}
