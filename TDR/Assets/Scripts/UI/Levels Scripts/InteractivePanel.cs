using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractivePanel : MonoBehaviour
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
    async void Update()
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
                box.GetComponent<Box>().sendCode(code);

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


            bool response = await DataBaseManager.instance.levelManager.LaunchRequest("dropObject", "box", data);
            if (response)
            {
                box.GetComponent<Box>().toggleVisability();
            }
            Debug.Log(response);
            
        }
    }
    public void setFeedbackMessage(string message)
    {
        feedbackLabel.text = message;
    }
}
