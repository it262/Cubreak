using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleEffect : MonoBehaviour
{

	IntermittentChaos IChaos;
    Color defaultColor;
    Renderer renderer;
    float x = 0;
    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
		IChaos = new IntermittentChaos ();
        renderer = GetComponent<Renderer>();
        defaultColor = renderer.materials[0].GetColor("_EmissionColor");
    }

    private void Update()
    {
        if ((t += Time.deltaTime) > 0.1f)
        {
            t = 0;
            x = IChaos.getChaos(x);
            renderer.materials[0].SetColor("_EmissionColor", defaultColor * x);
        }
    }

}
