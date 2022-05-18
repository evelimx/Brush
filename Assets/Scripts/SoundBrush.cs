using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBrush : MonoBehaviour
{
    public PathSetState state;
    public Transform soundCursor; // a path cursor user used to defince the movement path
    private LineRenderer _currLine; // path for the main object
    private LineRenderer _currKeyframeLine;
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;

    public AddAnimation addAnimation;
    public DrawTubes drawTubes; // to retrieve stroke lists
    public CanvasHandler canvas;
    public ControllerMode controllerMode;

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
    }

    // Update is called once per frame
    void Update()
    {

        if (controllerMode.readyForSketch && canvas.curBrush == "sound" && OVRInput.GetDown(OVRInput.Button.One))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (canvas.curBrush == "sound" && OVRInput.GetUp(OVRInput.Button.One))
        {
            state = PathSetState.WAITING;
        }

    }

    private void _createNewPath()
    {
        lastPos = transform.position;
        if (!addAnimation.insertKeyframe)
        {
            GameObject newPath = new GameObject("Sound Line");
            _currLine = newPath.AddComponent<LineRenderer>();
            _currLine.startWidth = .01f;
            _currLine.endWidth = .01f;
            _currLine.material.color = Color.red;
        }
        else
        {
            GameObject newPath = new GameObject("New Keyframe Path");
            _currKeyframeLine = newPath.AddComponent<LineRenderer>();
            _currKeyframeLine.startWidth = .01f;
            _currKeyframeLine.endWidth = .01f;
        }

        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = soundCursor.transform.position;

            if (curPos != lastPos)
            {  // when the controller is held
                if (!addAnimation.insertKeyframe)
                {
                    _currLine.positionCount = numClicks + 1;
                    _currLine.SetPosition(numClicks, curPos);
                }
                else
                {
                    _currKeyframeLine.positionCount = numClicks + 1;
                    _currKeyframeLine.SetPosition(numClicks, curPos);
                }

                numClicks++;
                lastPos = curPos;
            }
        }
    }

    public Vector3[] GetPathPoints()
    {
        if (_currLine == null) return null;  // no path is drawn

        Vector3[] pos;
        if (!addAnimation.insertKeyframe || _currKeyframeLine == null)
        {
            pos = new Vector3[_currLine.positionCount];
            _currLine.GetPositions(pos);
        }
        else
        {
            pos = new Vector3[_currKeyframeLine.positionCount];
            _currKeyframeLine.GetPositions(pos);
        }

        return pos;
    }

    public Vector3[] GetPathKeyframe()
    {
        Vector3[] pos = new Vector3[_currKeyframeLine.positionCount];
        _currKeyframeLine.GetPositions(pos);
        return pos;
    }
}
