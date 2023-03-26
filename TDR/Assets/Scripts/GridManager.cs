using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text playersLabel; 
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int cols = 4;
    [SerializeField]
    private float startPosX = 275;
    [SerializeField]
    private float startPosY = 850;
    [SerializeField]
    private float spaceX = 300;
    [SerializeField]
    private float spaceY = 150;
    private List<GameObject> nodes;
    // Start is called before the first frame update
    void Start()
    {
        DataBaseManager.instance.assignGridManager(this);
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        nodes = new List<GameObject>();
        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("Node"));
        float newPosY = startPosY;
        float newPosX = startPosX;
        for (int col = 0; col < cols; col++)
        {
            if(col != 0)
            {
                newPosX += spaceX;
            }
            newPosY = startPosY;
            for(int row = 0; row < rows; row++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);
                nodes.Add(tile);
                newPosY -= 150;
                tile.transform.position = new Vector2(newPosX, newPosY);
                tile.SetActive(false);
            }
        }
        Destroy(referenceTile);
        DataBaseManager.instance.updateUsers();
    }
    public void updateNodes(string userId, List<User> users)
    {
        //iterate through all the nodes in the grid (currently 20 => 4 cols 5 rows)
        for (int i = 0; i < nodes.Count; i++)
        {
            //presents current users in system, changes the id of the owner to yellow.
            if (i < users.Count)
            {
                nodes[i].SetActive(true);
                TMP_Text textBox = nodes[i].GetComponentInChildren<TMP_Text>();
                if (users[i].userName.Equals(userId))
                {
                    textBox.color = Color.yellow;
                }
                else
                {
                    textBox.color = Color.white;
                }
                textBox.text = users[i].userName;
                
            }
            //hides the unused nodes.
            else
            {
                nodes[i].SetActive(false);
            }
        }
        //updates current active players label
        playersLabel.text = WaitingForPlayersToString(users.Count);
    }

    public string WaitingForPlayersToString(int currentAmount)
    {
        return "Waiting for players: \n" + currentAmount + "/" + DataBaseManager.MAXPLAYERS;
    }
}
