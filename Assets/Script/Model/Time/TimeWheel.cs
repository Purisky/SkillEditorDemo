using System.Collections.Generic;
using TreeNode.Utility;

namespace SkillEditorDemo.Model
{

    public class TimeWheels : Singleton<TimeWheels>
    {
        readonly List<IStepNext> List = new ();
        public static void Add(IStepNext stepNext) => Inst.List.Add(stepNext);
        public static void StepNext()=> Inst.List.ForEach(n => n.StepNext());
    }
    public class TimeWheel<T> : IStepNext where T : struct
    {
        public const int WheelLength = 60;
        int CurrentTick;
        readonly List<WheelSection<T>>[] Wheel;
        List<T> output = new ();
        public List<T> Output => output;
        public TimeWheel(int start = 0)
        {
            CurrentTick = start;
            Wheel = new List<WheelSection<T>>[WheelLength];
            for (int i = 0; i < WheelLength; i++)
            {
                Wheel[i] = new ();
            }
            TimeWheels.Add(this);
        }
        public WheelSection<T> Add(int delay, T index)
        {
            delay = (delay >= 0 ? delay : 0);
            int pos = (CurrentTick + delay) % WheelLength;
            WheelSection<T> wheelSection = new ()
            {
                Pos = pos,
                Index = index,
                WheelCount = (delay % WheelLength == 0) ? (delay / WheelLength) - 1 : (delay / WheelLength),
            };
            Wheel[pos].Add(wheelSection);
            return wheelSection;
        }
        public void Remove(WheelSection<T> wheelSection)
        {
            Wheel[wheelSection.Pos].Remove(wheelSection);
        }
        public void StepNext()
        {
            CurrentTick++;
            output.Clear();
            List<WheelSection<T>> delete = new ();
            List<WheelSection<T>> current = Wheel[CurrentTick % WheelLength];
            foreach (var item in current)
            {
                if (item.WheelCount <= 0)
                {
                    Output.Add(item.Index);
                    delete.Add(item);
                }
                else
                {
                    item.WheelCount--;
                }
            }
            foreach (var del in delete)
            {
                current.Remove(del);
            }
        }
    }
    public interface IStepNext
    {
        void StepNext();
    }
    public class WheelSection<T>where T : struct
    {
        public int Pos;
        public int WheelCount;
        public T Index;
    }

}
