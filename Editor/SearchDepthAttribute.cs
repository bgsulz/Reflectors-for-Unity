using UnityEngine;

namespace Extra.Editor.Properties
{
    public class SearchDepthAttribute : PropertyAttribute
    {
        public int SearchDepth { get; }
        public SearchDepthAttribute(int searchDepth = 1) => SearchDepth = searchDepth;
    }
}