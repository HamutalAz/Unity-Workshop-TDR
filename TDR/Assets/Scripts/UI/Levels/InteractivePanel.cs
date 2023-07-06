using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractivePanel : MonoBehaviour
{
    //public List<TMP_InputField> inputFields;
    [SerializeField]
    public GameObject box;
    [SerializeField]
    public TMP_Text feedbackLabel;
    private string code;
    [SerializeField]
    public Image OKBTN;
    [SerializeField]
    public Image rightBTN;
    [SerializeField]
    public Image leftBTN;
    [SerializeField]
    public Image upBTN;
    [SerializeField]
    public Image downBTN;
    [SerializeField]
    public Button resetBTN;

    [SerializeField]
    private Sprite redArrow;
    [SerializeField]
    private Sprite blackArrow;
    [SerializeField]
    private Sprite redOK;
    [SerializeField]
    private Sprite blackOK;
    private Image lastClicked;

    // Start is called before the first frame update
    void Start()
    {
        lastClicked = upBTN;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            downClicked();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upClicked();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightClicked();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftClicked();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OkClicked();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"key", "owner" }
            };

            box.GetComponent<Box>().dropObject(data);
            code = "";
            restoreLastClickedColor();
        }
    }

    public void OkClicked()
    {
        restoreLastClickedColor();

        lastClicked = OKBTN;
        OKBTN.GetComponent<Image>().sprite = redOK;

        Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"key", "isOpen" },
                    {"code", code }
                };

        box.GetComponent<Box>().sendCode(data);
        code = "";

    }

    public void rightClicked()
    {
        arrowBtnClicked('R', rightBTN);
    }

    public void leftClicked()
    {
        arrowBtnClicked('L', leftBTN);
    }

    public void upClicked()
    {
        arrowBtnClicked('U', upBTN);

    }

    public void downClicked()
    {
        arrowBtnClicked('D',downBTN);
    }

    public void resetClicked()
    {
        code = "";
        setFeedbackMessage(code);
        restoreLastClickedColor();
    }

    public void setFeedbackMessage(string message)
    {
        feedbackLabel.text = message;
    }

    private void restoreLastClickedColor()
    {
    if (lastClicked == OKBTN)
        lastClicked.GetComponent<Image>().sprite = blackOK;
    else
        lastClicked.GetComponent<Image>().sprite = blackArrow;
    }

    private void arrowBtnClicked(char c, Image img)
    {
        restoreLastClickedColor();
        img.GetComponent<Image>().sprite = redArrow;
        lastClicked = img;
        code += c;
        setFeedbackMessage(code);
    }
}
