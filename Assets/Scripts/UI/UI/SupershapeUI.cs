using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SupershapeUI : MonoBehaviour
{

    public Slider mSlider;
    public Slider zSlider;
    public Slider sSlider;
    public Slider aSlider;
    public Slider bSlider;
    public float totalNMax = 55;
    UIScreen screen;
    CreateParticleSuperShape supershape;
    public List<Slider> Ns = new List<Slider>();
    List<float> lastNs = new List<float>();
    private void Start()
    {
        supershape = CreateParticleSuperShape.instance;
        screen = GetComponent<UIScreen>();
        screen.SetScreenType(UIScreenType.SupershapeUI);
        SetSliders();
        gameObject.SetActive(false);
        
    }
    public void ResetSupershape()
    {
        supershape.SetTotalM(18);
        supershape.particleLayers[0].zBase = 0.35f;
        supershape.particleLayers[0].zVariance = 0.0f;
        supershape.particleLayers[0].a = 3;
        supershape.particleLayers[0].b = 13;

        supershape.particleLayers[0].n1 = 20;
        supershape.particleLayers[0].n2 = 20;
        supershape.particleLayers[0].n3 = 15;

        SetSliders();

    }

    void SetSliders()
    {
        lastNs.Clear();
        mSlider.value = supershape.GetTotalM();
        zSlider.value = supershape.particleLayers[0].zBase;
        sSlider.value = supershape.particleLayers[0].zVariance;
        aSlider.value = supershape.particleLayers[0].a;
        bSlider.value = supershape.particleLayers[0].b;
        Ns[0].value = supershape.particleLayers[0].n1;
        Ns[1].value = supershape.particleLayers[0].n2;
        Ns[2].value = supershape.particleLayers[0].n3;
        lastNs.Add(Ns[0].value);
        lastNs.Add(Ns[1].value);
        lastNs.Add(Ns[2].value);

    }

    public void SetSupershapeM()
    {
        supershape.SetTotalM((int)mSlider.value);
    }
    public void SetSupershapeZ()
    {
        supershape.particleLayers[0].zBase = zSlider.value;
    }
    public void SetSupershapeS()
    {
        supershape.particleLayers[0].zVariance = sSlider.value;
    }
    public void SetSupershapeA()
    {
        supershape.particleLayers[0].a = aSlider.value;
    }
    public void SetSupershapeB()
    {
        supershape.particleLayers[0].b = bSlider.value;
    }

    void Update()
    {
        for (int i = 0; i < Ns.Count; i++)
        {
            if (Ns[i].value != lastNs[i])
            {
                SetNs(i);
                lastNs[i] = Ns[i].value;
            }

        }

    }
    void SetNs(int index)
    {
        float sum = GetCurrentNTotal();

        if (sum > totalNMax)
        {
            float over = sum - totalNMax;
            for (int i = 0; i < Ns.Count; i++)
            {
                if (i == index)
                    continue;

                Ns[i].value -= over * 0.5f;

                if (Ns[index].value >= totalNMax)
                    Ns[i].value = 0;

            }
        }
        supershape.particleLayers[0].n1 = Ns[0].value;
        supershape.particleLayers[0].n2 = Ns[1].value;
        supershape.particleLayers[0].n3 = Ns[2].value;
    }

    private float GetCurrentNTotal()
    {
        float sum = 0;
        for (int i = 0; i < Ns.Count; i++)
            sum += Ns[i].value;
        return sum;
    }

}
