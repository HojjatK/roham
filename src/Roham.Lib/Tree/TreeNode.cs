using System.Collections.Generic;
using System.Linq;

namespace Roham.Lib.Tree
{
    public class TreeNode<T>
    {
        private readonly HashSet<TreeNode<T>> _children = new HashSet<TreeNode<T>>();

        public TreeNode(T value)
        {
            Value = value;
            Parent = null;
        }

        public T Value { get; set; }
        public TreeNode<T> Parent { get; protected set; }
        public int ChildrenCount => _children.Count;
        public IEnumerable<TreeNode<T>> Children => _children;

        public void AddChild(TreeNode<T> childNode)
        {
            childNode.Parent = this;
            _children.Add(childNode);
        }

        public void RemoveChild(TreeNode<T> childNode)
        {
            _children.RemoveAny(_children.Where(c => childNode.Equals(c)));
        }

        public void Clear()
        {
            _children.Clear();
        }
    }
}