using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Taggle.HealthApp.Controllers;
using DG.Tweening;
using System;

namespace Taggle.HealthApp.Components
{
    public class DragDetector : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public RectTransform target;
        public bool RealtimeDragUpdate = false;
        public delegate void EndDrag(Direction dir);
        public event EndDrag eventEnd;
        #region FIELDS
        static DragDetector instance = null;
        public static DragDetector Instance {
            get {
                if (instance == null) {
                    instance = UnityEngine.Object.FindObjectOfType<DragDetector> ();
                }
                return instance;
            }
        }
        public enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }

        public float pos;
        [HideInInspector]
        public float defaultPos;
        private float maxDistanceLeft = -1f;
        private float maxDistanceRight = -1f;
        #endregion

        void Awake()
        {
            if(target == null)
                target = this.GetComponent<RectTransform>();
        }

        void Start()
        {
            defaultPos = pos = target.localPosition.x;
            
        }

        public void SetMaxDistanceLeft(float value)
        {
            maxDistanceLeft = value;
        }

        public void SetMaxDistanceRight(float value)
        {
            maxDistanceRight = value;
        }

        #region  IDragHandler - IEndDragHandler
        public void OnEndDrag(PointerEventData eventData)
        {
            Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
            Direction curDirection = GetDragDirection(dragVectorDirection);
            if(RealtimeDragUpdate)
            {
                DOTween.Clear(this.transform);
                float delta = Mathf.Abs(target.localPosition.x - pos);
                // Debug.Log("OnEndDrag direction: " + curDirection.ToString() + " - distance: " + delta);

                if(delta >= 100f)
                    eventEnd?.Invoke(curDirection);
                
                ReturnDefaultPos();
            }
            else
                eventEnd?.Invoke(curDirection);
        }

        //It must be implemented otherwise IEndDragHandler won't work 
        public void OnDrag(PointerEventData eventData){
            if (RealtimeDragUpdate)
            {   
                //Check ob left
                if(maxDistanceLeft != -1f && GetDragDirection((eventData.position - eventData.pressPosition).normalized) == Direction.Left && Mathf.Abs(target.localPosition.x - pos) > maxDistanceLeft)
                {
                    target.localPosition = new Vector3(pos - maxDistanceLeft,target.localPosition.y,target.localPosition.z);
                    return;
                }

                //Check ob right
                if(maxDistanceRight != -1f && GetDragDirection((eventData.position - eventData.pressPosition).normalized) == Direction.Right && Mathf.Abs(target.localPosition.x - pos) > maxDistanceRight)
                {
                    target.localPosition = new Vector3(pos + maxDistanceRight,target.localPosition.y,target.localPosition.z);
                    return;
                }
                
                target.localPosition += new Vector3 (eventData.delta.x, 0, 0);
            }
        }

        private Direction GetDragDirection(Vector3 dragVector)
        {
            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);
            Direction draggedDir;
            if (positiveX > positiveY)
            {
                draggedDir = (dragVector.x > 0) ? Direction.Right : Direction.Left;
            }
            else
            {
                draggedDir = (dragVector.y > 0) ? Direction.Up : Direction.Down;
            }
            return draggedDir;
        }

        public void ReturnDefaultPos()
        {
            target.DOLocalMoveX(pos,0.25f);
        }
        #endregion
    }

}

