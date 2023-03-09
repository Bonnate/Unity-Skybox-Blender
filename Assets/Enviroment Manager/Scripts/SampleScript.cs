using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CoBlendSkies());
    }

    private IEnumerator CoBlendSkies()
    {
        while (true)
        {
            EnviromentManager.Instance.BlendEnviroment("Mid", 5.0f);
            yield return new WaitForSeconds(10.0f);

            EnviromentManager.Instance.BlendEnviroment("Night", 5.0f);
            yield return new WaitForSeconds(10.0f);

            EnviromentManager.Instance.BlendEnviroment("Day", 5.0f);
            yield return new WaitForSeconds(10.0f);
        }
    }
}
