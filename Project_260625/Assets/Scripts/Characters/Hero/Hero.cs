using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    #region Enum
    private enum Direction { None, Left, Right, Up, Down }
    private enum State { None, Idle, Move }
    #endregion Enum

    #region Inspector
    public Animator animator;
    #endregion Inspector

    private const float TILE_SIZE = 1.0f;
    private const float TILE_SIZE_HALF = 0.5f;
    private const float MOVING_TIME = 0.3f;

    private Tuple<int, int> _tilePos = new(0, 0);
    private Coroutine _coMoving = null;
    private Direction _preDir = Direction.Down;
    private State _preState = State.Idle;



    //////////////////////////////////////////////////

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetAnim(Direction.Down, State.Idle, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInputValid() == false) return;

        if (IsMove()) return;

        SetAnim(_preDir, State.Idle, false);

        //// local func ////

        bool IsInputValid()
        {
            return _coMoving == null;
        }

        bool IsMove()
        {
            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");

            if (hor != 0 || ver != 0)
            {
                Direction dir = Direction.None;

                if (hor < 0)
                    dir = Direction.Left;
                else if (hor > 0)
                    dir = Direction.Right;
                else if (ver > 0)
                    dir = Direction.Up;
                else if (ver < 0)
                    dir = Direction.Down;

                if (_coMoving != null)
                    StopCoroutine(_coMoving);

                _coMoving = StartCoroutine(CoMove(dir));

                SetAnim(dir, State.Move, false);

                return true;
            }
            else
                return false;
        }
    }

    private IEnumerator CoMove(Direction dir)
    {
        if (dir == Direction.None)
        {
            _coMoving = null;
            yield break;
        }


        switch (dir)
        {
            case Direction.Left:
                _tilePos = new(_tilePos.Item1 - 1, _tilePos.Item2);
                break;
            case Direction.Right:
                _tilePos = new(_tilePos.Item1 + 1, _tilePos.Item2);
                break;
            case Direction.Up:
                _tilePos = new(_tilePos.Item1, _tilePos.Item2 + 1);
                break;
            case Direction.Down:
                _tilePos = new(_tilePos.Item1, _tilePos.Item2 - 1);
                break;
        }

        Vector2 startPos = transform.localPosition;
        Vector2 endPos = new(_tilePos.Item1 * TILE_SIZE + TILE_SIZE_HALF, _tilePos.Item2 * TILE_SIZE + TILE_SIZE_HALF);
        float time = 0.0f;

        while (true)
        {
            time += Time.deltaTime;
            time = Mathf.Clamp(time, 0f, MOVING_TIME);

            Vector2 newPos = Vector2.Lerp(startPos, endPos, time / MOVING_TIME);

            transform.localPosition = newPos;

            if (time == MOVING_TIME)
                break;
            else
                yield return null;
        }

        _coMoving = null;
    }

    private void SetAnim(Direction dir, State state, bool isForce)
    {
        float dirX = dir == Direction.Right ? 1f : (dir == Direction.Left ? -1f : 0f);
        float dirY = dir == Direction.Up ? 1f : (dir == Direction.Down ? -1f : 0f);

        if (_preDir != dir || _preState != state || isForce)
        {
            animator.SetFloat("DirX", dirX);
            animator.SetFloat("DirY", dirY);
            animator.SetBool("Walk", state == State.Move);

            Utils.Log($"dir : {dir}, state : {state}, isForce : {isForce}");
        }

        if (_preDir != dir) _preDir = dir;
        if (_preState != state) _preState = state;
    }
}
