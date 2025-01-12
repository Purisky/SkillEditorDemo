using System.Collections.Generic;
using System.Numerics;

namespace SkillEditorDemo.Model
{
    public class Quadtree
    {
        private const int MaxObjects = 10;
        private const int MaxLevels = 5;

        private int _level;
        private List<IAABB> _objects;
        private AABB _bounds;
        private Quadtree[] _nodes;

        public Quadtree(int level, AABB bounds)
        {
            _level = level;
            _objects = new List<IAABB>();
            _bounds = bounds;
            _nodes = new Quadtree[4];
        }

        public void Update(ref IAABB obj)
        {
            if (!obj.Dirty) { return; }
            RemoveOld(obj);
            Insert(obj);
            obj.Dirty = false;
        }


        public void Clear()
        {
            _objects.Clear();
            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] != null)
                {
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        private void Split()
        {
            float subWidth = _bounds.Size.X / 2;
            float subHeight = _bounds.Size.Y / 2;
            float x = _bounds.Center.X - subWidth / 2;
            float y = _bounds.Center.Y - subHeight / 2;

            _nodes[0] = new Quadtree(_level + 1, new AABB { Center = new Vector2(x + subWidth, y), Size = new Vector2(subWidth, subHeight) });
            _nodes[1] = new Quadtree(_level + 1, new AABB { Center = new Vector2(x, y), Size = new Vector2(subWidth, subHeight) });
            _nodes[2] = new Quadtree(_level + 1, new AABB { Center = new Vector2(x, y + subHeight), Size = new Vector2(subWidth, subHeight) });
            _nodes[3] = new Quadtree(_level + 1, new AABB { Center = new Vector2(x + subWidth, y + subHeight), Size = new Vector2(subWidth, subHeight) });
        }

        private int GetIndex(AABB aabb)
        {
            int index = -1;
            float verticalMidpoint = _bounds.Center.X;
            float horizontalMidpoint = _bounds.Center.Y;

            bool topQuadrant = (aabb.Center.Y < horizontalMidpoint && aabb.Center.Y + aabb.Size.Y / 2 < horizontalMidpoint);
            bool bottomQuadrant = (aabb.Center.Y > horizontalMidpoint);

            if (aabb.Center.X < verticalMidpoint && aabb.Center.X + aabb.Size.X / 2 < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            else if (aabb.Center.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }

        public void Insert(IAABB obj)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(obj.AABB);

                if (index != -1)
                {
                    _nodes[index].Insert(obj);
                    return;
                }
            }

            _objects.Add(obj);

            if (_objects.Count > MaxObjects && _level < MaxLevels)
            {
                if (_nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < _objects.Count)
                {
                    int index = GetIndex(_objects[i].AABB);
                    if (index != -1)
                    {
                        _nodes[index].Insert(_objects[i]);
                        _objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public void Remove(IAABB obj)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(obj.AABB);

                if (index != -1)
                {
                    _nodes[index].Remove(obj);
                    return;
                }
            }

            _objects.Remove(obj);
        }
        public void RemoveOld(IAABB obj)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(obj.oldAABB);

                if (index != -1)
                {
                    _nodes[index].Remove(obj);
                    return;
                }
            }

            _objects.Remove(obj);
        }
        public List<IAABB> Retrieve(List<IAABB> returnObjects, AABB aabb)
        {
            int index = GetIndex(aabb);
            if (index != -1 && _nodes[0] != null)
            {
                _nodes[index].Retrieve(returnObjects, aabb);
            }

            returnObjects.AddRange(_objects);

            return returnObjects;
        }
    }
}