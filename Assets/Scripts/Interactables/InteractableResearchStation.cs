using QuantumTek.QuantumInventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Klaxon.Interactable
{
    public class InteractableResearchStation : Interactable
    {

        bool isOpen;
        public SpriteRenderer researchGlow;
        public SpriteRenderer researchItem;
        public Light2D researchLight;
        //ResearchStationDisplayUI researchDisplay;
        public override void Start()
        {
            base.Start();
            HideResearchItem();
            //researchDisplay = ResearchStationDisplayUI.instance;
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);
            if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.None)
            {
                if (PlayerInformation.instance.uiScreenVisible || PlayerInformation.instance.playerInput.isInUI)
                    return;
                
                OpenResearch();
            }
            else if (UIScreenManager.instance.GetCurrentUI() == UIScreenType.ResearchStationUI)
            {
                CloseResearch();
            }
        }

        private void OpenResearch()
        {
            if (UIScreenManager.instance.DisplayIngameUI(UIScreenType.ResearchStationUI, true))
                ResearchStationDisplayUI.instance.ShowUI(this);
        }

        private void CloseResearch()
        {
            UIScreenManager.instance.HideScreenUI();
            ResearchStationDisplayUI.instance.HideUI();
        }

        public void ShowResearchItem(QI_ItemData item)
        {
            researchGlow.gameObject.SetActive(true);
            researchItem.sprite = item.Icon;
            researchItem.gameObject.SetActive(true);
            researchLight.gameObject.SetActive(true);
            StartCoroutine("FlickerSignLight");
        }
        public void HideResearchItem()
        {
            researchGlow.gameObject.SetActive(false);
            researchItem.sprite = null;
            researchItem.gameObject.SetActive(false);
            researchLight.gameObject.SetActive(false);

            StopAllCoroutines();
        }
        IEnumerator FlickerSignLight()
        {
            float timeBetweenFlickers = Random.Range(0.005f, 5f);
            while (true)
            {
                researchGlow.enabled = true;
                researchItem.enabled = true;
                researchLight.enabled = true;
                yield return new WaitForSeconds(timeBetweenFlickers);
                timeBetweenFlickers = Random.Range(0.005f, 0.07f);
                researchGlow.enabled = false;
                researchItem.enabled = false;
                researchLight.enabled = false;
                yield return new WaitForSeconds(timeBetweenFlickers);
                timeBetweenFlickers = Random.Range(0.005f, 5f);

                yield return null;

            }
        }
    }

}