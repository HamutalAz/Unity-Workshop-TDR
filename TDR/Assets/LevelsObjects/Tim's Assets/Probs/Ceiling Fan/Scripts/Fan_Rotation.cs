using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;



public class Fan_Rotation : MonoBehaviour {

	//variable for speed
	public float speed=100;
	private int ind = 0;
    private List<int> directions = new List<int>() { 0, -4, 0, -4, 0, 4 , 0, 4, 0, -4, 0, 4, 0, 0};

    private void Start()
    {
        InvokeRepeating("changeDirection", 0, 0.3f);

    }

    void Update () {
        // rotate fan blades 
        transform.Rotate((new Vector3(0, 0, directions[ind])) * Time.deltaTime * speed);
    }

    private void changeDirection()
    {
        if (ind < directions.Count - 1)
            ind++;
        else
            ind = 0;
    }
}
