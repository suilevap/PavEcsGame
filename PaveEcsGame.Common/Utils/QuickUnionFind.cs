using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaveEcsGame.Utils
{
    public class QuickUnionFind<T>
    {
        private readonly Dictionary<T, int> _map = new Dictionary<T, int>();

        private readonly List<int> _parentId = new List<int>();

        private readonly List<int> _size = new List<int>();

        private int _superSetCount = 0;

        private int GetId(T item)
        {
            if (!_map.TryGetValue(item, out var id))
            {
                id = _map.Count;
                _parentId.Add(id);
                _size.Add(1);
                _map[item] = id;
                _superSetCount++;
            }

            return id;
        }

        public int Root(T item)
        {
            var id = GetId(item);
            return RootInternal(id);
        }

        private int RootInternal(int id)
        {
            var parentId = _parentId[id];

            while (id != parentId)
            {
                parentId = _parentId[parentId];
                _parentId[id] = parentId;
                id = parentId;

                parentId = _parentId[parentId];
            }
            return id;
        }


        public bool IsConnected(T p, T q) 
        {
            return Root(p) == Root(q);
        }

        public void Union(T p, T q)
        {
            var id1 = GetId(p);
            var id2 = GetId(q);
            UnionInternal(id1, id2);
        }

        private void UnionInternal(int id1, int id2)
        {
            int rootId1 = RootInternal(id1);
            int rootId2 = RootInternal(id2);

            if (rootId1 == rootId2) return;

            if (_size[rootId1] < _size[rootId2])
            {
                _parentId[rootId1] = rootId2;
                _size[rootId2] += _size[rootId1]; // add the count of the smaller array to the count of the larger array
            }
            else // if j is smaller than i
            {
                _parentId[rootId2] = rootId1;
                _size[rootId1] += _size[rootId2];
            }

            _superSetCount--; // if we combine components, the count of components goes down by 1
        }

        public void Union(IReadOnlyList<T> items)
        {
            var id1 = GetId(items[0]);
            for (int i = 1; i < items.Count; i++)
            {
                UnionInternal(id1, GetId(items[i]));
            }
        }

        public IEnumerable<int> GetAllRoots() => _parentId.Distinct();
    }
}
