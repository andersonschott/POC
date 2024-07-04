using System.Text.Json.Serialization;
using FullTextSearchDemo.SearchEngine.Facets;
using FullTextSearchDemo.SearchEngine.Models;

namespace FullTextSearchDemo.Models;

public class Entidade
{
    [JsonPropertyName("data")]
    public IDictionary<string, Ativo> data { get; set; }
}

public class Ativo: IDocument
{
    [JsonPropertyName("codNeg")]
    public string codNeg { get; set; }

    [JsonPropertyName("nomeCurto")]
    public string NomeCurto { get; set; }

    [JsonPropertyName("especPapel")]
    [FacetProperty]
    public string EspecPapel { get; set; }

    [JsonPropertyName("dataMax")]
    public int DataMax { get; set; }

    [JsonPropertyName("dataMin")]
    public int DataMin { get; set; }

    public string UniqueKey { get; } = DateTime.Now.ToFileTimeUtc().ToString();
}