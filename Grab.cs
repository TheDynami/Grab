using UnityEngine;

public class Grab : MonoBehaviour
{
	public LayerMask whatIsGrabbable;
	Rigidbody objectGrabbing;
	private SpringJoint grabJoint;
	private LineRenderer grabLr;
	private Vector3 myGrabPoint;
	private Vector3 myHandPoint;

	Camera cam;

    private void Awake()
    {
		cam = Camera.main;
    }

	private void LateUpdate()
	{
		DrawGrabbing();
	}

    private void Update()
    {
		if (Input.GetKey("mouse 0"))
		{
				GrabObject();
		}
		if (Input.GetKeyUp("mouse 0"))
		{		
			if (objectGrabbing)
			{
				StopGrab();
			}
		}
	}

    private void GrabObject()
	{
		if (objectGrabbing == null)
		{
			StartGrab();
			return;
		}
		HoldGrab();
	}

	private void DrawGrabbing()
	{
		if (!objectGrabbing)
		{
			return;
		}
		myGrabPoint = Vector3.Lerp(myGrabPoint, objectGrabbing.position, Time.deltaTime * 45f);
		myHandPoint = Vector3.Lerp(myHandPoint, grabJoint.connectedAnchor, Time.deltaTime * 45f);
		grabLr.SetPosition(0, myGrabPoint);
		grabLr.SetPosition(1, myHandPoint);
	}

	private void StartGrab()
	{
		RaycastHit[] array = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 8f, whatIsGrabbable);
		if (array.Length < 1)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.GetComponent<Rigidbody>())
			{
				objectGrabbing = array[i].transform.GetComponent<Rigidbody>();
				grabJoint = objectGrabbing.gameObject.AddComponent<SpringJoint>();
				grabJoint.autoConfigureConnectedAnchor = false;
				grabJoint.minDistance = 0f;
				grabJoint.maxDistance = 0f;
				grabJoint.damper = 4f;
				grabJoint.spring = 40f;
				grabJoint.massScale = 5f;
				objectGrabbing.angularDrag = 5f;
				objectGrabbing.drag = 1f;
				grabLr = objectGrabbing.gameObject.AddComponent<LineRenderer>();
				grabLr.positionCount = 2;
				grabLr.startWidth = 0.05f;
				grabLr.material = new Material(Shader.Find("Sprites/Default"));
				grabLr.numCapVertices = 10;
				grabLr.numCornerVertices = 10;
				return;
			}
		}
	}

	private void HoldGrab()
	{
		grabJoint.connectedAnchor = cam.transform.position + cam.transform.forward * 5.5f;
		grabLr.startWidth = 0f;
		grabLr.endWidth = 0.0075f * objectGrabbing.velocity.magnitude;
	}

	private void StopGrab()
	{
		Destroy(grabJoint);
		Destroy(grabLr);
		objectGrabbing.angularDrag = 0.05f;
		objectGrabbing.drag = 0f;
		objectGrabbing = null;
	}
}
