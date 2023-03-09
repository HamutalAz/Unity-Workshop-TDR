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
   // [SerializeField]
    private float posX = 275;
    //[SerializeField]
    private float posY = 850;
    private List<GameObject> nodes;
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
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
       // updateNodes();
    }
    private void updateNodes()
    {
        for(int i = 0; i < nodes.Count; i ++)
        {
            if( i > 9)
            {
                nodes[i].SetActive(false);
            }
            else
            {
                TextMeshProUGUI text = nodes[i].GetComponentInChildren<TextMeshProUGUI>();
                text.text = "hello world!";
                text.color = Color.yellow;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
