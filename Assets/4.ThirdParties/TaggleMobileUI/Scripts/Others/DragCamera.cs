using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Taggle.HealthApp.Others{
	public class DragCamera : MonoBehaviour {
		public bool RotateX;
		public bool reverseX;
		public bool RotateY;
		public bool reverseY;
		public float RotateAmount = 15f;
		public Transform target;
		public Vector3 offset;
		Vector3 FirstPoint;
		Vector3 SecondPoint;
		float xAngle = 0f;
		float yAngle= 0f;
		float xAngleTemp= 0f;
		float yAngleTemp = 0f;

		public LineRenderer lineRender;

		void LateUpdate()
		{
			OrbitCamera();
		}

		public void OrbitCamera()
		{
			#if UNITY_EDITOR
			if (Input.GetMouseButton(0)){
				if(lineRender != null){
					lineRender.enabled = false;
				}

				if(EventSystem.current != null){
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
				}
				
				//Vector3 target = Vector3.zero; //this is the center of the scene, you can use any point here
				float y_rotate = Input.GetAxis("Mouse X") * RotateAmount;
				float x_rotate = Input.GetAxis("Mouse Y") * RotateAmount;
				OrbitCamera(y_rotate,x_rotate);

			}
			if(Input.GetMouseButtonUp(0)){
				if(lineRender != null){
					lineRender.enabled = true;
				}
			}
			#else
			if(Input.touchCount > 0){
				if(lineRender != null){
					lineRender.enabled = false;
				}

				if(EventSystem.current != null){
					foreach (Touch touch in Input.touches)
					{
						int id = touch.fingerId;
						if (EventSystem.current.IsPointerOverGameObject(id))
						{
							return;
						}
					}
				}
				
				if(Input.GetTouch(0).phase == TouchPhase.Began){
					FirstPoint = Input.GetTouch(0).position;
					yAngleTemp = yAngle;
				}
				if(Input.GetTouch(0).phase == TouchPhase.Moved){
					SecondPoint = Input.GetTouch(0).position;
					xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
					yAngle = yAngleTemp + (SecondPoint.y - FirstPoint.y) * 90 / Screen.height;
					OrbitCamera(xAngle,yAngle);
				}

				if(Input.GetTouch(0).phase == TouchPhase.Ended){
					if(lineRender != null){
						lineRender.enabled = true;
					}
				}
			}
			#endif
		}

		public void OrbitCamera(float y_rotate,float x_rotate)
		{
			Vector3 angles = transform.eulerAngles;
			angles.z = 0;
			transform.eulerAngles = angles;
			if(RotateX){
				transform.RotateAround(target.position, Vector3.up, y_rotate * (reverseX?-1f:1f));
			}
			if(RotateY){
				transform.RotateAround(target.position, Vector3.left, x_rotate * (reverseY?-1f:1f));
			}
			transform.LookAt(target.position - offset);
		}
	}
}
