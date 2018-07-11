using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillSetter : MonoBehaviour {

    public FloatReference Variable;
    public FloatReference Min;
    public FloatReference Max;

    public Image image;

    private void Update()
    {
        image.fillAmount = Mathf.Clamp((float)(Variable.Value/Max),Min, Max);
    }
}
