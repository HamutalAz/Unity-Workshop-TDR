using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OldInteractivePanel : MonoBehaviour
{
    public List<TMP_InputField> inputFields;
    [SerializeField]
    public GameObject box;
    [SerializeField]
    public TMP_Text feedbackLabel;

    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        inputFields[i].Select();
        inputFields[i].ActivateInputField();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputFields[i].text != "")
        {

            // set focus to next feild with "TAB"
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Tab pressed!!!!");

                i++;
                if (i > inputFields.Count)
                    i = 0;

                inputFields[i].Select();
                inputFields[i].ActivateInputField();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                // send data to handler
                string code = null;
                for (int i = 0; i < inputFields.Count; i++)
                    code += inputFields[i].text;
                i = 0;

                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"key", "isOpen" },
                    {"code", code }
                };

                box.GetComponent<Box>().sendCode(data);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0; i < inputFields.Count; i++)
                inputFields[i].text = "";
            i = 0;

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"key", "owner" }
            };

            box.GetComponent<Box>().dropObject(data);            
        }
    }
    public void setFeedbackMessage(string message)
    {
        feedbackLabel.text = message;
    }

}
