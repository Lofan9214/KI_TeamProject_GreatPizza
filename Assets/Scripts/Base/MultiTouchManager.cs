using UnityEngine;

public class MultiTouchManager : Singleton<MultiTouchManager>
{
    public enum TouchState
    {
        None,
        Tap,
        DoubleTap,
        LongPress
    }

    public TouchState CurrentTouchState { get; private set; }

    public bool Tap => CurrentTouchState == TouchState.Tap;
    public bool DoubleTap => CurrentTouchState == TouchState.DoubleTap;
    public bool LongPress => CurrentTouchState == TouchState.LongPress;

    public Vector2 SwipeDirection { get; private set; }
    public float Pinch { get; private set; }
    public float Rotation { get; private set; }

    public Vector2 TouchPosition { get; private set; }
    public Vector2 DeltaPosition { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsTouchStart { get; private set; }
    public bool IsTouchEnd { get; private set; }

    private float tapTimeThreshold = 0.2f;
    private float doubleTapTimeThreshold = 0.3f;
    private float longPressTimeThreshold = 0.5f;
    private float swipeTimeThreshold = 0.5f;
    private float swipeMinDistance = 0.5f;

    private float lastTapTime;
    private float touchStartTime;
    private Vector2 touchStartPos;
    private float moveThreshold = 50f;
    private bool isWaitingForDoubleTap;
    private bool isTouching;

    private int firstFingerId = -1;
    private int secondFingerId = -1;
    private Vector2 firstFingerStartPos;
    private Vector2 secondFingerStartPos;

    private void Update()
    {
        TouchState previousState = CurrentTouchState;
        CurrentTouchState = TouchState.None;
        SwipeDirection = Vector2.zero;
        Pinch = 0f;
        TouchPosition = Vector2.zero;
        DeltaPosition = Vector2.zero;
        IsTouchStart = false;
        IsMoving = false;
        IsTouchEnd = false;

        if (Input.touchCount >= 2)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (firstFingerId == -1)
                    {
                        firstFingerId = touch.fingerId;
                        firstFingerStartPos = touch.position;
                    }
                    else if (secondFingerId == -1)
                    {
                        secondFingerId = touch.fingerId;
                        secondFingerStartPos = touch.position;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (touch.fingerId == firstFingerId)
                    {
                        firstFingerId = -1;
                    }
                    else if (touch.fingerId == secondFingerId)
                    {
                        secondFingerId = -1;
                    }
                }
            }

            if (firstFingerId != -1 && secondFingerId != -1)
            {
                Touch? firstTouch = null;
                Touch? secondTouch = null;

                foreach (Touch touch in Input.touches)
                {
                    if (touch.fingerId == firstFingerId)
                        firstTouch = touch;
                    else if (touch.fingerId == secondFingerId)
                        secondTouch = touch;
                }

                if (firstTouch.HasValue && secondTouch.HasValue)
                {
                    if ((firstTouch.Value.phase == TouchPhase.Moved || firstTouch.Value.phase == TouchPhase.Stationary) &&
                        (secondTouch.Value.phase == TouchPhase.Moved || secondTouch.Value.phase == TouchPhase.Stationary))
                    {
                        Vector2 prevPos0 = firstTouch.Value.position - firstTouch.Value.deltaPosition;
                        Vector2 prevPos1 = secondTouch.Value.position - secondTouch.Value.deltaPosition;
                        float prevDistance = Vector2.Distance(prevPos0, prevPos1);

                        float currentDistance = Vector2.Distance(firstTouch.Value.position, secondTouch.Value.position);

                        Pinch = (currentDistance - prevDistance) / Screen.dpi;

                        Vector2 prevVector = prevPos1 - prevPos0;
                        Vector2 currentVector = secondTouch.Value.position - firstTouch.Value.position;
                        float angle = Vector2.SignedAngle(prevVector, currentVector);
                        Rotation = angle;
                    }
                }
            }
        }
        else
        {
            firstFingerId = -1;
            secondFingerId = -1;
        }

        if (isWaitingForDoubleTap && Time.time - lastTapTime > doubleTapTimeThreshold)
        {
            CurrentTouchState = TouchState.Tap;
            isWaitingForDoubleTap = false;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartTime = Time.time;
                    touchStartPos = touch.position;
                    TouchPosition = touch.position;
                    isTouching = true;
                    IsTouchStart = true;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    IsTouchEnd = true;
                    isTouching = false;
                    if (touch.phase == TouchPhase.Ended)
                    {
                        float touchDuration = Time.time - touchStartTime;
                        Vector2 delta = touch.position - touchStartPos;
                        float touchDistance = delta.magnitude;
                        float touchDistanceInches = touchDistance / Screen.dpi;

                        if (touchDuration <= swipeTimeThreshold && touchDistanceInches >= swipeMinDistance)
                        {
                            SwipeDirection = delta.normalized;
                        }
                        else if (touchDistance < moveThreshold)
                        {
                            if (touchDuration < tapTimeThreshold)
                            {
                                if (isWaitingForDoubleTap)
                                {
                                    if (Time.time - lastTapTime < doubleTapTimeThreshold)
                                    {
                                        CurrentTouchState = TouchState.DoubleTap;
                                        isWaitingForDoubleTap = false;
                                    }
                                }
                                else
                                {
                                    lastTapTime = Time.time;
                                    isWaitingForDoubleTap = true;
                                }
                            }
                        }
                    }
                    break;

                case TouchPhase.Stationary:
                    if (isTouching && previousState != TouchState.LongPress)
                    {
                        float currentDuration = Time.time - touchStartTime;
                        if (currentDuration > longPressTimeThreshold)
                        {
                            CurrentTouchState = TouchState.LongPress;
                            isWaitingForDoubleTap = false;
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    IsMoving = true;
                    TouchPosition = touch.position;
                    DeltaPosition = touch.deltaPosition;
                    break;
            }
        }
    }

}
