using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Enviroment
{
    [System.Serializable]
    public class SunPreset
    {
        [Header("Controls brightness of directional light")]
        [SerializeField] public float SunIntensity;

        [Header("Controls directional light color")]
        [SerializeField] public Color SunColor;
    }

    [System.Serializable]
    public class FogPreset
    {
        [Header("Controls distance of fog")]
        [SerializeField] public float FogStart, FogEnd;

        [Header("Controls fog color")]
        [SerializeField] public Color FogColor;
    }

    [System.Serializable]
    public class SidedSkyboxPreset
    {
        [Header("Front [+Z]")]
        [SerializeField] public Texture FrontTex;

        [Header("Front [-Z]")]
        [SerializeField] public Texture BackTex;

        [Header("Left [+X]")]
        [SerializeField] public Texture LeftTex;

        [Header("Right [-Z]")]
        [SerializeField] public Texture RightTex;

        [Header("Up [+Y]")]
        [SerializeField] public Texture UpTex;

        [Header("Down [-Y]")]
        [SerializeField] public Texture DownTex;
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "EnviromentPreset", menuName = "Preset/EnviromentPreset")]
    public class EnviromentPreset : ScriptableObject
    {
        [Header("Contains the 6 side texture of the Skybox to be replaced")]
        [SerializeField] public SidedSkyboxPreset SidedSkyboxPreset = null;

        [Header("Controls fog option")]
        [SerializeField] public FogPreset FogPreset = null;

        [Header("Controls directional light(sun) option")]
        [SerializeField] public SunPreset SunPreset = null;

        [Header("Controls the brightness of the skybox lighting in the Scene")]
        [Range(0f, 8f)][SerializeField] public float LightningIntensityMultiplier = 1.0f;

        [Header("Controls how much the skybox affects reflections in the Scene.")]
        [Range(0f, 1f)][SerializeField] public float ReflectionsIntensityMultiplier = 1.0f;

        public void LoadCurrentSettings()
        {
            try
            {
                // get render setting values
                this.LightningIntensityMultiplier = RenderSettings.ambientIntensity;
                this.ReflectionsIntensityMultiplier = RenderSettings.reflectionIntensity;

                // try get fog options
                if (RenderSettings.fogMode != FogMode.Linear)
                    Debug.LogWarning("FogPreset was skipped. Fog mode is not Linear");
                else
                {
                    FogPreset fogPreset = new FogPreset();

                    fogPreset.FogStart = RenderSettings.fogStartDistance;
                    fogPreset.FogEnd = RenderSettings.fogEndDistance;
                    fogPreset.FogColor = RenderSettings.fogColor;

                    this.FogPreset = fogPreset;
                }

                // try find directional light in scene via named "Directional Light"
                GameObject? directionalLightGo = GameObject.Find("Directional Light");
                if (directionalLightGo is null)
                    Debug.LogWarning("SunPreset was skipped. Cannot find \"Directional Light\" in scene");
                else
                {
                    Light directionalLight = directionalLightGo.GetComponent<Light>();
                    if (directionalLight is null)
                        Debug.LogWarning("SunPreset was skipped. Cannot find light component in \"Directional Light\" object");
                    else
                    {
                        SunPreset sunPreset = new SunPreset();
                        sunPreset.SunIntensity = directionalLight.intensity;

                        if (directionalLight.useColorTemperature)
                            Debug.LogWarning("SunPreset.SunColor was skipped. Light.useColorTemperature is true");
                        else
                            sunPreset.SunColor = directionalLight.color;

                        this.SunPreset = sunPreset;
                    }
                }

                // try get textures from skybox's textures from scene
                Material mat = RenderSettings.skybox;
                SidedSkyboxPreset sidedSkyboxPreset = new SidedSkyboxPreset();
                try
                {
                    sidedSkyboxPreset.FrontTex = mat.GetTexture("_FrontTex");
                    sidedSkyboxPreset.BackTex = mat.GetTexture("_BackTex");
                    sidedSkyboxPreset.LeftTex = mat.GetTexture("_LeftTex");
                    sidedSkyboxPreset.RightTex = mat.GetTexture("_RightTex");
                    sidedSkyboxPreset.UpTex = mat.GetTexture("_UpTex");
                    sidedSkyboxPreset.DownTex = mat.GetTexture("_DownTex");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"<color=orange>Preset Cannot Loaded! Did you load skybox material completely on RenderSettings?</color>");
                    throw (e);
                }

                this.SidedSkyboxPreset = sidedSkyboxPreset;

#if UNITY_EDITOR
                Debug.Log($"<color=cyan>Preset \"{this.name}\" completely loaded.</color>");
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    #region Unity Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(EnviromentPreset))]
    public class QuestManager_EditorFunctions : Editor
    {
        private EnviromentPreset baseTarget; // EnviromentPreset

        void OnEnable() { baseTarget = (EnviromentPreset)target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUIStyle style = new GUIStyle();
            style.richText = true;

            GUILayout.Label("\n\n<b><size=16><color=cyan>Load the settings of the current scene into this preset</color></size></b>", style);

            if (GUILayout.Button("Load to preset!"))
            {
                baseTarget.LoadCurrentSettings();
            }

            GUILayout.Label("\n\n<b><size=16><color=cyan>Set this preset to scene</color></size></b>", style);

            if (GUILayout.Button("Preset to scene!"))
            {
                this.PresetToScene();
            }
        }

        private void PresetToScene()
        {
            // Enviroment Intensity
            RenderSettings.ambientIntensity = baseTarget.LightningIntensityMultiplier;
            RenderSettings.reflectionIntensity = baseTarget.ReflectionsIntensityMultiplier;

            // Fog
            RenderSettings.fogColor = baseTarget.FogPreset.FogColor;
            RenderSettings.fogStartDistance = baseTarget.FogPreset.FogStart;
            RenderSettings.fogEndDistance = baseTarget.FogPreset.FogEnd;

            try // Directional Light(Sun) Blend via find object name "Directional Light"
            {
                Light directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();

                directionalLight.color = baseTarget.SunPreset.SunColor;
                directionalLight.intensity = baseTarget.SunPreset.SunIntensity;
            } 
            catch(System.Exception e)
            {
                Debug.LogError(e);
            }

            // Skybox
            Material skyboxMat = RenderSettings.skybox;
            skyboxMat.SetTexture("_FrontTex", baseTarget.SidedSkyboxPreset.FrontTex);
            skyboxMat.SetTexture("_BackTex", baseTarget.SidedSkyboxPreset.BackTex);
            skyboxMat.SetTexture("_LeftTex", baseTarget.SidedSkyboxPreset.LeftTex);
            skyboxMat.SetTexture("_RightTex", baseTarget.SidedSkyboxPreset.RightTex);
            skyboxMat.SetTexture("_UpTex", baseTarget.SidedSkyboxPreset.UpTex);
            skyboxMat.SetTexture("_DownTex", baseTarget.SidedSkyboxPreset.DownTex);
            skyboxMat.SetFloat("_Blend", 0f);
        }
    }
#endif
    #endregion
}