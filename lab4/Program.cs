using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var g = BuildGraph();

            var f = GetMaxFlow(g);
            Console.WriteLine("Max flow = " + f);
            foreach (var edge in g.GetEdges())
                Console.WriteLine(edge);
            Console.ReadKey();
        }

        public static int[][] SAdjacencyMatrix = new int[][]
        {
            
            new[] { 0, 20, 20, 40, 0, 0, 0, 0 },
            new[] { 0, 0, 10, 0, 10, 0, 0, 0 },
            new[] { 0, 0, 0, 20, 20, 0, 0, 0 },
            new[] { 0, 0, 0, 0, 0, 20, 20, 0 },
            new[] { 0, 0, 0, 0, 0, 0, 0, 30 },
            new[] { 0, 0, 10, 0, 20, 0, 0, 20 },
            new[] { 0, 0, 0, 0, 0, 10, 0, 20 }
            new[] { 0, 0, 0, 0, 0, 0, 0 }

        }; 

        public static Dictionary<int, string> SIdDictionary = new Dictionary<int, string>();

        static Program()
        {
            SIdDictionary.Add(0, "A1");
            SIdDictionary.Add(1, "A2");
            SIdDictionary.Add(2, "A3");
            SIdDictionary.Add(3, "A4");
            SIdDictionary.Add(4, "A5");
            SIdDictionary.Add(5, "A6");
            SIdDictionary.Add(6, "A7");
        }

        public static string Id2String(int id)
        {
            return SIdDictionary[id];
        }

        public class DistanceLabels
        {

            public static Dictionary<Node, int> Labels;


            public static int[] Nodes;


            public DistanceLabels(int n)
            {
                Labels = new Dictionary<Node, int>();
                Nodes = new int[n + 1];
            }


            public int GetLabel(Node n)
            {
                return Labels[n];
            }



            public bool SetLabel(Node n, int label)
            {
                var existsUnassignedLabel = false;

                if (Labels.ContainsKey(n))
                {
                    var oldLabel = Labels[n];
                    Nodes[oldLabel]--;
                    if (Nodes[oldLabel] == 0) existsUnassignedLabel = true;
                    Labels[n] = label;
                }
                else
                {
                    Labels.Add(n, label);
                }
                Nodes[label]++;
                return existsUnassignedLabel;
            }
        }

        public static double GetMaxFlow(Graph g)
        {
            if (g.NumNodes() == 0)
            {
                return 0;
            }

            var labels = CalcDistanceLabels(g);
            double f = 0;
            var n = g.NumNodes();
            var backEdges = AddBackEdges(g);
            var path = new List<Edge>();
            var i = g.GetSource();


            while (i != null && labels.GetLabel(g.GetSource()) < n)
            {
                var e = GetAdmissibleEdge(g, i, labels);
                if (e != null)
                {
                    i = Advance(e, path);
                    if (!i.@equals(g.GetSink())) continue;
                    var delta = Augment(g, path);
                    f += delta;
                    i = g.GetSource();
                    path.Clear();
                }
                else i = Retreat(g, labels, i, path);
            }

            RemoveBackEdges(g, backEdges);

            return f;
        }

        public static DistanceLabels CalcDistanceLabels(Graph g)
        {
            var n = g.NumNodes();
            var labels = new DistanceLabels(n);

            var visited = new HashSet<Node>();

            foreach (var i in g.Nodes)
            {
                labels.SetLabel(i, n);
            }

            labels.SetLabel(g.GetSink(), 0);

            var queue = new List<Node> {g.GetSink()};

            while (queue.Count != 0)
            {
                var j = queue[0];
                queue.RemoveAt(0);

                foreach (var e in g.Incident(j))
                {
                    var i = e.GetSource();
                    if (visited.Contains(i)) continue;
                    labels.SetLabel(i, labels.GetLabel(j) + 1);
                    visited.Add(i);
                    queue.Add(i);
                }
                visited.Add(j);
            }

            return labels;
        }


        public static List<Edge> AddBackEdges(Graph g)
        {
            var backEdges = new List<Edge>();
            foreach (var e in g.GetEdges())
            {
                var backEdge = new Edge(e.GetDest(), e.GetSource(), 0, 0);
                g.AddEdge(backEdge);
                backEdges.Add(backEdge);
            }
            return backEdges;
        }


        public static Edge GetAdmissibleEdge(Graph g, Node i, DistanceLabels d)
        {
            return g.Adjacent(i).FirstOrDefault(e => e.GetResidualCap() > 0 && d.GetLabel(i) == 1 + d.GetLabel(e.GetDest()));
        }
        
        public static Node Advance(Edge e, List<Edge> path)
        {
            path.Add(e);
            return e.GetDest();
        }

        public static double Augment(Graph g, List<Edge> path)
        {
            var delta = path.Select(e => e.GetResidualCap()).Concat(new[] {double.MaxValue}).Min();

            foreach (var e in path)
            {
                var flow = e.GetFlow();
                flow += delta;
                e.SetFlow(flow);

                var revEdge = g.Incident(e.GetSource()).FirstOrDefault(incEdge => incEdge.GetSource().@equals(e.GetDest()));

                if (revEdge != null)
                {
                    var cap = revEdge.GetCap();
                    cap += delta;
                    revEdge.SetCap(cap);
                }
                if (revEdge == null) continue;
                flow = revEdge.GetFlow();
                if (!(flow > 0)) continue;
                flow -= delta;
                revEdge.SetFlow(flow);
            }
            return delta;
        }


        public static Node Retreat(Graph g, DistanceLabels labels, Node i, List<Edge> path)
        {

            var dMin = g.NumNodes() - 1;

            dMin = (from e in g.Adjacent(i) where e.GetResidualCap() > 0 select e.GetDest() into j select labels.GetLabel(j)).Concat(new[] {dMin}).Min();

            var flag = labels.SetLabel(i, 1 + dMin);

            Node predecessor;
            if (!flag)
            {

                if (!i.equals(g.GetSource()))
                {
                    Edge e = path[0];
                    path.RemoveAt(0);
                    predecessor = e.GetSource();
                }
                else predecessor = g.GetSource();
            }
            else predecessor = null;

            return predecessor;
        }

        public static void RemoveBackEdges(Graph g, List<Edge> backEdges)
        {
            foreach (var e in backEdges)
            {
                g.RemoveEdge(e);
            }
        }


        public static Graph BuildGraph()
        {
            var graph = new Graph();

            var nodeDictionary = new Dictionary<int, Node>();
            var edgeList = new List<Edge>();

            for (var i = 0; i < SAdjacencyMatrix.Length; i++)
            {
                var node = new Node(i, Id2String(i));
                nodeDictionary.Add(i, node);
                graph.AddNode(node);
            }


            graph.SetSource(nodeDictionary[0]);
            graph.SetSink(nodeDictionary[nodeDictionary.Count - 1]);

            for (var i = 0; i < SAdjacencyMatrix.Length; i++)
            {
                for (var j = 0; j < SAdjacencyMatrix[i].Length; j++)
                {
                    if (SAdjacencyMatrix[i][j] > 0)
                    {
                        var edge = new Edge(nodeDictionary[i], nodeDictionary[j], SAdjacencyMatrix[i][j]);
                        edgeList.Add(edge);
                    }
                }
            }

            foreach (var edge in edgeList)
            {
                graph.AddEdge(edge);
            }

            return graph;
        }
    }

    public class Edge
    {
        public Node Source;
        public Node Dest;
        public double Cap;
        public double Flow;

        public Edge(Node source, Node dest, double cap)
        {
            Source = source;
            Dest = dest;
            Cap = cap;
        }

        public Edge(Node source, Node dest, double cap, double flow)
        {
            Source = source;
            Dest = dest;
            Cap = cap;
            Flow = flow;
        }

        public Edge(Edge e)
        {
            Source = e.Source;
            Dest = e.Dest;
            Cap = e.Cap;
            Flow = e.Flow;
        }


        public bool equals(object o)
        {
            if (o == null) return false;
            if (!(o.GetType() == typeof(Edge)))
            return false;
            var e = (Edge) o;
            return e.Source.equals(Source) && e.Dest.equals(Dest) && e.Flow == Flow && e.Cap == Cap;
        }

        public override string ToString()
        {
            return "(" + Source.Label + ", " + Dest.Label + ") [" + Flow + " / " + Cap + "]";
        }

        public double GetFlow()
        {
            return Flow;
        }

        public void SetFlow(double flow)
        {
            if (flow > Cap) throw new Exception("You can not assign a greater flow of capacity curves");
            Flow = flow;
        }

        public Node GetSource()
        {
            return Source;
        }

        public Node GetDest()
        {
            return Dest;
        }

        public double GetCap()
        {
            return Cap;
        }

        public void SetCap(double cap)
        {
            Cap = cap;
        }

        public double GetResidualCap()
        {
            return Cap - Flow;
        }

    }

    public class Node
    {
        public int Id;
        public string Label = "";

        public Node(int id)
        {
            Id = id;
        }

        public Node(int id, string label)
        {
            Id = id;
            Label = label;
        }

        public Node(Node n)
        {
            Id = n.GetId();
            if (n.Label != null) Label = n.Label + "";
        }

        public int GetId()
        {
            return Id;
        }

        public string GetLabel()
        {
            return Label;
        }

        public void SetLabel(string label)
        {
            Label = label;
        }


        public bool equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj.GetType() == typeof (Node))) return false;
            var n = (Node) obj;
            return n.Id == Id;
        }

        public int HashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return "Node [id = " + Id + ", label = " + Label + "]";
        }

    }

    public class Graph
    {

        public Dictionary<int, List<Edge>> Adjacencies = new Dictionary<int, List<Edge>>();
        public Dictionary<int, List<Edge>> Incidences = new Dictionary<int, List<Edge>>();
        private readonly Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
        public Node Source;
        public Node Sink;

        public void AddNode(Node n)
        {
            if (ContainsNode(n)) throw new Exception();
            _nodes.Add(n.Id, n);
            Adjacencies.Add(n.Id, new List<Edge>());
            Incidences.Add(n.Id, new List<Edge>());
        }


        public void AddEdge(Edge e)
        {
            if (!ContainsNode(e.Source) || !ContainsNode(e.Dest))
                throw new Exception();
            var adjacent = Adjacencies[e.Source.Id];
            var incident = Incidences[e.Dest.Id];
            adjacent.Add(e);
            incident.Add(e);
        }


        public void SetSource(Node node)
        {
            Source = node;
        }


        public Node GetSource()
        {
            return Source;
        }


        public void SetSink(Node node)
        {
            Sink = node;
        }


        public Node GetSink()
        {
            return Sink;
        }


        public int NumNodes()
        {
            return _nodes.Count;
        }


        public int NumEdges()
        {
            return Adjacencies.Values.Sum(adjList => adjList.Count);
        }


        public List<Edge> Adjacent(Node n)
        {
            return Adjacencies[n.Id];
        }


        public bool ContainsNode(Node n)
        {
            return _nodes.ContainsKey(n.Id);
        }


        public bool ContainsEdge(Edge e)
        {
            List<Edge> adjList = Adjacencies[e.Source.Id];
            return adjList.Contains(e);
        }


        public Node GetNode(int id)
        {
            return _nodes[id];
        }


        public List<Node> Nodes => _nodes.Values.ToList();

        public HashSet<int> NodesIDs => new HashSet<int>(_nodes.Keys);

        public List<Edge> GetEdges()
        {
            var edges = new List<Edge>();
            foreach (var adjList in Adjacencies.Values)
            {
                edges.AddRange(adjList);
            }

            return edges;
        }


        public void RemoveNode(Node n)
        {
            _nodes.Remove(n.Id);
            Adjacencies.Remove(n.Id);
            Incidences.Remove(n.Id);

            foreach (var adjList in Adjacencies.Values)
            {
                for (var i = 0; i < adjList.Count; i++)
                {
                    var e = adjList[i + 1];
                    if (!e.Dest.@equals(n)) continue;
                    adjList.RemoveAt(i);
                    break;
                }
            }

            foreach (var incList in Incidences.Values)
            {
                for (var i = 0; i < incList.Count; i++)
                {
                    var e = incList[i + 1];
                    if (!e.Dest.@equals(n)) continue;
                    incList.RemoveAt(i);
                    break;
                }
            }
        }


        public void RemoveEdge(Edge e)
        {
            var adjList = Adjacencies[e.Source.Id];
            var incList = Incidences[e.Dest.Id];
            adjList.Remove(e);
            incList.Remove(e);
        }


        public void Clear()
        {
            _nodes.Clear();
            Adjacencies.Clear();
            Incidences.Clear();
        }


        public List<Edge> Incident(Node n)
        {
            return Incidences[n.Id];
        }


        public object Clone()
        {
            var graph = new Graph();
            foreach (var n in Nodes)
            {
                var clonedNode = new Node(n);
                graph.Adjacencies.Add(n.Id, new List<Edge>());
                graph.Incidences.Add(n.Id, new List<Edge>());
                graph._nodes.Add(n.Id, clonedNode);

                if (n.equals(Source))
                {
                    graph.SetSource(clonedNode);
                }
                else if (n.equals(Sink))
                {
                    graph.SetSink(clonedNode);
                }
            }


            foreach (var n in Nodes)
            {
                var clonedAdjList = graph.Adjacencies[n.Id];

                foreach (var e in Adjacent(n))
                {
                    var clonedSrc = graph._nodes[e.Source.Id];
                    var clonedDest = graph._nodes[e.Dest.Id];
                    var clonedEdge = new Edge(clonedSrc, clonedDest, e.Cap, e.Flow);
                    clonedAdjList.Add(clonedEdge);
                    var clonedIncList = graph.Incidences[e.Dest.Id];
                    clonedIncList.Add(clonedEdge);
                }
            }


            return graph;
        }


        public Graph GetSubGraph(HashSet<int> s)
        {
            var subGraph = new Graph();

            foreach (var n in s)
            {
                var node = _nodes[n];
                var clonedNode = new Node(node);
                subGraph.AddNode(clonedNode);
            }

            if (Source != null)
            {
                var clonedSource = new Node(Source);
                subGraph.AddNode(clonedSource);
                subGraph.SetSource(clonedSource);
            }
            if (Sink != null)
            {
                var clonedSink = new Node(Sink);
                subGraph.AddNode(clonedSink);
                subGraph.SetSink(clonedSink);
            }

            foreach (int n in subGraph.NodesIDs)
            {
                var clonedAdjList = subGraph.Adjacencies[n];
                var node = _nodes[n];

                foreach (var e in Adjacent(node))
                {
                    if (subGraph.ContainsNode(e.Dest))
                    {
                        var clonedSrc = subGraph._nodes[e.Source.Id];
                        var clonedDest = subGraph._nodes[e.Dest.Id];
                        var clonedEdge = new Edge(clonedSrc, clonedDest, e.Cap, e.Flow);
                        clonedAdjList.Add(clonedEdge);
                        var clonedIncList = subGraph.Incidences[e.Dest.Id];
                        clonedIncList.Add(clonedEdge);
                    }
                }
            }
            return subGraph;
        }


        public override string ToString()
        {
            return "Adjacent: " + Adjacencies + "\nincidences: " + Incidences;
        }

    }
}

