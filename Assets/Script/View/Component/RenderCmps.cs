using UnityEngine;

namespace SkillEditorDemo.View
{
    public struct RenderCmp
    {
        public Transform Transform;
        public GameObject GameObject;

        public RenderCmp(Transform transform)
        {
            Transform = transform;
            GameObject = transform.gameObject;
        }
        public RenderCmp(GameObject gameObject)
        {
            Transform = gameObject.transform;
            GameObject = gameObject;
        }
    }
}
