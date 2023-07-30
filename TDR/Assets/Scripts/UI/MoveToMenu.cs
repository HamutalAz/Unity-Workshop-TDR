using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        runTimer();
        //timer of 5-10 secondes then move
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void runTimer()
    {
        await Task.Delay(10000);
        SceneManager.LoadScene(sceneName: "LoginV2");
    }
}




