using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Game.Character.UI
{
    /// <summary>
    /// スマホ向けタッチ・スワイプ操作の処理
    /// PCではマウスドラッグで代用
    /// </summary>
    public class TouchInputHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [Header("スワイプ設定")]
        [SerializeField] private float swipeThreshold = 50f; // スワイプと認識する最小距離
        
        [Header("デバッグ")]
        [SerializeField] private bool showDebugLog = true;
        
        // イベント
        public event Action<Vector2> OnSwipeDetected;
        public event Action OnTapDetected;
        public event Action<Vector2> OnDragUpdate;
        
        private Vector2 dragStartPosition;
        private bool isDragging;
        private float dragDistance;
        
        void Update()
        {
            // スマホのバックボタン（Androidなど）
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackButtonPressed();
            }
        }
        
        // === ドラッグイベント ===
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragStartPosition = eventData.position;
            isDragging = true;
            dragDistance = 0f;
            
            if (showDebugLog)
                Debug.Log($"ドラッグ開始: {dragStartPosition}");
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            Vector2 currentPosition = eventData.position;
            Vector2 dragDelta = currentPosition - dragStartPosition;
            dragDistance = dragDelta.magnitude;
            
            // ドラッグ更新イベント
            OnDragUpdate?.Invoke(dragDelta);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            Vector2 dragEndPosition = eventData.position;
            Vector2 swipeVector = dragEndPosition - dragStartPosition;
            
            // スワイプと判定
            if (dragDistance >= swipeThreshold)
            {
                DetectSwipeDirection(swipeVector);
            }
            
            isDragging = false;
            
            if (showDebugLog)
                Debug.Log($"ドラッグ終了: {dragEndPosition}, 距離: {dragDistance}");
        }
        
        // === タップイベント ===
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // ドラッグ中はタップとして扱わない
            if (dragDistance > 10f) return;
            
            OnTapDetected?.Invoke();
            
            if (showDebugLog)
                Debug.Log("タップ検出");
        }
        
        // === スワイプ方向判定 ===
        
        void DetectSwipeDirection(Vector2 swipeVector)
        {
            float angle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
            
            string direction;
            if (angle >= -45f && angle < 45f)
                direction = "右";
            else if (angle >= 45f && angle < 135f)
                direction = "上";
            else if (angle >= -135f && angle < -45f)
                direction = "下";
            else
                direction = "左";
            
            if (showDebugLog)
                Debug.Log($"スワイプ検出: {direction}, 角度: {angle:F0}度, 距離: {swipeVector.magnitude:F0}px");
            
            OnSwipeDetected?.Invoke(swipeVector);
        }
        
        // === バックボタン ===
        
        void OnBackButtonPressed()
        {
            if (showDebugLog)
                Debug.Log("バックボタン押下");
            
            // TODO: 前の画面に戻る処理
        }
        
        // === 外部からの呼び出し用 ===
        
        /// <summary>
        /// 左スワイプを検出したか（次のキャラクターへ）
        /// </summary>
        public static bool IsSwipeLeft(Vector2 swipeVector)
        {
            return swipeVector.x < 0 && Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y);
        }
        
        /// <summary>
        /// 右スワイプを検出したか（前のキャラクターへ）
        /// </summary>
        public static bool IsSwipeRight(Vector2 swipeVector)
        {
            return swipeVector.x > 0 && Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y);
        }
        
        /// <summary>
        /// 上スワイプを検出したか
        /// </summary>
        public static bool IsSwipeUp(Vector2 swipeVector)
        {
            return swipeVector.y > 0 && Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x);
        }
        
        /// <summary>
        /// 下スワイプを検出したか
        /// </summary>
        public static bool IsSwipeDown(Vector2 swipeVector)
        {
            return swipeVector.y < 0 && Mathf.Abs(swipeVector.y) > Mathf.Abs(swipeVector.x);
        }
    }
}

