using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    #region Enum
    private enum Direction { None, Left, Right, Up, Down }
    #endregion Enum


    private const float TILE_SIZE = 1.0f;
    private const float TILE_SIZE_HALF = 0.5f;
    private const float MOVING_TIME = 0.3f;

    private Tuple<int, int> _tilePos = new(0, 0);
    private Coroutine _coMoving = null;

    //////////////////////////////////////////////////

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInputValid() == false) return;

        if (IsMove()) return;

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
}
