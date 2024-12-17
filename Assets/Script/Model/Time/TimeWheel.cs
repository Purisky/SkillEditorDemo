using System.Collections.Generic;
using UnityEngine;

namespace SkillEditorDemo
{

    public class TimeWheels : Singleton<TimeWheels>
    {
        readonly List<IMoveNext> List = new ();
        public static void Add(IMoveNext moveNext) => Instance.List.Add(moveNext);
        public static void MoveNext()=> Instance.List.ForEach(n => n.MoveNext());
    }
    public class TimeWheel<T> : IMoveNext where T : struct
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
        public void MoveNext()
        {
            CurrentTick++;
            output = new List<T>();
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
    public interface IMoveNext
    {
        void MoveNext();
    }
    public class WheelSection<T>where T : struct
    {
        public int Pos;
        public int WheelCount;
        public T Index;
    }

}
