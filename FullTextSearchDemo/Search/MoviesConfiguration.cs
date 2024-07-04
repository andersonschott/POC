using FullTextSearchDemo.Models;
using FullTextSearchDemo.SearchEngine.Configuration;

namespace FullTextSearchDemo.Search;

public class MoviesConfiguration : IIndexConfiguration<Ativo>
{
    public string IndexName => "movies-index";

    public FacetConfiguration<Ativo>? FacetConfiguration => new()
    {
        IndexName = "movies-index-facets",
        MultiValuedFields = new[] { nameof(Ativo.EspecPapel) }
    };
}