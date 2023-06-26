using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialHandler : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> panelsArr = new();
    int currInd = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextClick()
    {
        Debug.Log("count:" + panelsArr.Count);
        if(currInd < panelsArr.Count-1)
        {
            Debug.Log("currInd:" + currInd);
            currInd++;
            if (currInd >= 1)
                panelsArr[currInd - 1].SetActive(false);
            panelsArr[currInd].SetActive(true);
        }
        else
        {
            panelsArr[currInd].SetActive(false);
            currInd = 0;
            Debug.Log("Done!");
        }
    }

    public void backClick()
    {
        if (currInd > 0)
        {
            currInd--;
            if (currInd + 1 < panelsArr.Count)
                panelsArr[currInd + 1].SetActive(false);
            panelsArr[currInd].SetActive(true);
        }
    }
}
