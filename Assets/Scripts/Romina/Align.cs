using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class Align : MonoBehaviour
{
    [SerializeField]
    GameObject room;

    [SerializeField]
    float move_speed = 0.003f;

    [SerializeField]
    float rotatoin_speed = 0.01f;

    bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    List<string> xboxButtons = new List<string> { "X", "Y", "A", "B", "Left Stick Button", "Right Stick Button", "Start", "Back", "RB", "LB", };
    List<string> xboxAxes = new List<string> { "Left Stick X", "Left Stick Y", "Right Stick X", "Right Stick Y", "D-pad X", "D-pad Y", "RT", "LT", "Triggers" };

    void LogButtons()
    {
        //Debug.Log("printing buttons");
        foreach (string name in xboxButtons)
        {
            //Debug.Log("checking " + name);
            if (Input.GetButtonDown(name))
                Debug.Log("Button Down: "+ name.ToString());
        }
    }

    void LogAxes()
    {
        foreach (string name in xboxAxes)
        {
            if (Input.GetAxis(name)!=0)
                Debug.Log("Axis: " + name + " is: " + Input.GetAxis(name).ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //LogButtons();
        //LogAxes();

        if (Input.GetButtonDown("Start"))
        {
            active = true; 
            //print("moving changed: " + active);
        }
        if (Input.GetButtonDown("Back"))
        {
            active = false;
            //print("moving changed: " + active);
        }
        if (active) 
        {
            // print("moving");
            room.transform.Translate(new Vector3(
                Input.GetAxis("Left Stick X")+Input.GetAxis("D-pad X"), 
                Input.GetAxis("D-pad Y"), 
                Input.GetAxis("Left Stick Y"))* move_speed);

            room.transform.Rotate(room.transform.rotation * Vector3.up * Input.GetAxis("Triggers") * rotatoin_speed);
        }
    }
}
