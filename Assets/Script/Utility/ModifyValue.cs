using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SkillEditorDemo.Utility
{
    public struct ModifyValue : IEquatable<ModifyValue>, IEqualityComparer<ModifyValue>
    {
        public decimal Base;
        public decimal Mult;
        public readonly decimal Value => Base * (Mult + 1);
        public static ModifyValue Zero = new();
        public static ModifyValue operator +(ModifyValue a, ModifyValue b) => new(a.Base + b.Base, a.Mult + b.Mult);
        public static ModifyValue operator -(ModifyValue a, ModifyValue b) => new(a.Base - b.Base, a.Mult - b.Mult);
        public static ModifyValue operator -(ModifyValue a) => new(-a.Base, -a.Mult);
        public static ModifyValue operator *(ModifyValue a, float b) => new(a.Base * (decimal)b, a.Mult * (decimal)b);
        public static ModifyValue operator *(ModifyValue a, decimal b) => new(a.Base * b, a.Mult * b);

        public static implicit operator ModifyValue(float baseValue) => new(baseValue, 0);
        public static implicit operator ModifyValue(decimal baseValue) => new(baseValue, 0);
        public ModifyValue(float b, float m)
        {
            Base = (decimal)b;
            Mult = (decimal)m;
        }
        public ModifyValue(decimal b, decimal m)
        {
            Base = b;
            Mult = m;
        }
        public bool Equals(ModifyValue other) => Base == other.Base && Mult == other.Mult;
        public bool Equals(ModifyValue x, ModifyValue y) => x.Equals(y);
        public int GetHashCode([DisallowNull] ModifyValue obj) => obj.Base.GetHashCode() ^ obj.Mult.GetHashCode();
    }
}
