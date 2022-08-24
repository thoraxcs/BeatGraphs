using AngleSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeatGraphs.Modules
{
    /// <summary>
    /// The Updater is used to scrape the various Sports-Reference websites for game scores
    /// </summary>
    public static class Updater
    {
        private static readonly int MIDYEARCUTOFF = 225; // 225 is an arbitary number chosen to represent the a split during the offseason

        public static BeatGraphForm form; // This is used to allow logging to the output window

        /// <summary>
        /// Entry point to the updater
        /// </summary>
        /// <param name="leagues">The set of leagues to be included in the update.</param>
        /// <param name="season">The season to be included in the update, multiple seasons are handled at the calling level.</param>
        /// <param name="bgForm">The form, included to allow logging.</param>
        public static void Run(List<string> leagues, string season, BeatGraphForm bgForm)
        {
            form = bgForm;

            // Trigger the appropriate league parsers
            if (leagues.Contains("MLB"))
                loadMLB("MLB", season);
            if (leagues.Contains("NBA"))
                loadNBA("NBA", season);
            if (leagues.Contains("NFL"))
                loadNFL("NFL", season);
            if (leagues.Contains("NHL"))
                loadNHL("NHL", season);
        }

        /// <summary>
        /// Load the scores for Major League Baseball in the gives season
        /// </summary>
        private static void loadMLB(string league, string season)
        {
            try
            {
                form.Log($"Updating scores for the {season} {league} season.");

                string sHTML = "", sHTMLP = "";
                int seasonStart = -1;

                // Get the ID for the season and clear all existing scores for it from the database
                var seasonID = Helpers.FindSeason(int.Parse(season), league);
                ClearSeason(seasonID);

                // Get the raw HTML for the scrape
                try
                {
                    sHTML = GetHtml("https://www.baseball-reference.com/leagues/majors/" + season + "-schedule.shtml");
                }
                catch { return; }

                // Sometimes the score section is delineated by "MLB Schedule", sometimes by "Major League Schedule", truncate HTML to either
                if (sHTML.Contains(@"<h2>MLB Schedule</h2>"))
                    sHTML = sHTML.Substring(sHTML.IndexOf("<h2>MLB Schedule</h2>"));
                if (sHTML.Contains(@"<h2>Major League Schedule</h2>"))
                    sHTML = sHTML.Substring(sHTML.IndexOf("<h2>Major League Schedule</h2>"));

                // If "Today's Games" exists in the text, games beyond cannot be counted. Truncate past this point.
                if (sHTML.Contains(@"<span id='today'>Today's Games</span>"))
                    sHTML = sHTML.Substring(0, sHTML.IndexOf(@"<span id='today'>Today's Games</span>"));

                // If postseason games exist
                if (sHTML.Contains(@"<h2>Postseason Schedule</h2>"))
                {
                    // Copy the text to a playoff set and truncate the playoff section from the main section
                    sHTMLP = sHTML.Substring(sHTML.IndexOf(@"<h2>Postseason Schedule</h2>"));
                    sHTML = sHTML.Substring(0, sHTML.IndexOf(@"<h2>Postseason Schedule</h2>"));

                    // The end of the page has three possible markings to end the playoff section. Truncate to any that exist.
                    if (sHTMLP.Contains($@"<h2>More {season} Major League Pages</h2>"))
                        sHTMLP = sHTMLP.Substring(0, sHTMLP.IndexOf($@"<h2>More {season} Major League Pages</h2>"));
                    if (sHTMLP.Contains($@"<h2>More {season} Major League Baseball Pages</h2>"))
                        sHTMLP = sHTMLP.Substring(0, sHTMLP.IndexOf($@"<h2>More {season} Major League Baseball Pages</h2>"));
                    if (sHTMLP.Contains(@"<span id='today'>Today's Games</span>"))
                        sHTMLP = sHTMLP.Substring(0, sHTMLP.IndexOf(@"<span id='today'>Today's Games</span>"));
                }

                // Entries are delinated by a <p> tag, separate them out
                var games = ParseDoc(sHTML, "p").Result;

                // Create some regular expressions to help parse the information needed.
                var teamReg = new Regex(@"\/teams\/([A-Z0-9]{3})\/\d{4}.shtml");
                var rangeReg = new Regex(@"\/boxes\/[A-Z0-9]{3}\/[A-Z0-9]{3}(\d{8})\d.shtml");
                var scoreReg = new Regex(@"\((\d*)\)");

                string awayTeam = "", homeTeam = "", gameDate = "";
                int awayScore, homeScore, awayID, homeID;

                foreach (var game in games)
                {
                    try
                    {
                        var attributes = game.QuerySelectorAll("a"); // Retrieves instances of <a> tags for each record

                        // This line contains no game information if the text has "Standings" in it or there are no matches to the score regex
                        if (game.InnerHtml.Contains("Standings"))
                            continue;
                        if (scoreReg.Matches(game.InnerHtml).Count == 0)
                            continue;

                        // Date comes across as /boxes/XXX/XXX000000000.shtml where the Xs represent the team, next 8 digits the date
                        // The last digit is an index for double-headers. Parse the 8 digit date and add dashes so it's 0000-00-00 and parsable.
                        gameDate = rangeReg.Match(attributes[2].OuterHtml).Groups[1].Value;
                        gameDate = gameDate.Insert(6, "-").Insert(4, "-");

                        // Parse team and score data
                        awayTeam = teamReg.Match(attributes[0].OuterHtml).Groups[1].Value;
                        homeTeam = teamReg.Match(attributes[1].OuterHtml).Groups[1].Value;
                        awayScore = int.Parse(scoreReg.Matches(game.InnerHtml)[0].Groups[1].Value);
                        homeScore = int.Parse(scoreReg.Matches(game.InnerHtml)[1].Groups[1].Value);
                        awayID = Helpers.GetTeamID(awayTeam, league, season);
                        homeID = Helpers.GetTeamID(homeTeam, league, season);

                        // There's a problem if the teams couldn't be identified
                        if (awayID == -1 || homeID == -1)
                        {
                            form.Log($"Could not identify teams for game on {gameDate} for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                            return;
                        }

                        // Set the information needed to determine the "week" of the season
                        DateTime dtGame = DateTime.Parse(gameDate);
                        int dayOfYear = dtGame.DayOfYear;
                        if (seasonStart == -1)
                            seasonStart = dayOfYear;

                        InsertGame(seasonID, ((dayOfYear - seasonStart) / 7) + 1, awayID, awayScore, homeID, homeScore);
                    }
                    catch
                    {
                        form.Log($"Failure in MLB regular season. Last success: {gameDate}, {awayTeam} - {homeTeam}");
                        throw;
                    }
                }

                // Playoffs
                var playoffs = ParseDoc(sHTMLP, "p").Result;
                int round, wcGames, year = int.Parse(season);
                var lastWin = -1;

                if (year == 1981) // 1981 Season had an extra round
                    round = 502;
                else if (year <= 1993) // 1970-1993 only had A/NLCS and World Series
                    round = 503;
                else if (year <= 2012) // Divisional round introduced in 1995 (No playoffs in 1994 due to strike)
                    round = 502;
                else // Wild Card round introduced in 2012
                    round = 501;

                if (year <= 2011) // No wild card round through 2011
                    wcGames = 0;
                else if (year <= 2019) // 2 wild card games from '12 to '19
                    wcGames = 2;
                else if (year == 2020) // 8 wild card games in 2020 (Covid year)
                    wcGames = 8;
                else if (year == 2021) // one more year of 2 wild cards
                    wcGames = 2;
                else // 4 wild card matches as of 2022
                    wcGames = 4;

                // Initialize playoff matchup tracker
                var matchups = new Matchups(round, wcGames);

                foreach (var game in playoffs)
                {
                    try
                    {
                        var attributes = game.QuerySelectorAll("a"); // Retrieves instances of <a> tags for each record

                        // This line contains no game information if the text has "Standings" in it or there are no matches to the score regex
                        if (game.InnerHtml.Contains("Standings"))
                            continue;
                        if (scoreReg.Matches(game.InnerHtml).Count == 0)
                            continue;

                        // Parse team and score data
                        awayTeam = teamReg.Match(attributes[0].OuterHtml).Groups[1].Value;
                        homeTeam = teamReg.Match(attributes[1].OuterHtml).Groups[1].Value;
                        awayScore = int.Parse(scoreReg.Matches(game.InnerHtml)[0].Groups[1].Value);
                        homeScore = int.Parse(scoreReg.Matches(game.InnerHtml)[1].Groups[1].Value);
                        awayID = Helpers.GetTeamID(awayTeam, league, season);
                        homeID = Helpers.GetTeamID(homeTeam, league, season);

                        // There's a problem if the teams couldn't be identified
                        if (awayID == -1 || homeID == -1)
                        {
                            form.Log($"Could not identify teams for playoff game for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                            return;
                        }

                        // Add the matchup to the playoff tracker if necessary and get the resulting playoff round
                        matchups.Add(awayID, homeID);
                        round = matchups.GetRound(awayID, homeID);

                        InsertGame(seasonID, round, awayID, awayScore, homeID, homeScore);
                        lastWin = awayScore > homeScore ? awayID : homeID; // Track which franchise has the last win of the season, CHAMPIONS!
                    }
                    catch
                    {
                        form.Log($"Failure in MLB post season. Last success: {round}, {awayTeam} - {homeTeam}");
                        throw;
                    }
                }

                // Send to the database a recap of the playoff matchups.
                RecordPlayoffs(seasonID, matchups, lastWin);
                form.Log($"{league} {season} has been updated successfully.");
            }
            catch (Exception ex)
            {
                form.Log(ex.Message, LogLevel.error);
            }
        }

        /// <summary>
        /// Load the scores for National Basketball Association in the gives season
        /// </summary>
        private static void loadNBA(string league, string season)
        {
            try
            {
                form.Log($"Updating scores for the {season} {league} season.");

                string sHTML = "", sHTMLP = "";
                int seasonStart = -1;

                // Get the ID for the season and clear all existing scores for it from the database
                var seasonID = Helpers.FindSeason(int.Parse(season), league);
                ClearSeason(seasonID);

                #region Playoff Info
                // For some stupid reason Basketball-Reference doesn't delineate playoffs so we have to find out on our own.
                // TODO: Is the lack of a playoffs page going to be a problem during the regular season?
                try
                {
                    sHTMLP = GetHtml("http://www.basketball-reference.com/playoffs/NBA_" + (int.Parse(season) + 1) + ".html", false);
                }
                catch { return; }

                // Initialize playoff matchup tracker
                var round = 504;
                var playoffs = new Matchups(round);
                var playoffStart = DateTime.Now;
                if (sHTMLP != "")
                {
                    var pseasons = ParseDoc(sHTMLP, "table").Result; // Retrieves instances of <table> tags for each record
                    var serieses = pseasons[0].QuerySelectorAll("table"); // Each series is another <table> within the first <table> record above

                    // For the NBA we're building the playoffs from the championship down
                    var matchesInRound = 1; // So the first round, the championship, has one match in it
                    var matchesLeftInRound = matchesInRound; // And it starts with one match left

                    foreach (var series in serieses)
                    {
                        // Iterate through the series, and each game within is marked by <tr>
                        var games = series.QuerySelectorAll("tr");
                        foreach (var game in games)
                        {
                            var attributes = game.QuerySelectorAll("td"); // Game information delineated by <td>

                            // Parse out the date for the game and if it's sooner than a previous playoff date we have, track it
                            // as the date the playoffs start
                            var gameDate = $"{attributes[1].InnerHtml.Substring(attributes[1].InnerHtml.IndexOf(",") + 2)}, {int.Parse(season) + 1}";
                            DateTime dtGame = DateTime.Parse(gameDate);
                            if (playoffStart > dtGame)
                                playoffStart = dtGame;

                            if (ParseLine(attributes[3].InnerHtml) == "")
                                continue; // This game hasn't been played

                            // Parse team and score data
                            var awayTeam = attributes[2].InnerHtml;
                            var homeTeam = attributes[4].InnerHtml.Substring(2); // Removes @ from home team line
                            var awayScore = int.Parse(attributes[3].InnerHtml);
                            var homeScore = int.Parse(attributes[5].InnerHtml);
                            var awayID = Helpers.GetTeamID(awayTeam, league, season);
                            var homeID = Helpers.GetTeamID(homeTeam, league, season);

                            // There's a problem if the teams couldn't be identified
                            if (awayID == -1 || homeID == -1)
                            {
                                form.Log($"Could not identify teams for playoff game for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                                return;
                            }

                            // Only add the match to the playoff tracker. Recording the game score will happen later.
                            playoffs.Add(awayID, homeID, round);
                        }

                        if (--matchesLeftInRound == 0)
                        {
                            // If we've run out of matches in this round, double the possible matches for the next round and reset.
                            // 1 match in final, 2 matches in conference final, 4 in divisional round, 8 possible in wild card
                            matchesLeftInRound = matchesInRound *= 2;
                            round--; // Change round number
                        }
                    }
                }
                #endregion

                // Get the base page for the season which will be parsed for months where games where played
                try
                {
                    sHTML = GetHtml("http://www.basketball-reference.com/leagues/NBA_" + (int.Parse(season) + 1) + "_games.html");
                }
                catch { return; }

                // Find the pages for the schedule. For some stupid reason Basketball-Reference splits the season into month pages
                var monthReg = new Regex(@"NBA_\d{4}_games-([a-z]*).html"">\1", RegexOptions.IgnoreCase);
                var matches = monthReg.Matches(sHTML);
                var lastWin = -1;
                foreach (Match match in matches)
                {
                    var month = match.Groups[1].Value;
                    var url = $@"https://www.basketball-reference.com/leagues/NBA_{(int.Parse(season) + 1)}_games-{month}.html";

                    // Get the raw HTML for the scrape
                    try
                    {
                        sHTML = GetHtml(url);
                    }
                    catch { return; }

                    var seasons = ParseDoc(sHTML, "table").Result; // All games held in the first <table>

                    // Regular Season
                    var games = seasons[0].QuerySelectorAll("tr"); // Games are separated by a <tr>
                    foreach (var game in games)
                    {
                        var attributes = game.QuerySelectorAll("td");
                        if (attributes.Count() == 0) //Header row will have no data cells
                            continue;

                        var gameDate = game.QuerySelector("th").QuerySelector("a").InnerHtml;

                        // For some stupid reason Basketball-Reference sometimes has a start time column, sometimes doesn't.
                        var timeColAdjustment = attributes.Length == 9 ? 1 : 0;

                        if (ParseLine(attributes[2 - timeColAdjustment].InnerHtml) == "")
                            continue; // This game hasn't been played

                        // Parse team and score data
                        var awayTeam = ParseLine(attributes[1 - timeColAdjustment].InnerHtml);
                        var homeTeam = ParseLine(attributes[3 - timeColAdjustment].InnerHtml);
                        var awayScore = int.Parse(attributes[2 - timeColAdjustment].InnerHtml);
                        var homeScore = int.Parse(attributes[4 - timeColAdjustment].InnerHtml);
                        var awayID = Helpers.GetTeamID(awayTeam, league, season);
                        var homeID = Helpers.GetTeamID(homeTeam, league, season);

                        // There's a problem if the teams couldn't be identified
                        if (awayID == -1 || homeID == -1)
                        {
                            form.Log($"Could not identify teams for game on {gameDate} for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                            return;
                        }

                        // Set the information needed to determine the "week" of the season
                        DateTime dtGame = DateTime.Parse(gameDate);
                        int dayOfYear = dtGame.DayOfYear;
                        if (dayOfYear < MIDYEARCUTOFF)
                        {
                            if (DateTime.IsLeapYear(dtGame.Year - 1))
                                dayOfYear += 366;
                            else
                                dayOfYear += 365;
                        }

                        if (seasonStart == -1)
                            seasonStart = dayOfYear;

                        dayOfYear = dayOfYear - seasonStart;
                        var weekOfYear = (dayOfYear / 7) + 1;

                        // If the date is past the playoff start date, it's a playoff game and use the round as the week
                        if (dtGame >= playoffStart)
                            weekOfYear = playoffs.GetRound(awayID, homeID);

                        InsertGame(seasonID, weekOfYear, awayID, awayScore, homeID, homeScore);
                        lastWin = awayScore > homeScore ? awayID : homeID; // Track which franchise has the last win of the season, CHAMPIONS!
                    }
                }

                // Send to the database a recap of the playoff matchups.
                RecordPlayoffs(seasonID, playoffs, lastWin);
                form.Log($"{league} {season} has been updated successfully.");
            }
            catch (Exception ex)
            {
                form.Log(ex.Message, LogLevel.error);
            }
        }

        /// <summary>
        /// Load the scores for National Football League in the gives season
        /// </summary>
        private static void loadNFL(string league, string season)
        {
            try
            {
                form.Log($"Updating scores for the {season} {league} season.");

                string sHTML = "";

                // Get the ID for the season and clear all existing scores for it from the database
                var seasonID = Helpers.FindSeason(int.Parse(season), league);
                ClearSeason(seasonID);

                // Get the raw HTML for the scrape
                try
                {
                    sHTML = GetHtml("http://www.pro-football-reference.com/years/" + season + "/games.htm");
                }
                catch { return; }

                var seasons = ParseDoc(sHTML, "table").Result; // Scores are found in a <table> tag

                // Full Season
                var matchups = new Matchups(501); // Passed in values don't matter here, we're going to set each entry manually for NFL
                var lastWin = -1;

                var games = seasons[0].QuerySelectorAll("tr"); // Games are indicated by a <tr>
                foreach (var game in games)
                {
                    var attributes = game.QuerySelectorAll("td");
                    if (attributes.Count() == 0) // Header row will have no data cells
                        continue;

                    // NFL gives us week numbers directly
                    var week = game.QuerySelector("th").InnerHtml;

                    // However they label non-regular season games with text, translate appropriately
                    if (week.Contains("Pre"))
                        continue; // Skip pre-season games.
                    else if (week == "WildCard")
                        week = "501";
                    else if (week == "Division")
                        week = "502";
                    else if (week == "ConfChamp")
                        week = "503";
                    else if (week == "SuperBowl")
                        week = "504";

                    if (attributes.Length == 8) // NFL has two possible data configurations, one has 8 attributes
                    {
                        // If there's no score, the game hasn't been played yet. Move to next record.
                        if (ParseLine(attributes[3].InnerHtml) == "")
                            continue;

                        // Parse team and score data
                        var awayTeam = ParseLine(attributes[2].InnerHtml);
                        var homeTeam = ParseLine(attributes[5].InnerHtml);
                        var awayScore = int.Parse(attributes[3].InnerHtml);
                        var homeScore = int.Parse(attributes[6].InnerHtml);
                        var awayID = Helpers.GetTeamID(awayTeam, league, season);
                        var homeID = Helpers.GetTeamID(homeTeam, league, season);

                        // There's a problem if the teams couldn't be identified
                        if (awayID == -1 || homeID == -1)
                        {
                            form.Log($"Could not identify teams in week {week} for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                            return;
                        }

                        // Record the game and add it to the playoff tracker
                        var round = int.Parse(week);
                        InsertGame(seasonID, round, awayID, awayScore, homeID, homeScore);
                        if (round >= 500)
                            matchups.Add(awayID, homeID, round);
                        lastWin = awayScore > homeScore ? awayID : homeID; // Track which franchise has the last win of the season, CHAMPIONS!
                    }
                    else // Using the other data configuration
                    {
                        // If there's no score, the game hasn't been played yet. Move to next record.
                        if (ParseLine(attributes[7].InnerHtml) == "")
                            continue;

                        // Parse team and score data
                        var winTeam = ParseLine(attributes[3].InnerHtml); //Winning Team
                        var loseTeam = ParseLine(attributes[5].InnerHtml); //Losing Team
                        var winScore = int.Parse(ParseLine(attributes[7].InnerHtml)); //Winning Score
                        var loseScore = int.Parse(ParseLine(attributes[8].InnerHtml)); //Losing Score         
                        var winID = Helpers.GetTeamID(winTeam, league, season);
                        var loseID = Helpers.GetTeamID(loseTeam, league, season);

                        // There's a problem if the teams couldn't be identified
                        if (winID == -1 || loseID == -1)
                        {
                            form.Log($"Could not identify teams for week {week} game between ({winTeam}) and ({loseTeam}).", LogLevel.error);
                            return;
                        }

                        // NFL sometimes orders the data by winner instead of home/away
                        var homeMark = ParseLine(attributes[4].InnerHtml);
                        var round = int.Parse(week);
                        if (homeMark == "@")
                        {   // Visitor wins
                            InsertGame(seasonID, int.Parse(week), winID, winScore, loseID, loseScore);
                            if (round >= 500)
                                matchups.Add(winID, loseID, round);
                        }
                        else
                        {   // Home team wins
                            InsertGame(seasonID, int.Parse(week), loseID, loseScore, winID, winScore);
                            if (round >= 500)
                                matchups.Add(loseID, winID, round);
                        }
                        lastWin = winID; // Track which franchise has the last win of the season, CHAMPIONS!
                    }
                }

                // Send to the database a recap of the playoff matchups.
                RecordPlayoffs(seasonID, matchups, lastWin);
                form.Log($"{league} {season} has been updated successfully.");
            }
            catch (Exception ex)
            {
                form.Log(ex.Message, LogLevel.error);
            }
        }

        /// <summary>
        /// Load the scores for National Hockey League in the gives season
        /// </summary>
        private static void loadNHL(string league, string season)
        {
            try
            {
                form.Log($"Updating scores for the {season} {league} season.");

                string sHTML = "";
                int seasonStart = -1;

                if (season == "2004")
                {
                    form.Log("The 2004-05 NHL season was cancelled.", LogLevel.warning);
                    return;
                }

                // Get the ID for the season and clear all existing scores for it from the database
                var seasonID = Helpers.FindSeason(int.Parse(season), league);
                ClearSeason(seasonID);

                // Get the raw HTML for the scrape
                try
                {
                    sHTML = GetHtml("http://www.hockey-reference.com/leagues/NHL_" + (int.Parse(season) + 1) + "_games.html");
                }
                catch { return; }

                var seasons = ParseDoc(sHTML, "table").Result; // Scores are held in the first <table>

                // Regular Season
                var games = seasons[0].QuerySelectorAll("tr"); // Games are indicated by a <tr>
                foreach (var game in games)
                {
                    var attributes = game.QuerySelectorAll("td");
                    if (attributes.Count() == 0) //Header row will have no data cells
                        continue;
                    if (ParseLine(attributes[1].InnerHtml) == "")
                        continue; // This game hasn't been played

                    var gameDate = ParseLine(game.QuerySelector("th").InnerHtml);

                    // Parse team and score data
                    var awayTeam = ParseLine(attributes[0].InnerHtml);
                    var homeTeam = ParseLine(attributes[2].InnerHtml);
                    var awayScore = int.Parse(attributes[1].InnerHtml);
                    var homeScore = int.Parse(attributes[3].InnerHtml);
                    var awayID = Helpers.GetTeamID(awayTeam, league, season);
                    var homeID = Helpers.GetTeamID(homeTeam, league, season);

                    // There's a problem if the teams couldn't be identified
                    if (awayID == -1 || homeID == -1)
                    {
                        form.Log($"Could not identify teams for game on {gameDate} for ({awayTeam}) @ ({homeTeam}).", LogLevel.error);
                        return;
                    }

                    // Set the information needed to determine the "week" of the season
                    DateTime dtGame = DateTime.Parse(gameDate);
                    int dayOfYear = dtGame.DayOfYear;
                    if (dayOfYear < MIDYEARCUTOFF)
                    {
                        if (DateTime.IsLeapYear(dtGame.Year - 1))
                            dayOfYear += 366;
                        else
                            dayOfYear += 365;
                    }
                    if (seasonStart == -1)
                        seasonStart = dayOfYear;

                    InsertGame(seasonID, ((dayOfYear - seasonStart) / 7) + 1, awayID, awayScore, homeID, homeScore);
                }

                // Playoffs
                var playoffs = seasons[1].QuerySelectorAll("tr"); // Playoffs are contained in the second <table> games still by <tr>
                int round, wcGames, year = int.Parse(season);

                if (year == 2019) // Covid year had an extra round of playoffs.
                    round = 500;
                else if (year <= 1973) // From 1970/1-1973/4 there were only 3 playoff rounds
                    round = 502;
                else // 1974/75 season introduced fourth round of playoffs
                    round = 501;

                if (year <= 1973) // No wild card round through 1973
                    wcGames = 0;
                else if (year <= 1979) // 4 wild card games from 74/75 to 78/79
                    wcGames = 4;
                else if (year == 2019) // 8 "zeroth round" games in 2019 (Covid year)
                    wcGames = 8;
                else // full wild card round can be treated normally
                    wcGames = 0;

                // Initialize playoff objects
                var matchups = new Matchups(round, wcGames);
                var covidmatchups = new Matchups(round, wcGames);

                #region Covid-19 exception
                // The 2019 Covid year also had a seeding playoff for the higher seeds. These games will all be considered
                // on the same round level as the "zeroth round" series for the lower seeds. However, these matchups could 
                // repeat in the knockout phase and must be handled specially.
                if (year == 2019)
                {
                    covidmatchups.Add(Helpers.GetTeamID("Boston Bruins", league, season), Helpers.GetTeamID("Tampa Bay Lightning", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Boston Bruins", league, season), Helpers.GetTeamID("Washington Capitals", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Boston Bruins", league, season), Helpers.GetTeamID("Philadelphia Flyers", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Tampa Bay Lightning", league, season), Helpers.GetTeamID("Washington Capitals", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Tampa Bay Lightning", league, season), Helpers.GetTeamID("Philadelphia Flyers", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Washington Capitals", league, season), Helpers.GetTeamID("Philadelphia Flyers", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("St. Louis Blues", league, season), Helpers.GetTeamID("Colorado Avalanche", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("St. Louis Blues", league, season), Helpers.GetTeamID("Vegas Golden Knights", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("St. Louis Blues", league, season), Helpers.GetTeamID("Dallas Stars", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Colorado Avalanche", league, season), Helpers.GetTeamID("Vegas Golden Knights", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Colorado Avalanche", league, season), Helpers.GetTeamID("Dallas Stars", league, season), round);
                    covidmatchups.Add(Helpers.GetTeamID("Vegas Golden Knights", league, season), Helpers.GetTeamID("Dallas Stars", league, season), round);
                }
                #endregion

                var lastWin = -1;
                foreach (var game in playoffs)
                {
                    var attributes = game.QuerySelectorAll("td");
                    if (attributes.Count() == 0) //Header row will have no data cells
                        continue;
                    if (ParseLine(attributes[1].InnerHtml) == "")
                        continue; // This game hasn't been played

                    // Parse team and score data
                    var awayTeam = attributes[0].QuerySelector("a").InnerHtml;
                    var homeTeam = attributes[2].QuerySelector("a").InnerHtml;
                    var awayScore = int.Parse(attributes[1].InnerHtml);
                    var homeScore = int.Parse(attributes[3].InnerHtml);
                    var awayID = Helpers.GetTeamID(awayTeam, league, season);
                    var homeID = Helpers.GetTeamID(homeTeam, league, season);

                    // There's a problem if the teams couldn't be identified
                    if (awayID == -1 || homeID == -1)
                    {
                        form.Log($"Could not identify teams in playoff game for ({awayTeam}) vs ({homeTeam}).", LogLevel.error);
                        return;
                    }

                    if (covidmatchups.Contains(awayID, homeID))
                    {
                        // Round Robin matchups are automatically "zeroth round". Do not add to matchup table. Remove from exception group
                        // so that if it comes up again it will be treated as a new matchup.
                        round = 500;
                        covidmatchups.Remove(awayID, homeID);
                    }
                    else
                    {
                        // Add the matchup to the playoff tracker if necessary and get the resulting playoff round
                        matchups.Add(awayID, homeID);
                        round = matchups.GetRound(awayID, homeID);
                    }

                    InsertGame(seasonID, round, awayID, awayScore, homeID, homeScore);
                    lastWin = awayScore > homeScore ? awayID : homeID; // Track which franchise has the last win of the season, CHAMPIONS!
                }

                // Send to the database a recap of the playoff matchups.
                RecordPlayoffs(seasonID, matchups, lastWin);
                form.Log($"{league} {season} has been updated successfully.");
            }
            catch (Exception ex)
            {
                form.Log(ex.Message, LogLevel.error);
            }
        }

        /// <summary>
        /// Takes in HTML and finds all tags marked with the parser and returns a list of items starting with that tag.
        /// </summary>
        private static async Task<AngleSharp.Dom.IHtmlCollection<AngleSharp.Dom.IElement>> ParseDoc(string sHTML, string parser)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var doc = await context.OpenAsync(req => req.Content(sHTML));
            return doc.QuerySelectorAll(parser);
        }

        /// <summary>
        /// Starts with the championship and calls to a recursive function to send playoff matches to the database
        /// </summary>
        private static void RecordPlayoffs(int seasonID, Matchups matchups, int champID)
        {
            // If there are no playoff matches to record, escape.
            if (!matchups.HasMatches())
                return;

            // Call to recursive playoff recording function. Staring from the championship (504), work down.
            RecordPlayoffsHelper(seasonID, matchups, champID, 504);
        }

        /// <summary>
        /// Recursive function that sends the current matchup to the playoff database, then tries to do the next round with participants
        /// </summary>
        private static void RecordPlayoffsHelper(int seasonID, Matchups matchups, int winnerID, int round)
        {
            // Get the playoff match for the indicated team in the indicate round, recursive escape condition is when match doesn't exist
            var matchup = matchups.GetMatch(round, winnerID);
            if (!matchup.HasValue)
                return;

            // TODO: move this to a helper function. DB calls should only happen there.
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[5];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);
            sqlParam[1] = SQLDBA.CreateParameter("@Range", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Value.Value);
            sqlParam[2] = SQLDBA.CreateParameter("@WinnerID", SqlDbType.Int, 64, ParameterDirection.Input, winnerID);
            sqlParam[3] = SQLDBA.CreateParameter("@AwayID", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Value.Key.Item1);
            sqlParam[4] = SQLDBA.CreateParameter("@HomeID", SqlDbType.Int, 64, ParameterDirection.Input, matchup.Value.Key.Item2);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Insert_Playoff", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();

            // Remove the current matchup from the set and try the next lower round for each team involved.
            matchups.Remove(matchup.Value.Key.Item1, matchup.Value.Key.Item2);
            RecordPlayoffsHelper(seasonID, matchups, matchup.Value.Key.Item1, matchup.Value.Value - 1);
            RecordPlayoffsHelper(seasonID, matchups, matchup.Value.Key.Item2, matchup.Value.Value - 1);
        }

        /// <summary>
        /// Clear all games for the season.
        /// </summary>
        // TODO: Move this function to the helper class.
        private static void ClearSeason(int seasonID)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[1];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Clear_Season", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Read the HTML from a given URL. ErrorThrow allows the designation of how serious failure to find the page is.
        /// </summary>
        // TODO: Consider moving this to helper class as well since it's external things like FTP and DB.
        private static string GetHtml(string url, bool errorThrow = true)
        {
            string html = "";

            try
            {
                HttpWebRequest hwRequest = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse hwResponse = (HttpWebResponse)hwRequest.GetResponse())
                {
                    using (StreamReader srReader = new StreamReader(hwResponse.GetResponseStream()))
                    {
                        html = srReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                if (errorThrow)
                {
                    form.Log($"Requested page could not be accessed and may not yet exist: {url}", LogLevel.error);
                    throw;
                }
                else
                {
                    form.Log($"Requested page could not be accessed and may not yet exist: {url}", LogLevel.warning);
                }
            }
            return html;
        }

        /// <summary>
        /// Insert game data into the database
        /// </summary>
        // TODO: Move this function to the helper class.
        private static void InsertGame(int seasonID, int week, int awayID, int awayScore, int homeID, int homeScore)
        {
            SQLDatabaseAccess SQLDBA = new SQLDatabaseAccess();
            SqlParameter[] sqlParam = new SqlParameter[6];

            sqlParam[0] = SQLDBA.CreateParameter("@SeasonID", SqlDbType.Int, 64, ParameterDirection.Input, seasonID);
            sqlParam[1] = SQLDBA.CreateParameter("@WeekID", SqlDbType.Int, 64, ParameterDirection.Input, week);
            sqlParam[2] = SQLDBA.CreateParameter("@AwayID", SqlDbType.Int, 64, ParameterDirection.Input, awayID);
            sqlParam[3] = SQLDBA.CreateParameter("@AwayScore", SqlDbType.Int, 64, ParameterDirection.Input, awayScore);
            sqlParam[4] = SQLDBA.CreateParameter("@HomeID", SqlDbType.Int, 64, ParameterDirection.Input, homeID);
            sqlParam[5] = SQLDBA.CreateParameter("@HomeScore", SqlDbType.Int, 64, ParameterDirection.Input, homeScore);

            SQLDBA.Open();
            SQLDBA.ExecuteSqlSP("Insert_Game", sqlParam);

            SQLDBA.Close();
            SQLDBA.Dispose();
        }

        /// <summary>
        /// Removes ancillary text from a data entry to get to the wanted information.
        /// </summary>
        private static string ParseLine(object line)
        {
            string text = (string)line;
            if (text == "")
                return text;

            // Remove all tags
            // TODO: Replace with regex? ">([^<>]+?)<" or even better ">((?!\s)[^<>]+?)<"
            if (text.Contains("<a"))
                text = text.Substring(text.IndexOf("<a"));
            if (text.Contains("<strong>"))
                text = text.Substring(text.IndexOf("<strong>") + 7);
            if (text.Contains("<"))
            {
                text = text.Substring(text.IndexOf(">") + 1);
                text = text.Substring(0, text.IndexOf("<"));
            }
            return text;
        }
    }
}
