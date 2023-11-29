using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The material used when object is not touching targeted objects [:either in shapeNames list or start with \"Collide\"]")]
    Material original;
    [Tooltip("The material used when object is touching with targeted objects [:either in shapeNames list or start with \"Collide\"]")]
    [SerializeField]
    Material touching;

    //[SerializeField]
    //[Tooltip("Renders the colliding components with 'red' material when the script is used with 'non-kinematic' RigidBody and Convex components.")]
    //bool renderCollidingComponents = false;
    //[SerializeField]
    //[Tooltip("When the script is used with 'non-kinematic' RigidBody and Convex components, this material is used to render the components that are colliding with a targeted object")]
    //Material red;

    [SerializeField]
    [Tooltip("If collision stays resets the colliding shape's location")]
    bool reset_on_collide = false;


    // TODO: learn how to enable and disable this based on reset_on_touch// but the best way is to call the reset position on the object itself so everyone goes to its own place 
    [SerializeField]
    [Tooltip("Wait this much seconds before resetting the shape's location")]
    float wait_before_reset = 0.5f;

    float collision_start_time;

    private int n_colliding = 0;

    private void Start()
    {
        Debug.Log("Collision trigger start");
        setMaterialRecursive(original, gameObject);
    }

    // if there is a renderer specifically specified uses that,
    // else, if this object has a renderer, sets its material. If not recursively sets materials in children
    private void setMaterialRecursive(Material m, GameObject o)
    {
        if (o.GetComponent<Renderer>() != null)
        {
            o.GetComponent<Renderer>().material = m;
        }
        for (int i = 0; i < o.transform.childCount; i++)
        {
            setMaterialRecursive(m, o.transform.GetChild(i).gameObject);
        }
    }

    private bool isTargetedObject(GameObject go)
    {
        return go.GetComponent<ShapeCollisionDetection>() != null;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.name + ":" + " collision enter with" + collision.gameObject.name);
        if (isTargetedObject(collision.gameObject))
        {
            Debug.Log("entered collision with targeted object");
            n_colliding += 1;
            setMaterialRecursive(touching, gameObject);
            Debug.Log("changed color to colliding in collision enter, colliding with: " + n_colliding);
            collision_start_time = Time.time;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(gameObject.name + ":" + " collision stay with" + collision.gameObject.name);
        Debug.Log("collision start time: " + collision_start_time + " now:" + Time.time + "diff: "+ (Time.time - collision_start_time));
        if (reset_on_collide && isTargetedObject(collision.gameObject) && Time.time - collision_start_time > wait_before_reset)
        {
            collision.gameObject.GetComponent<ShapeCollisionDetection>().Reset();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log(gameObject.name + ":" + " collision exit with" + collision.gameObject.name);
        if (isTargetedObject(collision.gameObject))
        {
            n_colliding -= 1;
            Debug.Log("in collision exit, colliding with: " + n_colliding);
            if (n_colliding > 0)
            {
                setMaterialRecursive(touching, gameObject);
            }
            else
            {
                setMaterialRecursive(original, gameObject);
                Debug.Log("changed color to normal in collision exit");
            }
        }
    }
}
