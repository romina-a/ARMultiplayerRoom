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

    [Tooltip("Material to use when the object has been put inside the box correctly")]
    [SerializeField]
    Material successMaterial;

    [Tooltip("Object's default material")]
    [SerializeField]
    Material defaultMaterial;

    [Tooltip("Material to use when the object is touching the box")]
    [SerializeField]
    Material touchingMaterial;

    [Tooltip("The position of the shape when it is put inside of the box successfully, relative to its parent position.")]
    [SerializeField]
    Vector3 successPosition;

    [Tooltip("Seconds to wait while the object is touchinig the box's bottom trigger before accepting success.")]
    [SerializeField]
    float waitBeforeAccept = 1f;

    [Tooltip("Whether to reset object's state if it constantly touches the box for a period of time.")]
    [SerializeField]
    bool resetOnCollision = false;

    [Tooltip("Seconds to allow constant collision with the box before reseting the object")]
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
            gameObject.transform.SetLocalPositionAndRotation(successPosition, Quaternion.identity);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else if (myStatus == status.enteredBoxIncorrectly)
        {
            ResetTransform() ;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Substring(0, 4).Equals("Hole") && !other.gameObject.name.Equals("BoxCentreTrigger"))
        {
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
            n_colliding += 1;
            Debug.Log(gameObject.name + ":" + " collision enter with" + collision.gameObject.name);
            if (myStatus == status.outOfBox)
            {
                if (myCollisionStatus == collisionStatus.notColliding)
                {

                    setMaterial(touchingMaterial);
                    collisionStartTime = Time.time;
                    myCollisionStatus = collisionStatus.colliding;
                }
            }
            else
            {
                setMaterial(defaultMaterial);
                myCollisionStatus = collisionStatus.colliding;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
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
