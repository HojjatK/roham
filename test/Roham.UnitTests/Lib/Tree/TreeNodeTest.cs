using NUnit.Framework;
using System.Linq;

namespace Roham.Lib.Tree
{
    [TestFixture]
    [Category("UnitTests.Tree")]
    public class TreeNodeTest : UnitTestFixture
    {
        [Test]
        public void GivenANewTreeNode_ThenTreeNodeValueIsSet()
        {
            // Given
            var newNode = new TreeNode<int>(10);

            // Then
            Assert.AreEqual(10, newNode.Value);
        }

        [Test]
        public void GivenANewTreeNode_ThenTreeNodeParentIsNull()
        {
            // Given
            var newNode = new TreeNode<string>("root");

            // Then
            Assert.IsNull(newNode.Parent);
        }

        [Test]
        public void GivenNewTreeNode_ThenTreeNodeHasNoChild()
        {
            // Given
            var newNode = new TreeNode<string>("root");

            // Then
            Assert.AreEqual(0, newNode.ChildrenCount);
            Assert.IsFalse(newNode.Children.Any());
        }

        [Test]
        public void GivenATreeNode_WhenAnotherNodeIsAddedAsChild_ThenNodeWillBeParentOfAddedNode()
        {
            // Given
            var treeNode = new TreeNode<string>("parent");

            // When
            var childNode = new TreeNode<string>("child");
            treeNode.AddChild(childNode);

            // Then
            Assert.AreSame(treeNode, childNode.Parent);
        }

        [Test]
        public void GivenATreeNode_WhenADescendantNodeIsAdded_ThenNodeWillBeAncestorOfAddedNode()
        {
            // Given
            var root = new TreeNode<int>(1);
            TreeNode<int> childNode;
            root.AddChild(new TreeNode<int>(2));
            root.AddChild(childNode = new TreeNode<int>(3));
            root.AddChild(new TreeNode<int>(4));

            //When
            var grandChildNode = new TreeNode<int>(5);
            childNode.AddChild(grandChildNode);

            // Then
            Assert.AreSame(childNode, grandChildNode.Parent);
            Assert.AreSame(root, grandChildNode.Parent.Parent);
            Assert.IsNull(grandChildNode.Parent.Parent.Parent);
        }
    }
}
