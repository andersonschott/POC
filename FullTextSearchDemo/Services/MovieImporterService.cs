using System.Text.Json;
using FullTextSearchDemo.Models;
using FullTextSearchDemo.SearchEngine;
using FullTextSearchDemo.SearchEngine.Queries;
using FullTextSearchDemo.SearchEngine.Results;
using J2N.Collections.Generic;
using Lucene.Net.Index;

namespace FullTextSearchDemo.Services;

public class MovieImporterService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MovieImporterService(IServiceScopeFactory scopeFactory)
    {
        _serviceScopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var searchEngine = scope.ServiceProvider.GetRequiredService<ISearchEngine<Ativo>>();

        var result = new SearchResult<Ativo>()
        {
            TotalItems = 0
        };

        try
        {
            result = searchEngine.Search(new AllFieldsSearchQuery { Type = SearchType.ExactMatch });
        }
        catch (IndexNotFoundException ex)
        {
            //Ignore exception when index does not exist
            Console.WriteLine(ex);
        }

        if (result.TotalItems > 0)
        {
            return;
        }

        await ImportMoviesAsync(searchEngine, stoppingToken);
    }

    private static async Task ImportMoviesAsync(ISearchEngine<Ativo> searchEngine, CancellationToken stoppingToken)
    {
        var httpClient = new HttpClient();

        var result = await httpClient.GetAsync("https://cvscarlos.github.io/b3-api-dados-historicos/api/v1/tickers-cash-market.json", stoppingToken);
        var data = JsonSerializer.Deserialize<Entidade>(await result.Content.ReadAsStringAsync(cancellationToken: stoppingToken));

        // var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "title.basics.tsv");
        //
        // var index = 0;

        var startTime = DateTime.Now;
        // using var reader = new StreamReader(filePath);
        // var batch = new List<Movie>();
        // while (await reader.ReadLineAsync(stoppingToken) is { } line)
        // {
        //     //skip headers
        //     if (index == 0)
        //     {
        //         index++;
        //         continue;
        //     }
        //
        //     try
        //     {
        //         batch.Add(GetMovie(line));
        //     }
        //     catch
        //     {
        //         //skip invalid lines
        //     }
        //
        searchEngine.RemoveAll();
        
        var massa = data.data.Select(x => x.Value);
        Console.WriteLine($"Massa: {massa.Count()}");
        for (var i = 0; i < 1000; i++)
        {
            var dados = massa.Select(ativo => new Ativo()
                {
                    codNeg = ativo.codNeg.Split('-')[0] + "-" + i,
                    DataMax = ativo.DataMax,
                    DataMin = ativo.DataMin,
                    EspecPapel = ativo.EspecPapel,
                    NomeCurto = ativo.NomeCurto
                })
                .ToList();
            searchEngine.AddRange(dados);
            
            Console.WriteLine($"Indexing {i}/{1000}.");
        }
        //
        //     index++;
        //
        //     if (index > 2_000_000)
        //     {
        //         break;
        //     }
        // }

        var endTime = DateTime.Now;
        var duration = endTime - startTime;
        Console.WriteLine($"Indexing completed in {duration.TotalHours} hours.");
        Console.WriteLine($"Indexing completed in {duration.TotalMinutes} minutes.");
        Console.WriteLine($"Indexing completed in {duration.TotalSeconds} seconds.");
        //Console.WriteLine($"Indexed {index} movies.");

        //Avoid to keep in memory all the movies
        searchEngine.DisposeResources();
    }

    private static Movie GetMovie(string line)
    {
        var fields = line.Split('\t');

        if (fields.Length < 9)
        {
            Console.WriteLine($"Error: Insufficient fields - {line}");
            throw new Exception();
        }

        try
        {
            return new Movie
            {
                TConst = fields[0],
                TitleType = fields[1],
                PrimaryTitle = fields[2],
                OriginalTitle = fields[3],
                IsAdult = fields[4] == "1",
                StartYear = ParseInt(fields[5]),
                EndYear = ParseInt(fields[6]),
                RuntimeMinutes = ParseInt(fields[7]),
                Genres = fields[8].Split(','),
            };
        }
        catch
        {
            Console.WriteLine($"Error: {line}");
            throw;
        }
    }

    private static int ParseInt(string value)
    {
        var result = ParseNullableInt(value);
        return result ?? 0;
    }

    private static int? ParseNullableInt(string value)
    {
        if (value == @"\N" || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return null;
    }
}