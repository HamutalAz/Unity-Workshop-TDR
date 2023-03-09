using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    private int rows = 5;
    private int cols = 4;
    private float width = 200;
    private float height = 100;
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
        for(int row = 0; row < rows; row++)
        {
            for(int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);
                nodes.Add(tile);
                float posX = col * width + 1;
                float posY = row * height + 1;
                tile.transform.position = new Vector2(posX, posY);
        
            }
        }
        Destroy(referenceTile);
        updateNodes();
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
