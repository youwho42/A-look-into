using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{

    [Serializable]
    public class TutorialSection
    {
        public GameObject sectionArrow;
        public bool hasShown;
    }

    public bool hasShownTutorial;
    public List<TutorialSection> sections = new List<TutorialSection>();
    [HideInInspector]
    public int currentIndex = 0;
    public Image tutorialButtonCross;

    public void StartTutorial()
    {
        HideAll();
        currentIndex= 0;
        if (!hasShownTutorial)
            SetTutorialArrow(currentIndex);
        tutorialButtonCross.gameObject.SetActive(!hasShownTutorial); 
    }

    public void SetNextTutorialIndex(int currentAttempt)
    {
        if(currentAttempt == sections.Count-1)
        {
            HideAll();
            hasShownTutorial = true;
            tutorialButtonCross.gameObject.SetActive(false);
        }
        if (currentAttempt != currentIndex) 
            return;

        HideAll();

        sections[currentIndex].hasShown = true;

        currentIndex++;
        
        if (currentIndex < sections.Count)
            SetTutorialArrow(currentIndex);
        else 
            hasShownTutorial = true;
    }
    void SetTutorialArrow(int index)
    {
        sections[index].sectionArrow.SetActive(true);
    }

    void HideAll()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].sectionArrow.SetActive(false);
        }
    }

    public void ResetTutorial()
    {
        hasShownTutorial = !hasShownTutorial;
        currentIndex = 0;
        StartTutorial();
    }

    public void SetFromSave(bool hasShown)
    {
        hasShownTutorial = hasShown;
        if (!hasShownTutorial)
            ResetTutorial();
    }
}
