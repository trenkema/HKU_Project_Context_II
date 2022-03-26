using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvents : MonoBehaviour
{
    [SerializeField] GameObject momArrow;
    [SerializeField] GameObject builderArrow;
    [SerializeField] GameObject foresterArrow;
    [SerializeField] GameObject farmerArrow;

    [SerializeField] GameObject[] startColliders;

    bool talkedToMom = false;

    private void OnEnable()
    {
        EventSystemNew<string>.Subscribe(Event_Type.TALK_TO_NPC, TalkToNPC);
    }

    private void OnDisable()
    {
        EventSystemNew<string>.Unsubscribe(Event_Type.TALK_TO_NPC, TalkToNPC);
    }

    private void TalkToNPC(string _npcName)
    {
        switch (_npcName)
        {
            case "mom":
                if (!talkedToMom)
                {
                    EventSystemNew.RaiseEvent(Event_Type.START_GAME);

                    foreach (var collider in startColliders)
                    {
                        collider.SetActive(false);
                    }

                    talkedToMom = true;

                    momArrow.SetActive(false);
                    builderArrow.SetActive(true);
                    foresterArrow.SetActive(true);
                    farmerArrow.SetActive(true);
                }
                break;
        }
    }
}
