using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Board : Interactable
{
    [SerializeField]
    public GameObject boardPanel;

    [SerializeField]
    List<GameObject> rooks = new();
    private bool isPanelActive = false;

    // Start is called before the first frame update
    void Start()
    {
        DataBaseManager.instance.levelHandler.addLevelObject("Board", this);
        boardPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    async protected override void Interact()
    {
        Debug.Log("**** interact with Board!!!");

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "key", "owner" }
        };
        //send request to try and capture the panel!
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("pickUpObject", "board", data);

        if (response)
            toggleVisability();
    }

    public void setRooksInLocation(List<int> choosedLocations)
    {
        Debug.Log("$$$$$ setting rooks location");
        for (int i = 0; i < rooks.Count; i++)
        {
            placeRookInPosition(rooks[i], choosedLocations[i]);
        }
    }

    private void placeRookInPosition(GameObject rook, int cubeInd)
    {
        int colNum = cubeInd / 10;
        GameObject col = gameObject.transform.Find("col" + colNum).gameObject;
        GameObject cube = col.transform.Find("Cube" + cubeInd.ToString()).gameObject;

        rook.transform.eulerAngles = new Vector3(90, 0, 0);
        rook.transform.parent = cube.transform;
        rook.transform.localPosition = Vector3.zero;

    }

    async public void dropObject(Dictionary<string, object> data)
    {
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("dropObject", "board", data);
        Debug.Log("was able to drop menu?" + response);

        if (response)
            toggleVisability();


    }

    private void toggleVisability()
    {
        isPanelActive = !isPanelActive;
        boardPanel.SetActive(isPanelActive);
        DataBaseManager.instance.levelHandler.togglePlayerInputSystem(isPanelActive);
    }

    public override void UpdateUI(Dictionary<string, object> data)
    {
        List<int> choosedLocations = (List<int>)data["currentRooksLocations"];
        if (choosedLocations.Contains(0)) // means an invalid location is set
            return;

        setRooksInLocation(choosedLocations);

    }
}
