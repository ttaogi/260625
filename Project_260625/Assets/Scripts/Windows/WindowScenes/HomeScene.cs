using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HomeScene : WindowScene
{
    #region Inspector
    #endregion Inspector

    public override eScene Scene { get { return eScene.Home; } }

    private readonly Tuple<int, int> CHAR_POS_INIT = Tuple.Create(0, 0);

    #region Open
    protected override IEnumerator LoadObjects()
    {
        List<AsyncOperationHandle<GameObject>> handles = new();

        AsyncOperationHandle<GameObject> handleChar = Utils.LoadPrefab(Constants.ADDR_HERO);
        handles.Add(handleChar);

        while(true)
        {
            bool isLoaded = true;

            foreach (var handle in handles)
                if (handle.IsDone == false)
                {
                    isLoaded = false;
                    break;
                }

            if (isLoaded)
                break;
            else
                yield return null;
        }

        foreach (var handle in handles)
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                EntityId id = handle.Result.GetEntityId();

                if (_handles.ContainsKey(id))
                {
                    (var oldHandle, var oldObj) = _handles[id];

                    _handles.Remove(id);

                    Utils.UnloadPrefab(oldHandle, oldObj);
                }

                GameObject obj = Instantiate(handle.Result);
                obj.SetActive(true);

                _handles.Add(id, new(handle, obj));

                // Camera setting.
                if (handle.Result == handleChar.Result && handleChar.IsValid())
                {
                    CameraManager.Instance.SetTarget(obj);

                    if (handleChar.Result != null && handleChar.Result.TryGetComponent<Hero>(out var hero))
                        hero.SetPos(CHAR_POS_INIT);
                }
            }

        yield break;
    }
    #endregion Open

    #region Close
    protected override void UnloadObjects()
    {
        foreach (var tuple in _handles)
            Utils.UnloadPrefab(tuple.Value.Item1, tuple.Value.Item2);
    }
    #endregion Close
}
