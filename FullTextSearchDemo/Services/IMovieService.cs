using FullTextSearchDemo.Models;
using FullTextSearchDemo.Parameters;
using FullTextSearchDemo.SearchEngine.Results;

namespace FullTextSearchDemo.Services;

public interface IMovieService
{
    SearchResult<Ativo> GetMovies(GetMoviesQuery query);
    
    SearchResult<Ativo> SearchMovies(SearchMovieQuery query);
    
    SearchResult<Ativo> FullTextSearchMovies(SearchMovieQuery query);
}