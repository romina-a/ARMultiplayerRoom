using MixedReality.Toolkit.SpatialManipulation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ShapeCollisionDetection : MonoBehaviour
{
    [SerializeField]
    string shapeSortingCubeName = "ShapeSortingCubeFixed";

    [SerializeField]
    Material successMaterial;

    [SerializeField]
    Material defaultMaterial;

    [SerializeField]
    Material touchingMaterial;

    [SerializeField]
    Vector3 successLocation;

    [SerializeField]
    float waitBeforeAccept = 1f;

    [SerializeField]
    bool resetOnCollision = false;

    [SerializeField]
    float collisionWaitThreshold = 1f;

    private int n_colliding = 0;

    float statusChangeStarted = 0;

    float collisionStartTime = 0;

    enum status {outOfBox, enteredBoxCorrectly, enteredBoxIncorrectly};
    enum collisionStatus { colliding, notColliding};

    status myStatus = status.outOfBox;

    collisionStatus myCollisionStatus = collisionStatus.notColliding;

    Vector3 defaultLocalPosition;
    Quaternion defaultLocalRotation;
    RigidbodyConstraints defaultConstraints;

    string LastHoleTrigger;


    // Start is called before the first frame update
    void Start()
    {
        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
        defaultConstraints = gameObject.GetComponent<RigidbodyConstraints>();
        if (defaultMaterial == null) { defaultMaterial = GetComponent<Material>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (myStatus != status.outOfBox && Time.time-statusChangeStarted > waitBeforeAccept)
        {
            applyStatus();
        }

        if (resetOnCollision && myStatus == status.outOfBox && myCollisionStatus == collisionStatus.colliding && Time.time - collisionStartTime > collisionWaitThreshold)
        {
            ResetTransform();
        }

    }

    public void Reset()
    {
        myStatus = status.outOfBox;
        myCollisionStatus = collisionStatus.notColliding;
        gameObject.GetComponent<Rigidbody>().constraints = defaultConstraints;
        gameObject.transform.SetLocalPositionAndRotation(defaultLocalPosition, defaultLocalRotation);
        gameObject.GetComponent<ObjectManipulator>().enabled = true;
        setMaterial(defaultMaterial);
    }

    private void ResetTransform()
    {
        myStatus = status.outOfBox;
        gameObject.GetComponent<ObjectManipulator>().enabled = false;
        gameObject.transform.SetLocalPositionAndRotation(defaultLocalPosition, defaultLocalRotation);
        gameObject.GetComponent<ObjectManipulator>().enabled = true;
    }

    void applyStatus()
    {
        if (myStatus == status.enteredBoxCorrectly)
        {
            if (successMaterial != null)
            {
                setMaterial(successMaterial);
            }
            gameObject.transform.SetLocalPositionAndRotation(successLocation, Quaternion.identity);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else if (myStatus == status.enteredBoxIncorrectly)
        {
            ResetTransform() ;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger entered on "+gameObject.name);
        if (!other.gameObject.name.Substring(0, 4).Equals("Hole") && !other.gameObject.name.Equals("BoxCentreTrigger"))
        {
            //Debug.Log("Unrelated trigger");
            return;
        }
        if (other.gameObject.name.Equals("BoxCentreTrigger"))
        {
            Debug.Log("triggered with box centre");
            if (LastHoleTrigger.Equals("Hole" + gameObject.name + "Trigger"))
            {
                myStatus = status.enteredBoxCorrectly;
                statusChangeStarted = Time.time;
            }
            else
            {
                myStatus = status.enteredBoxIncorrectly;
                statusChangeStarted = Time.time;
            }
        }
        else
        {
            Debug.Log("triggered with"+ other.gameObject.name);
            LastHoleTrigger = other.gameObject.name;
        }
    }

    private void setMaterial(Material material)
    {
        setMaterialRecursive(material, gameObject);
    }

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
    private bool isTargetedBox(GameObject go)
    {
        return go.name.Equals(shapeSortingCubeName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.name + ":" + " collision enter with" + collision.gameObject.name);
        if (isTargetedBox(collision.gameObject))
        {
            Debug.Log("entered collision with targeted object");
            n_colliding += 1;
            if (myStatus == status.outOfBox)
            {
                setMaterial(touchingMaterial);
                Debug.Log("changed color to colliding in collision enter, colliding with: " + n_colliding);
                collisionStartTime = Time.time;
                myCollisionStatus = collisionStatus.colliding;
            }
            else
            {
                Debug.Log("changed color to default in collision enter, colliding with: " + n_colliding);
                setMaterial(defaultMaterial);
                myCollisionStatus = collisionStatus.colliding;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log(gameObject.name + ":" + " collision exit with" + collision.gameObject.name);
        if (isTargetedBox(collision.gameObject))
        {
            n_colliding -= 1;
            Debug.Log("in collision exit, colliding with: " + n_colliding);
            if (n_colliding <= 0)
            {
                setMaterial(defaultMaterial);
                myCollisionStatus = collisionStatus.notColliding;
                Debug.Log("changed color to normal in collision exit");
            }
        }
    }
}
