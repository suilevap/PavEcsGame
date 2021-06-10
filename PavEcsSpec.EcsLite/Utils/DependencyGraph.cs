using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsSpec.EcsLite
{
    internal class DependencyGraph<TK,TV>
        where TK : class
        where TV : class
    {
        private class Node<T1,T2>
        {
            public T1 Value;
            public HashSet<Node<T2, T1>> Incomming { get; private set; }
            public HashSet<Node<T2, T1>> Outgoing { get; private set; }

            public Node()
            {
                Incomming = new HashSet<Node<T2, T1>>();
                Outgoing = new HashSet<Node<T2, T1>>();
            }
        }

        private Dictionary<TK, Node<TK, TV>> _nodes;
        private Dictionary<TV, Node<TV, TK>> _links;

        private static Node<T1,T2> GetNode<T1,T2>(Dictionary<T1,Node<T1,T2>> dict, T1 key)
        {
            if (!dict.TryGetValue(key, out var node))
            {
                node = new Node<T1, T2>()
                {
                    Value = key
                };
                dict.Add(key, node);
            }
            return node;
        }

        public DependencyGraph()
        {
            _nodes = new Dictionary<TK, Node<TK, TV>>();
            _links = new Dictionary<TV, Node<TV, TK>>();
        }
        
        public void AddIncommingConnections(TK key, IEnumerable<TV> dependencies)
        {
            var node = GetNode(_nodes, key);
            foreach (var d in dependencies)
            {
                var link = GetNode(_links, d);

                node.Incomming.Add(link);
                link.Outgoing.Add(node);
            }
        }

        public void AddOutgoingConnections(TK key, IEnumerable<TV> values)
        {
            var node = GetNode(_nodes, key);
            foreach (var d in values)
            {
                var link = GetNode(_links, d);

                node.Outgoing.Add(link);
                link.Incomming.Add(node);
            }
        }


        public IEnumerable<TV> GetIncommingConnections(TK key)
        {
            if (_nodes.TryGetValue(key, out var node))
            {
                foreach (var link in node.Incomming)
                {
                    yield return link.Value;
                }
            }
        }


        public IEnumerable<TV> GetOutgoingConnections(TK key)
        {
            if (_nodes.TryGetValue(key, out var node))
            {
                foreach (var link in node.Outgoing)
                {
                    yield return link.Value;
                }
            }
        }

        public IEnumerable<TK> GetDependencies(TK key)
        {
            if (_nodes.TryGetValue(key, out var node))
            {
                foreach (var link in node.Incomming)
                {
                    foreach (var producer in link.Incomming)
                    {
                        if (producer != node)
                        {
                            yield return producer.Value;
                        }
                    }
                }
            }
        }

        public IEnumerable<(TK key, IEnumerable<TK> deps)> GetAllDeps()
        {
            foreach (var key in _nodes.Keys)
            {
                yield return (key, GetDependencies(key));
            }
        }
    }
}
