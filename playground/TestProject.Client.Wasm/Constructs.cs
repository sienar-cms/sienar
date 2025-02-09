using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Sienar.Data;
using Sienar.Services;

namespace TestProject.Client;

public class FootballClub
{
	[DisplayName("Team name")]
	public required string Name { get; set; }

	public string? Qualification { get; set; }

	[DisplayName("Total games")]
	public int TotalGames { get; set; }

	public int Wins { get; set; }
	public int Draws { get; set; }
	public int Losses { get; set; }

	[DisplayName("Goals for")]
	public int GoalsFor { get; set; }

	[DisplayName("Goals against")]
	public int GoalsAgainst { get; set; }
	public int Points { get; set; }
}

public class FootballClubRepo : IEntityReader<FootballClub>
{
	private ICollection<FootballClub> _clubs =
	[
		new FootballClub
		{
			Name = "Leicester City", TotalGames = 38, Wins = 23, Draws = 12, Losses = 3, GoalsFor = 68, GoalsAgainst = 36, Points = 81, Qualification = "Qualification for the Champions League group stage"
		},
		new FootballClub
		{
			Name = "Arsenal", TotalGames = 38, Wins = 20, Draws = 11, Losses = 7, GoalsFor = 65, GoalsAgainst = 36, Points = 71, Qualification = "Qualification for the Champions League group stage"
		},
		new FootballClub
		{
			Name = "Tottenham Hotspur", TotalGames = 38, Wins = 19, Draws = 13, Losses = 6, GoalsFor = 69, GoalsAgainst = 35, Points = 70, Qualification = "Qualification for the Champions League group stage"
		},
		new FootballClub
		{
			Name = "Manchester City", TotalGames = 38, Wins = 19, Draws = 9, Losses = 10, GoalsFor = 71, GoalsAgainst = 41, Points = 66, Qualification = "Qualification for the Champions League play-off round"
		},
		new FootballClub { Name = "Manchester United", TotalGames = 38, Wins = 19, Draws = 9, Losses = 10, GoalsFor = 49, GoalsAgainst = 35, Points = 66, Qualification = "Qualification for the Europa League group stage" },
		new FootballClub { Name = "Southampton", TotalGames = 38, Wins = 18, Draws = 9, Losses = 11, GoalsFor = 59, GoalsAgainst = 41, Points = 63, Qualification = "Qualification for the Europa League group stage" },
		new FootballClub { Name = "West Ham United", TotalGames = 38, Wins = 16, Draws = 14, Losses = 8, GoalsFor = 65, GoalsAgainst = 51, Points = 62, Qualification = "Qualification for the Europa League third qualifying round" },
		new FootballClub { Name = "Liverpool", TotalGames = 38, Wins = 16, Draws = 12, Losses = 10, GoalsFor = 63, GoalsAgainst = 50, Points = 60 },
		new FootballClub { Name = "Stoke City", TotalGames = 38, Wins = 14, Draws = 9, Losses = 15, GoalsFor = 41, GoalsAgainst = 55, Points = 51 },
		new FootballClub { Name = "Chelsea", TotalGames = 38, Wins = 12, Draws = 14, Losses = 12, GoalsFor = 59, GoalsAgainst = 53, Points = 50 },
		new FootballClub { Name = "Everton", TotalGames = 38, Wins = 11, Draws = 14, Losses = 13, GoalsFor = 59, GoalsAgainst = 55, Points = 47 },
		new FootballClub { Name = "Swansea City", TotalGames = 38, Wins = 12, Draws = 11, Losses = 15, GoalsFor = 42, GoalsAgainst = 52, Points = 47 },
		new FootballClub { Name = "Watford", TotalGames = 38, Wins = 12, Draws = 9, Losses = 17, GoalsFor = 40, GoalsAgainst = 50, Points = 45 },
		new FootballClub { Name = "West Bromwich Albion", TotalGames = 38, Wins = 10, Draws = 13, Losses = 15, GoalsFor = 34, GoalsAgainst = 48, Points = 43 },
		new FootballClub { Name = "Crystal Palace", TotalGames = 38, Wins = 11, Draws = 9, Losses = 18, GoalsFor = 39, GoalsAgainst = 51, Points = 42 },
		new FootballClub { Name = "AFC Bournemouth", TotalGames = 38, Wins = 11, Draws = 9, Losses = 18, GoalsFor = 45, GoalsAgainst = 67, Points = 42 },
		new FootballClub { Name = "Sunderland", TotalGames = 38, Wins = 9, Draws = 12, Losses = 17, GoalsFor = 48, GoalsAgainst = 62, Points = 39 },
		new FootballClub { Name = "Newcastle United", TotalGames = 38, Wins = 9, Draws = 10, Losses = 19, GoalsFor = 44, GoalsAgainst = 65, Points = 37, Qualification = "Relegation to the Football League Championship" },
		new FootballClub { Name = "Norwich City", TotalGames = 38, Wins = 9, Draws = 7, Losses = 22, GoalsFor = 39, GoalsAgainst = 67, Points = 34, Qualification = "Relegation to the Football League Championship" },
		new FootballClub { Name = "Aston Villa", TotalGames = 38, Wins = 3, Draws = 8, Losses = 27, GoalsFor = 27, GoalsAgainst = 76, Points = 17, Qualification = "Relegation to the Football League Championship" }
	];
	public Task<OperationResult<FootballClub?>> Read(Guid id, Filter? filter = null)
	{
		throw new NotImplementedException();
	}

	public async Task<OperationResult<PagedQuery<FootballClub>>> Read(Filter? filter = null)
	{
		// Simulate a network call
		await Task.Delay(2500);

		IEnumerable<FootballClub> clubs = _clubs;
		var result = new PagedQuery<FootballClub>{ TotalCount = _clubs.Count };

		if (filter is null)
		{
			result.Items = clubs.ToList();
			return new(result: result);
		}

		if (filter.Page > 1)
		{
			clubs = clubs.Skip((filter.Page - 1) * filter.PageSize);
		}

		if (!string.IsNullOrEmpty(filter.SearchTerm))
		{
			clubs = clubs.Where(c => c.Name.Contains(filter.SearchTerm));
		}

		if (!string.IsNullOrEmpty(filter.SortName))
		{
			Func<FootballClub, object> orderBy = filter.SortName switch
			{
				nameof(FootballClub.Name) => c => c.Name,
				nameof(FootballClub.TotalGames) => c => c.TotalGames,
				nameof(FootballClub.Wins) => c => c.Wins,
				nameof(FootballClub.Draws) => c => c.Draws,
				nameof(FootballClub.Losses) => c => c.Losses,
				nameof(FootballClub.GoalsFor) => c => c.GoalsFor,
				nameof(FootballClub.GoalsAgainst) => c => c.GoalsAgainst,
				nameof(FootballClub.Points) => c => c.Points,
				_ => c => c.Name
			};

			clubs = filter.SortDescending ?? false
				? clubs.OrderByDescending(orderBy)
				: clubs.OrderBy(orderBy);
		}

		result.Items = clubs.ToList();
		return new(result: result);
	}
}