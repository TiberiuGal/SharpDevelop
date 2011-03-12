﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Calculates diff of 2 <see cref="PositionedGraph"/>s.
	/// </summary>
	public class GraphMatcher
	{
		public GraphMatcher()
		{
		}
		
		public GraphDiff MatchGraphs(PositionedGraph oldGraph, PositionedGraph newGraph)
		{
			if (oldGraph == null)
			{
				if (newGraph == null)
				{
					return new GraphDiff();
				}
				else
				{
					GraphDiff addAllDiff = new GraphDiff();
					foreach	(PositionedNode newNode in newGraph.Nodes)
						addAllDiff.SetAdded(newNode);
					return addAllDiff;
				}
			}
			else if (newGraph == null)
			{
				GraphDiff removeAllDiff = new GraphDiff();
				foreach	(PositionedNode oldNode in oldGraph.Nodes)
					removeAllDiff.SetRemoved(oldNode);
				return removeAllDiff;
			}
			
			// none of the graphs is null
			GraphDiff diff = new GraphDiff();
			
			Dictionary<int, PositionedNode> newNodeForHashCode = buildHashToNodeMap(newGraph);
			Dictionary<PositionedNode, bool> newNodeMatched = new Dictionary<PositionedNode, bool>();
			
			foreach	(PositionedNode oldNode in oldGraph.Nodes)
			{
				PositionedNode matchingNode = matchNode(oldNode, newNodeForHashCode);
				if (matchingNode != null)
				{
					diff.SetMatching(oldNode, matchingNode);
					newNodeMatched[matchingNode] = true;
				}
				else
				{
					diff.SetRemoved(oldNode);
				}
			}
			foreach	(PositionedNode newNode in newGraph.Nodes)
			{
				if (!newNodeMatched.ContainsKey(newNode))
				{
					diff.SetAdded(newNode);
				}
			}
			
			return diff;
		}
		
		private Dictionary<int, PositionedNode> buildHashToNodeMap(PositionedGraph graph)
		{
			var hashToNodeMap = new Dictionary<int, PositionedNode>();
			foreach (PositionedNode node in graph.Nodes)
			{
				hashToNodeMap[node.ObjectNode.HashCode] = node;
			}
			return hashToNodeMap;
		}
		
		private PositionedNode matchNode(PositionedNode oldNode, Dictionary<int, PositionedNode> newNodeMap)
		{
			PositionedNode newNodeFound = newNodeMap.GetValue(oldNode.ObjectNode.HashCode);
			if ((newNodeFound != null) && isSameAddress(oldNode, newNodeFound))
			{
				return newNodeFound;
			}
			else
			{
				return null;
			}
		}
		
		private bool isSameAddress(PositionedNode node1, PositionedNode node2)
		{
			return node1.ObjectNode.PermanentReference.GetObjectAddress() == node2.ObjectNode.PermanentReference.GetObjectAddress();
		}
	}
}
