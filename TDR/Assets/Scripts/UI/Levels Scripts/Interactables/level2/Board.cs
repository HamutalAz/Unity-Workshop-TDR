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

    // Start is called before the first frame update
    void Start()
    {
        DataBaseManager.instance.levelHandler.levelObjects.Add("Board", this);
        boardPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {

        Debug.Log("**** interact with Board!!!");

        // open board panel
        boardPanel.SetActive(true);
        DataBaseManager.instance.levelHandler.togglePlayerInputSystem(true);
    }

    public void setRooksInLocation(List<int> choosedLocations)
    {
        Debug.Log("$$$$$ setting rooks location");
        for(int i=0; i<rooks.Count; i++)
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
}
