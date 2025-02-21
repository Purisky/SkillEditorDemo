using System;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{
    public static class Events//use real event system if your project is complex
    {
        public static class OnChange
        {
            public static Action<int> Transform;
            public static Action<int, int> Skill;


        }

        public static class OnCreate
        {
            public static Action<int> Player;
            public static Action<int> Unit;
            public static Action<int> Projectile;
            public static Action<int> Hitbox;
        }
        public static class  OnDestroy
        {
            public static Action<int> Unit;
            public static Action<int> Projectile;
            public static Action<int> Hitbox;
        }
    }
}
