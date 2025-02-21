using SkillEditorDemo.Model;
using SkillEditorDemo.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeNode.Utility;
using UnityEngine;
using Debug = SkillEditorDemo.Utility.Debug;

namespace SkillEditorDemo.View
{
    public class GoPool : ObjPool<GameObject>
    {
        readonly Transform Parent;
        readonly GameObject Obj;
        public GoPool(GameObject go, Transform parent = null, Action<GameObject> onGet = null, Action<GameObject> onRelease = null) : base(onGet, onRelease)
        {
            Parent = parent;
            Obj = go;
        }
        protected override GameObject New()
        {
            if (Obj == null) { return null; }
            GameObject @object = GameObject.Instantiate(Obj);
            if (Parent != null)
            {
                @object.transform.SetParent(Parent);
            }
            return @object;
        }
    }




    public class GoPools : Singleton<GoPools>
    {
        public static GameObject Get(string id)
        {
            return Inst.GetGameObject(id);
        }
        readonly Dictionary<string, GoPool> pools = new();
        GameObject GetGameObject(string id)
        {
            GoPool pool = GetPool(id);
            if (pool == null) { return null; }
            return pool.Get();
        }

        GoPool GetPool(string id)
        {
            if (pools.TryGetValue(id, out GoPool pool))
            {
                return pool;
            }
            GameObject prefab = Resources.Load<GameObject>($"Prefab/{id}");
            if (null == prefab)
            {
                Debug.LogError("Can't find prefab: " + id);
                pools[id] = null;
                return null;
            }
            pool = new(prefab, null, (go) => go.transform.localScale = Vector3.one, (go) => go.transform.localScale = Vector3.zero);
            pools[id] = pool;
            return pool;
        }

        public static void Release(string id, GameObject go, int delay_ms = 0)
        {
            GoPool pool = Inst.GetPool(id);
            if (pool == null) { return; }
            if (delay_ms >= 0)
            {
                Task.Delay(delay_ms).ContinueWith(
                   (_) => MainThread.Post(() => pool.Release(go)));
            }
            else
            {
                pool.Release(go);
            }
        }

    }
}
