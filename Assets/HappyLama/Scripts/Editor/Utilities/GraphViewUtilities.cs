using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HappyLama
{
    public static class GraphViewUtilities
    {
        public static Port GetPortInstance(Node node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }
    }
}