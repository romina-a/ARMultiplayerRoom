using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class RoomRenderControll : MonoBehaviour
{
    [SerializeField]
    List<GameObject> roomContent;

    bool active = true;
    bool showRoom = true;
    // Start is called before the first frame update

    List<string> xboxButtons = new List<string> { "X", "Y", "A", "B", "Left Stick Button", "Right Stick Button", "Start", "Back", "RB", "LB", };
    List<string> xboxAxes = new List<string> { "Left Stick X", "Left Stick Y", "Right Stick X", "Right Stick Y", "D-pad X", "D-pad Y", "RT", "LT", "Triggers" };

    void LogAxes()
    {
        foreach (string name in xboxAxes)
        {
            if (Input.GetAxis(name) != 0)
                Debug.Log("Axis: " + name + " is: " + Input.GetAxis(name).ToString());
        }
    }
    void LogButtons()
    {
        //Debug.Log("printing buttons");
        foreach (string name in xboxButtons)
        {
            //Debug.Log("checking " + name);
            if (Input.GetButton(name))
                Debug.Log("Button Down: " + name.ToString());
        }
    }

    private void RenderRoomContent(bool mode)
    {
        Debug.Log("Changing room render enabled");
        foreach (GameObject go in roomContent)
        {
           RenderGameObjectRecursive(go, mode);
        }
    }

    private void RenderGameObjectRecursive(GameObject o, bool mode)
    {
        if (o.GetComponent<Renderer>() != null)
        {
            o.GetComponent<Renderer>().enabled = mode;
        }
        for (int i = 0; i < o.transform.childCount; i++)
        {
            RenderGameObjectRecursive(o.transform.GetChild(i).gameObject, mode);
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
            print("start pressed, RoomRenderControl active: " + active);
        }
        if (Input.GetButtonDown("Back"))
        {
            active = false;
            print("start pressed, RoomRenderControl active: " + active);
        }
        if (active)
        {
            if (Input.GetButtonDown("Y"))
            {
                print("Y pressed");
                showRoom = !showRoom;
                RenderRoomContent(showRoom);
            }
        }
    }
}
