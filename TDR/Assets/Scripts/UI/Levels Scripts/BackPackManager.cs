using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
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
        Debug.Log("*** BP MANAGER: Item inside!!!");

        if (empty == placeholders.Count) { 
            Debug.Log("NO PLACE IN BACKPACK!!!");
            return;
        }

        //GameObject item = (GameObject)Instantiate(Resources.Load(objName));
        //GameObject item2 = (GameObject)Instantiate(item, transform);
        //item2.transform.parent = placeholders[empty].transform; // ?????

        //item2.transform.position = placeholders[empty].transform.position;

        ////Debug.Log("LH: createPlayersAvatars(): avatar created at: " + playerLoc);

        //Destroy(item);

        GameObject imgObject = new GameObject(objName);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(placeholders[empty].transform); // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(0f, 0f); // setting position, will be on center
        trans.sizeDelta = delta; // custom size

        Image image = imgObject.AddComponent<Image>();
        Texture2D tex = Resources.Load<Texture2D>(objName);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        trans.transform.SetParent(placeholders[empty].transform); // setting parent

        empty++;
        
    }
}
