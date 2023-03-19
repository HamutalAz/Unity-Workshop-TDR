using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldFocus : MonoBehaviour
{
    public TMP_InputField lInputField;
    // Start is called before the first frame update
    void Start()
    {
        lInputField.Select();
        lInputField.ActivateInputField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
