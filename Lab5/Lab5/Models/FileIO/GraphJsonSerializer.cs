using Newtonsoft.Json;

namespace Lab5.Models.FileIO;

public class GraphJsonSerializer
{
    public static string SerializeGraph(Graph graph)
    {
        return JsonConvert.SerializeObject(graph);
    }

    public static Graph DeserializeGraph(string json)
    {
        return JsonConvert.DeserializeObject<Graph>(json);
    }
}