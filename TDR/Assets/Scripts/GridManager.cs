using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 5;
    [SerializeField]
    private int cols = 4;
    private float posX = 275;
    private float posY = 850;
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
        for(int col = 0; col < cols; col++)
        {
            if(col != 0)
            {
                posX += 300;
            }
            posY = 850;
            for(int row = 0; row < rows; row++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);
                nodes.Add(tile);
                posY -= 150;
                tile.transform.position = new Vector2(posX, posY);
                //tile.transform.position = new Vector2(posX, posY);
            }
        }
        Destroy(referenceTile);
        DataBaseManager.instance.updateUsers();
    }
    public void updateNodes(string userId, List<User> users)
    {
        //iterate through all the nodes in the grid (currently 20)
        for (int i = 0; i < nodes.Count; i++)
        {
            //presents current users in system, changes the id of the owner to yellow.
            if (i < users.Count)
            {
                nodes[i].SetActive(true);
                TextMeshProUGUI textBox = nodes[i].GetComponentInChildren<TextMeshProUGUI>();
                if (users[i].Equals(userId))
                {
                    textBox.color = Color.yellow;
                }
                textBox.text = users[i].userName;
                
            }
            //hides the unused nodes.
            else
            {
                nodes[i].SetActive(false);
            }
        }
    }
}
