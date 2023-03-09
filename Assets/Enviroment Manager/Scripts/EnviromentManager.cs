using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enviroment;

public class EnviromentManager : MonoBehaviour
{
    #region  Singleton

    private static EnviromentManager _instance = null;
    public static EnviromentManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EnviromentManager>();

            return _instance;
        }
    }

    #endregion

    [Header("Directional Light(Sun) From Scene")]
    [SerializeField] private Light mDirectionalLight;

    [Header("Skybox Rotate Speed")]
    [SerializeField] private float mSkyboxRotSpeed;

    [Space(30)]
    [Header("Preload Enviroment Preset")]
    [SerializeField] private EnviromentPreset[] mEnviromentPresets;

    private Dictionary<string, EnviromentPreset> mPreloadEnviromentPresets = new Dictionary<string, EnviromentPreset>();
    private EnviromentPreset? mCurrentPreset = null, mPrevPreset = null; // Current & Prev preset

    private Material mSkyboxMat; // Scene's skybox material
    private float mCurrentSkyboxRot; // Store the current rotation angle
    private Coroutine mCoBlendEnviroment; // Control blend coroutine

    private void Awake()
    {
        // Instance skybox material
        mSkyboxMat = new Material(RenderSettings.skybox);
        RenderSettings.skybox = mSkyboxMat;

        // Get skybox rotation
        mCurrentSkyboxRot = mSkyboxMat.GetFloat("_Rotation");

        // Load preload presets
        foreach (EnviromentPreset preset in mEnviromentPresets)
            mPreloadEnviromentPresets.Add(preset.name, preset);


        // Set "mCurrentPreset" from initial scene options
        EnviromentPreset currentPreset = ScriptableObject.CreateInstance<EnviromentPreset>();
        currentPreset.LoadCurrentSettings();
        mCurrentPreset = currentPreset;
    }

    private void Update()
    {
        mCurrentSkyboxRot += Time.deltaTime * mSkyboxRotSpeed;

        if (mCurrentSkyboxRot > 360f)
            mCurrentSkyboxRot -= 360f;

        mSkyboxMat.SetFloat("_Rotation", mCurrentSkyboxRot);
    }

    public bool TryInvertEnviromentPreset(float duration)
    {
        if (mPrevPreset == null || mCurrentPreset == null)
        {
            Debug.LogWarning("Not Preset Loaded!");
            return false;
        }

        BlendEnviroment(mPrevPreset, duration);

        return true;
    }

    public void BlendEnviroment(string key, float duration)
    {
        this.BlendEnviroment(mPreloadEnviromentPresets[key], duration);
    }

    public void BlendEnviroment(EnviromentPreset preset, float duration)
    {
        if (mCoBlendEnviroment is not null)
            StopCoroutine(mCoBlendEnviroment);

        mCoBlendEnviroment = StartCoroutine(CoBlendEnviroment(preset, duration));
    }

    private IEnumerator CoBlendEnviroment(EnviromentPreset preset, float duration)
    {
        // Store Current & Prev preset
        mPrevPreset = mCurrentPreset;
        mCurrentPreset = preset;

        // Get current option state
        EnviromentPreset curState = ScriptableObject.CreateInstance<EnviromentPreset>();
        curState.LightningIntensityMultiplier = RenderSettings.ambientIntensity;
        curState.ReflectionsIntensityMultiplier = RenderSettings.reflectionIntensity;
        float currentBlendValue = mSkyboxMat.GetFloat("_Blend");
        Color currentSunColor = mDirectionalLight.color;
        float currentSunIntensity = mDirectionalLight.intensity;
        Color currentFogColor = RenderSettings.fogColor;
        float currentFogStart = RenderSettings.fogStartDistance;
        float currentFogEnd = RenderSettings.fogEndDistance;

        // Load blend target textures to skybox mat
        mSkyboxMat.SetTexture("_FrontTex2", preset.SidedSkyboxPreset.FrontTex);
        mSkyboxMat.SetTexture("_BackTex2", preset.SidedSkyboxPreset.BackTex);
        mSkyboxMat.SetTexture("_LeftTex2", preset.SidedSkyboxPreset.LeftTex);
        mSkyboxMat.SetTexture("_RightTex2", preset.SidedSkyboxPreset.RightTex);
        mSkyboxMat.SetTexture("_UpTex2", preset.SidedSkyboxPreset.UpTex);
        mSkyboxMat.SetTexture("_DownTex2", preset.SidedSkyboxPreset.DownTex);

        // Blend processes
        float process = 0f;
        while (process < 1f)
        {
            process += Time.deltaTime / duration;

            // Enviroment Intensity Blend
            RenderSettings.ambientIntensity = Mathf.Lerp(curState.LightningIntensityMultiplier, preset.LightningIntensityMultiplier, process);
            RenderSettings.reflectionIntensity = Mathf.Lerp(curState.ReflectionsIntensityMultiplier, preset.ReflectionsIntensityMultiplier, process);

            // Fog Blend
            RenderSettings.fogColor = Color.Lerp(currentFogColor, preset.FogPreset.FogColor, process);
            RenderSettings.fogStartDistance = Mathf.Lerp(currentFogStart, preset.FogPreset.FogStart, process);
            RenderSettings.fogEndDistance = Mathf.Lerp(currentFogEnd, preset.FogPreset.FogEnd, process);

            // Directional Light(Sun) Blend
            mDirectionalLight.color = Color.Lerp(currentSunColor, preset.SunPreset.SunColor, process);
            mDirectionalLight.intensity = Mathf.Lerp(currentSunIntensity, preset.SunPreset.SunIntensity, process);

            // Skybox Blend
            mSkyboxMat.SetFloat("_Blend", Mathf.Lerp(currentBlendValue, 1.0f, process));

            yield return null;
        }

        // Load blended preset textures to base texture
        mSkyboxMat.SetTexture("_FrontTex", preset.SidedSkyboxPreset.FrontTex);
        mSkyboxMat.SetTexture("_BackTex", preset.SidedSkyboxPreset.BackTex);
        mSkyboxMat.SetTexture("_LeftTex", preset.SidedSkyboxPreset.LeftTex);
        mSkyboxMat.SetTexture("_RightTex", preset.SidedSkyboxPreset.RightTex);
        mSkyboxMat.SetTexture("_UpTex", preset.SidedSkyboxPreset.UpTex);
        mSkyboxMat.SetTexture("_DownTex", preset.SidedSkyboxPreset.DownTex);
        mSkyboxMat.SetFloat("_Blend", 0f);
    }
}