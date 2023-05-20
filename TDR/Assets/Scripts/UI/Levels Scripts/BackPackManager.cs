using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    private int empty = 0;
    [SerializeField]
    List<GameObject> placeholders;
    [SerializeField]
    BackPackPanel bpPanel;
    Dictionary<string, GameObject> imageToObjectMap = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PutInBackPack(string imageName, Vector2 delta, string objName)
    {
        if (empty == placeholders.Count) { 
            Debug.Log("**** BACKPACK IS FULL!!!");
            return;
        }

        GameObject img = createNewObjImage(imageName, delta, placeholders[empty]);
        imageToObjectMap.Add(objName, img);

        empty++;

        bpPanel.GetComponent<BackPackPanel>().PutInBackPack(imageName, delta, objName);
        
    }

    static public GameObject createNewObjImage(string objName, Vector2 delta, GameObject parent)
    {
        GameObject imgObject = new GameObject(objName);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(parent.transform);            // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(0f, 0f);           // setting position - center
        trans.sizeDelta = delta;                                // custom size

        Image image = imgObject.AddComponent<Image>();
        Texture2D tex = Resources.Load<Texture2D>(objName);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        trans.transform.SetParent(parent.transform);            // setting parent

        return imgObject;
    }

    public void deleteFromBackPack(string gameObjName)
    {
        imageToObjectMap.Remove(gameObjName);
        Destroy(imageToObjectMap[gameObjName]);
        empty--;
    }

}
