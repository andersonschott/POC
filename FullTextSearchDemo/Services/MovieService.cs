using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public class MovieService : IMovieService
{
    private readonly ISearchEngine<Ativo> _searchEngine;

    public MovieService(ISearchEngine<Ativo> searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public SearchResult<Ativo> GetMovies(GetMoviesQuery query)
    {
        var searchFields = new Dictionary<string, string?>();

        if (query.codNeg != null)
        {
            searchFields.Add(nameof(query.codNeg), query.codNeg);
        }

        if (query.NomeCurto != null)
        {
            searchFields.Add(nameof(query.NomeCurto), query.NomeCurto);
        }
       
        var facets = GetFacets(query);

        return _searchEngine.Search(new FieldSpecificSearchQuery
        {
            SearchTerms = searchFields,
            //Type = SearchType.ExactMatch,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    public SearchResult<Ativo> SearchMovies(SearchMovieQuery query)
    {
        var facets = GetFacets(query);

        return _searchEngine.Search(new AllFieldsSearchQuery
        {
            SearchTerm = query.Term,
            Type = SearchType.PrefixMatch,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    public SearchResult<Ativo> FullTextSearchMovies(SearchMovieQuery query)
    {
        var facets = GetFacets(query);
        return _searchEngine.Search(new FullTextSearchQuery
        {
            SearchTerm = query.Term,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Facets = facets
        });
    }

    private static IDictionary<string, IEnumerable<string?>?> GetFacets(MoviesQuery query)
    {
        var facets = new Dictionary<string, IEnumerable<string?>?>();
        if (query.FacetGenreFacets != null)
        {
            facets.Add(nameof(Ativo.EspecPapel), query.FacetGenreFacets);
        }

        if (query.TitleTypeFacets != null)
        {
            facets.Add(nameof(Movie.TitleType), query.TitleTypeFacets);
        }

        return facets;
    }
}