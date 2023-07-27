using System.Collections;
using System.Collections.Generic;
//using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;

public class Drawer : Interactable
{
    [SerializeField]
    private GameObject clipboard;
    private bool isOpen = false;
    [SerializeField]
    private tutorialHandler tHandler;

    // sound effects
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        isOpen = !isOpen;
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
        GetComponentInParent<Animator>().SetBool("isOpen", isOpen);
        gameObject.GetComponent<BoxCollider>().enabled = false;  //disable drawer interaction.
        clipboard.GetComponent<BoxCollider>().enabled = true;    // start clipboard interaction.
        tHandler.showNextGuidePanel(4);
    }
}
