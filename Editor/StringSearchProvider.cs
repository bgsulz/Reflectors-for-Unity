using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StringSearchProvider : ScriptableObject, ISearchWindowProvider
{
    private string[] _listItems;
    private Action<string> _selected;

    public void Construct(string[] listItems, Action<string> selected, bool forceInitialize = false)
    {
        _listItems = listItems;
        if (forceInitialize || _selected == null) _selected = selected;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var entries = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Members")) };

        var knownGroups = new List<string>();
        foreach (var item in _listItems)
        {
            var splitName = item.Split('.');

            entries.AddRange(GroupEntries(splitName, ref knownGroups));

            entries.Add(new SearchTreeEntry(new GUIContent(item))
            {
                level = splitName.Length,
                userData = item
            });
        }

        return entries;
    }

    private static List<SearchTreeGroupEntry> GroupEntries(string[] splitName, ref List<string> knownGroups)
    {
        var groups = new List<SearchTreeGroupEntry>();

        var groupName = string.Empty;
        for (var i = 0; i < splitName.Length - 1; i++)
        {
            groupName += splitName[i];
            if (!knownGroups.Contains(groupName))
            {
                groups.Add(new SearchTreeGroupEntry(new GUIContent(groupName), i + 1));
                knownGroups.Add(groupName);
            }
            groupName += "/";
        }

        return groups;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _selected?.Invoke(searchTreeEntry.userData as string);
        return true;
    }
}