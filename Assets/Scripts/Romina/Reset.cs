using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

class MyTransform
{
    public Vector3 localPosition;
    public Quaternion localRotation;

    public MyTransform(Transform tf)
    {
        localPosition = tf.localPosition;
        localRotation = tf.localRotation;
    }
}

public class Reset : MonoBehaviour
{
    [SerializeField]
    GameObject room;
    Dictionary<string, MyTransform> initial_transforms;

    bool active = true;
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


    void Start()
    {
        Debug.Log("Reset Start. Saving initial locations");
        initial_transforms = new Dictionary<string, MyTransform> ();
        foreach (Transform go in room.transform.GetComponentInChildren<Transform>()) 
        {
            if (go.GetComponent<ShapeCollisionDetection>() == null)
            {
                Debug.Log("Saving" + go.name);
                initial_transforms.Add(go.name, new MyTransform(go.transform));
            }
        }
    }
    private void ResetChildren()
    {
        Debug.Log("Reset ResetChildren. Reseting locations");

        foreach (Transform go in room.transform.GetComponentInChildren<Transform>())
        {
            Debug.Log("Resetting" + go.name);
            if (go.GetComponent<ShapeCollisionDetection>() != null)
            {
                go.GetComponent<ShapeCollisionDetection>().Reset();
            }
            else
            {
                MyTransform localTransform;
                initial_transforms.TryGetValue(go.name, out localTransform);
                go.transform.SetLocalPositionAndRotation(localTransform.localPosition, localTransform.localRotation);
            }
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
            print("start pressed, reset active: " + active);
        }
        if (Input.GetButtonDown("Back"))
        {
            active = false;
            print("start pressed, reset active: " + active);
        }
        if (active)
        {
            if (Input.GetButtonDown("B"))
            {
                print("B pressed");
                ResetChildren();
            }
        }
    }
}
