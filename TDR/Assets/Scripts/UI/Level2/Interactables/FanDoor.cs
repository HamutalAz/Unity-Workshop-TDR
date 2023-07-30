using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanDoor : Interactable
{
    private bool isOpen = false;
    public LevelHandler levelHandler;

    // sounds effect
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        levelHandler.addLevelObject("fanDoor", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateUI(Dictionary<string, object> data)
    {
        isOpen = (bool)data["isOpen"];
        if (isOpen)
            gameObject.GetComponent<AudioSource>().PlayOneShot(clip);

        gameObject.GetComponent<Animator>().SetBool("isOpen", isOpen);

    }
}
