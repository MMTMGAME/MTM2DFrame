using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
using VContainer;

namespace UnityEngine.InputSystem.OnScreen
{
    public class MyOnScreenStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private PointerEventData pointerEventData;
        public Press press;
        public void OnOutLinePress(int fingerid)
        {
            
            pointerEventData = new PointerEventData(EventSystem.current);
            MyDebug.Log($"fingerid:{fingerid}");
            pointerEventData.position = press._inputCenter.GetfingerAction(fingerid).ReadValue<Vector2>();
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), pointerEventData.position,pointerEventData.pressEventCamera, out var position);
            SendValueToControl(pointerEventData.position);
            OnDrag(pointerEventData);
        }

        public void OnOutLinePressHold(int fingerid)
        {
            pointerEventData = new PointerEventData(EventSystem.current);
            MyDebug.Log($"按下：{fingerid},{press._inputCenter == null}");
            pointerEventData.position = press._inputCenter.GetfingerAction(fingerid).ReadValue<Vector2>();
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), pointerEventData.position,pointerEventData.pressEventCamera, out var position);
            OnDrag(pointerEventData);
        }

        public void OnOutLinePressUp(int fingerid)
        {
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            SendValueToControl(Vector2.zero);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;
            
            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;
            
            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            SendValueToControl(Vector2.zero);
        }

        private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;
        }

        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}
