using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPackPanel : MonoBehaviour
{
    private int empty = 0;
    [SerializeField]
    List<GameObject> placeholders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutInBackPack(string objName, Vector2 delta)
    {
        Vector2 newDelta = delta / 2.2f;
        BackPackManager.createNewObjImage(objName, newDelta, placeholders[empty]);
    }

}
