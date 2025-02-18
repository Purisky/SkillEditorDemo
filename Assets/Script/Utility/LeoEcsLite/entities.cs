// ----------------------------------------------------------------------------
// The MIT-Red License
// Copyright (c) 2012-2024 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using System.Runtime.CompilerServices;

#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite
{
    public struct EcsPackedEntity
    {
        public int Id;
        public int Gen;

        public override int GetHashCode()
        {
            unchecked
            {
                return (23 * 31 + Id) * 31 + Gen;
            }
        }
        public static readonly EcsPackedEntity Empty = new() { Id = -1, Gen = -1 };

        public static bool operator ==(in EcsPackedEntity a, in EcsPackedEntity b) => a.Id == b.Id && a.Gen == b.Gen;
        public static bool operator !=(in EcsPackedEntity a, in EcsPackedEntity b) => a.Id != b.Id || a.Gen != b.Gen;
        public override bool Equals(object obj) => obj is EcsPackedEntity other && Id == other.Id && Gen == other.Gen;
    }

    public struct EcsPackedEntityWithWorld
    {
        public int Id;
        public int Gen;
        public EcsWorld World;

        public override int GetHashCode()
        {
            unchecked
            {
                return ((23 * 31 + Id) * 31 + Gen) * 31 + (World?.GetHashCode() ?? 0);
            }
        }
#if DEBUG
        // For using in IDE debugger.
        internal object[] DebugComponentsView
        {
            get
            {
                object[] list = null;
                if (World != null && World.IsAlive() && World.IsEntityAliveInternal(Id) && World.GetEntityGen(Id) == Gen)
                {
                    World.GetComponents(Id, ref list);
                }
                return list;
            }
        }
        // For using in IDE debugger.
        internal int DebugComponentsCount
        {
            get
            {
                if (World != null && World.IsAlive() && World.IsEntityAliveInternal(Id) && World.GetEntityGen(Id) == Gen)
                {
                    return World.GetComponentsCount(Id);
                }
                return 0;
            }
        }

        // For using in IDE debugger.
        public override string ToString()
        {
            if (Id == 0 && Gen == 0) { return "Entity-Null"; }
            if (World == null || !World.IsAlive() || !World.IsEntityAliveInternal(Id) || World.GetEntityGen(Id) != Gen) { return "Entity-NonAlive"; }
            System.Type[] types = null;
            var count = World.GetComponentTypes(Id, ref types);
            System.Text.StringBuilder sb = null;
            if (count > 0)
            {
                sb = new System.Text.StringBuilder(512);
                for (var i = 0; i < count; i++)
                {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(types[i].Name);
                }
            }
            return $"Entity-{Id}:{Gen} [{sb}]";
        }
#endif
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public static class EcsEntityExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntity PackEntity(this EcsWorld world, int entity)
        {
            EcsPackedEntity packed;
            packed.Id = entity;
            packed.Gen = world.GetEntityGen(entity);
            return packed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(this in EcsPackedEntity packed, EcsWorld world, out int entity)
        {
            entity = packed.Id;
            return
                world != null
                && world.IsAlive()
                && world.IsEntityAliveInternal(packed.Id)
                && world.GetEntityGen(packed.Id) == packed.Gen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo(this in EcsPackedEntity a, in EcsPackedEntity b)
        {
            return a.Id == b.Id && a.Gen == b.Gen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EcsPackedEntityWithWorld PackEntityWithWorld(this EcsWorld world, int entity)
        {
            EcsPackedEntityWithWorld packedEntity;
            packedEntity.World = world;
            packedEntity.Id = entity;
            packedEntity.Gen = world.GetEntityGen(entity);
            return packedEntity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Unpack(this in EcsPackedEntityWithWorld packedEntity, out EcsWorld world, out int entity)
        {
            world = packedEntity.World;
            entity = packedEntity.Id;
            return
                world != null
                && world.IsAlive()
                && world.IsEntityAliveInternal(packedEntity.Id)
                && world.GetEntityGen(packedEntity.Id) == packedEntity.Gen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo(this in EcsPackedEntityWithWorld a, in EcsPackedEntityWithWorld b)
        {
            return a.Id == b.Id && a.Gen == b.Gen && a.World == b.World;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this int entity) where T : struct => ref EcsPool<T>.Pool.Add(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this int entity, T value) where T : struct
        {
            ref var c = ref EcsPool<T>.Pool.Add(entity);
            c = value;
            return ref c;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Del<T>(this int entity) where T : struct => EcsPool<T>.Pool.Del(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this EcsPackedEntity entity) where T : struct => ref entity.Id.Add<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this EcsPackedEntity entity, T value) where T : struct => ref entity.Id.Add(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Get<T>(this int entity) where T : struct => ref EcsPool<T>.Pool.Get(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Get<T>(this EcsPackedEntity entity) where T : struct => ref entity.Id.Get<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetAdd<T>(this int entity) where T : struct => ref EcsPool<T>.GetAdd(entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetAdd<T>(this EcsPackedEntity entity) where T : struct => ref entity.Id.GetAdd<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Set<T>(this int entity, T value) where T : struct => ref EcsPool<T>.SetAdd(entity, value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T Set<T>(this EcsPackedEntity entity, T value) where T : struct => ref entity.Id.Set(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? TryGet<T>(this EcsPackedEntity entity) where T : struct => entity.Id.TryGet<T>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? TryGet<T>(this int entity) where T : struct
        {
            if (entity >= 0 && EcsPool<T>.Pool.Has(entity))
            {
                return entity.Get<T>();
            }
            return null;
        }
        public static bool Exist<T>(this int entity) where T : struct => entity >= 0 && EcsPool<T>.Pool.Has(entity);
        public static bool Exist<T>(this EcsPackedEntity entity) where T : struct => entity.Id.Exist<T>();
    }
}
