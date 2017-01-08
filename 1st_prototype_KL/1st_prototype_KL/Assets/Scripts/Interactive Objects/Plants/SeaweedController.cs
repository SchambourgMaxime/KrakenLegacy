using UnityEngine;
using System.Collections;

public class SeaweedController : MonoBehaviour {

    public Texture texturelHot;
    public Texture textureWarm;
    public Texture textureCold;


    private Texture currentTexture;

    public int TemperatureColdToWarm;
    public int TemperatureWarmToHot;

    private const int HOT = 3;
    //private const int WARM = 2;
    //private const int COLD = 1;

    // [StatCold, StatWarm, StatHot]
    public uint[] strengthGain = { 0, 0, 0 };
    public uint[] defenseGain = { 0, 0, 0 };
    public uint[] speedGain = { 0, 0, 0 };
    public uint[] healthRecovery = { 0, 0, 0 };

    public void Start()
    {
    }

    public void ChangeTemperature(int t)
    {
        Texture[] m = { textureCold, textureWarm, texturelHot };

        int temperatureInterval = GetTemperatureCategory(t);

        currentTexture = m[temperatureInterval];

        Renderer r = transform.GetComponentInChildren<Renderer>();
        r.material.mainTexture = currentTexture;

        FoodController fc = GetComponent<FoodController>();
        fc.SetStats(strengthGain[temperatureInterval],
                    defenseGain[temperatureInterval], 
                    speedGain[temperatureInterval], 
                    healthRecovery[temperatureInterval]);
    }

    private int GetTemperatureCategory(int t)
    {
        int ret = 0;

        if (t > TemperatureWarmToHot)
            ret = 2;
        else if (t > TemperatureColdToWarm && t <= TemperatureWarmToHot)
            ret = 1;
        else
            ret = 0;

        return ret;
    }
}
