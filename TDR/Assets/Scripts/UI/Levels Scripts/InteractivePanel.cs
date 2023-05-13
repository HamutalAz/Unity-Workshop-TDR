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
    public Button OKBTN;
    [SerializeField]
    public Button rightBTN;
    [SerializeField]
    public Button LeftTN;
    [SerializeField]
    public Button upBTN;
    [SerializeField]
    public Button downBTN;
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
    private Button lastClicked;

    // Start is called before the first frame update
    void Start()
    {

        //redArrow = Resources.Load("playRed", typeof(Sprite)) as Sprite;
        //blackArrow = Resources.Load("play", typeof(Sprite)) as Sprite;
        //redOK = Resources.Load("checkedRed", typeof(Sprite)) as Sprite;
        //blackOK = Resources.Load("checked", typeof(Sprite)) as Sprite;
        lastClicked = rightBTN;
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
        }
    }

    public void OkClicked()
    {
        if (lastClicked != OKBTN)
            lastClicked.GetComponent<Image>().sprite = blackArrow;

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
        if (lastClicked != OKBTN)
            lastClicked.GetComponent<Image>().sprite = blackArrow;
        else
            lastClicked.GetComponent<Image>().sprite = blackOK;

        rightBTN.GetComponent<Image>().sprite = redArrow;

        lastClicked = rightBTN;

        code += "R";
        setFeedbackMessage(code);
    }

    public void leftClicked()
    {
        if (lastClicked != OKBTN)
            lastClicked.GetComponent<Image>().sprite = blackArrow;
        else
            lastClicked.GetComponent<Image>().sprite = blackOK;

        downBTN.GetComponent<Image>().sprite = redArrow;

        lastClicked = LeftTN;
        code += "L";
        setFeedbackMessage(code);
        LeftTN.GetComponent<Image>().sprite = redArrow;

    }

    public void upClicked()
    {
        if (lastClicked != OKBTN)
            lastClicked.GetComponent<Image>().sprite = blackArrow;
        else
            lastClicked.GetComponent<Image>().sprite = blackOK;

        downBTN.GetComponent<Image>().sprite = redArrow;

        lastClicked = upBTN;
        code += "U";
        setFeedbackMessage(code);
        upBTN.GetComponent<Image>().sprite = redArrow;

    }

    public void downClicked()
    {
        if (lastClicked != OKBTN)
            lastClicked.GetComponent<Image>().sprite = blackArrow;
        else
            lastClicked.GetComponent<Image>().sprite = blackOK;

        downBTN.GetComponent<Image>().sprite = redArrow;

        lastClicked = downBTN;
        code += "D";
        setFeedbackMessage(code);
    }

    public void resetClicked()
    {
        code = "";
        setFeedbackMessage(code);
    }

    public void setFeedbackMessage(string message)
    {
        feedbackLabel.text = message;
    }

}
