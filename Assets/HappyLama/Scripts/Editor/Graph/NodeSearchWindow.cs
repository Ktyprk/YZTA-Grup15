using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HappyLama
{
    public class NodeSearchWindow : ScriptableObject,ISearchWindowProvider
    {
        private EditorWindow _window;
        private StoryGraphView _graphView;
        private Texture2D _indentationIcon;
        
        public void Configure(EditorWindow window,StoryGraphView graphView)
        {
            _window = window;
            _graphView = graphView;

            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return new List<SearchTreeEntry>
    {
        new SearchTreeGroupEntry(new GUIContent("Add Node"), 0),
        new SearchTreeEntry(new GUIContent("NPC Message", _indentationIcon))
        {
            level = 1,
            userData = "NPC"
        },
        new SearchTreeEntry(new GUIContent("Player Response", _indentationIcon))
        {
            level = 1,
            userData = "player"
        },
        new SearchTreeEntry(new GUIContent("End Node", _indentationIcon))
        {
            level = 1,
            userData = "end"
        },
        new SearchTreeEntry(new GUIContent("Comment Block", _indentationIcon))
        {
            level = 1,
            userData = "comment"
        }
    };
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(
                _window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);

            if (entry.userData.ToString() == "comment")
            {
                var rect = new Rect(graphMousePosition, _graphView.DefaultCommentBlockSize);
                _graphView.CreateCommentBlock(rect);
                return true;
            }
            else
            {
                _graphView.CreateNewNode(entry.userData.ToString(), graphMousePosition);
                return true;
            }
        }

    }
}