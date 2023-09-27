using footballManager;

class Program
    {
        static void Main(string[] args)
        {
            
            var currentDirectory = Directory.GetCurrentDirectory();
            var setupCsvPath = Path.Combine(currentDirectory, "csv", "setup.csv");
            var teamsCsvPath = Path.Combine(currentDirectory, "csv", "teams.csv");
                
                if (!File.Exists(setupCsvPath))
                {
                    Console.WriteLine("Setup CSV file does not exist.");
                    return;
                }

                if (!File.Exists(teamsCsvPath))
                {
                    Console.WriteLine("Teams CSV file does not exist.");
                    return;
                }

        LoadSetupData(setupCsvPath);
        var teamsData = LoadTeamsData(teamsCsvPath);

            var initialTeamsData = new List<FootballTeam>(teamsData);
            Program program = new();
            program.CreateRound0File(initialTeamsData);
            int numberOfRoundsToSimulate = 22; // first 22 rounds

            for (int round = 1; round <= numberOfRoundsToSimulate; round++)
            {
                // Simulate and save the results for this round
                program.SimulateRound(round, teamsData);
                program.CreateRoundFile(teamsData, round);
            }

            int upperRounds = 32; //the last 10 with upper and lower devision playing against eachother
            for (int round = 23; round <= upperRounds; round++)
            {
                program.FinalsSimulation(round, teamsData);
                program.CreateRoundFile(teamsData, round);
            }

            //header for the terminal print
            List<string> teamHeaders = new List<string>
            {
                "Position",
                "Clubname",
                "Short",
                "Played",
                "Ranking",
                "Wins",
                "Draws",
                "Losses",
                "Goals for",
                "Goals against",
                "Goal diffrence",
                "Points",
                "Streak"
           
            };
           
        string round32CsvPath = Path.Combine(currentDirectory, "results", "round32.csv");

        if (File.Exists(round32CsvPath))
        {
            Console.WriteLine("\nResults of the superliga:");
            using (var reader = new StreamReader(round32CsvPath))
            {
                string line;
                bool isHeader = true;
                Console.WriteLine($"{teamHeaders[0],-8} | {teamHeaders[1], -15} | {teamHeaders[2], -5} | {teamHeaders[3],-6} | {teamHeaders[4],-10} | {teamHeaders[5],-5} | {teamHeaders[6],-5} | {teamHeaders[7],-7} | {teamHeaders[8],-9} | {teamHeaders[9],-14} | {teamHeaders[10],-15} | {teamHeaders[11],-7} | {teamHeaders[12],-7}");

                while ((line = reader.ReadLine()) != null)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                    }
                    else
                    {
                        var values = line.Split(',');
                        if (values.Length == 13)
                        {
                            Console.WriteLine($"{values[0],-8} | {values[1],-15} | {values[2],-5} | {values[3],-6} | {values[4],-10} | {values[5],-5} | {values[6],-5} | {values[7],-7} | {values[8],-9} | {values[9],-14} | {values[10],-15} | {values[11],-7} | {values[12],-7}");
                        }
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Round 32 results file not found.");
        }

            //garbage collection
            GC.Collect();
        }

        void CreateRound0File(List<FootballTeam> teams)
        {
            string folderPath = "results";
            string fileName = "round0.csv";
            string filePath = Path.Combine(folderPath, fileName);

            // Write the header to the file
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("Position,FullClubName,ShortClubName,GamesPlayed,SpecialRanking,Wins,Draws,Losses,GoalsFor,GoalsAgainst,GoalDifference,Points,CurrentStreak");
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var team in teams)
                {
                    string data = $"{team.Position},{team.FullClubName},{team.ShortClubName},0,,0,0,0,0,0,0,0,";
                    writer.WriteLine(data);
                }
            }
        }

        void CreateRoundFile(List<FootballTeam> teams, int roundIndex)
        {
            string folderPath = "results";
            string fileName = $"round{roundIndex}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            // Sort teams based on points and goal difference to assign positions
            var sortedTeams = teams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.GoalsAgainst)
                .ThenBy(t => t.FullClubName)
                .ToList();

            // Use StreamWriter to write the data
            using (StreamWriter writer = new StreamWriter(filePath, false)) // Use FileMode.Create to overwrite existing files
            {
                // Write the header to the file
                writer.WriteLine("Position,FullClubName,ShortClubName,GamesPlayed,SpecialRanking,Wins,Draws,Losses,GoalsFor,GoalsAgainst,GoalDifference,Points,CurrentStreak");

                int position = 1;

                foreach (var team in sortedTeams)
                {
                    // Generate data for each team in this round and add it to the file
                    string data = $"{position},{team.FullClubName},{team.ShortClubName},{team.GamesPlayed},{team.SpecialRanking},{team.Wins},{team.Draws},{team.Losses},{team.GoalsFor},{team.GoalsAgainst},{team.GoalDifference},{team.Points},{team.CurrentStreak}";
                    writer.WriteLine(data);

                    position++;
                }
            }
        }
 
        void SimulateRound(int roundIndex, List<FootballTeam> teamsData)
        {
            Random random = new Random();

            // Shuffle the teams to randomize the match order
            List<FootballTeam> shuffledTeams = new List<FootballTeam>(teamsData);
            Shuffle(shuffledTeams, random);
            


            for (int i = 0; i < shuffledTeams.Count; i += 2)
            {
                var homeTeam = shuffledTeams[i];
                var awayTeam = shuffledTeams[i + 1];

                // Simulate the number of goals in the match based on the calculated average
                int homeGoals = SimulateGoals(random);
                int awayGoals = SimulateGoals(random);


                // Update goals for and goals against
                homeTeam.GoalsFor += homeGoals;
                homeTeam.GoalsAgainst += awayGoals;
                awayTeam.GoalsFor += awayGoals;
                awayTeam.GoalsAgainst += homeGoals;

                // Update goal difference
                homeTeam.GoalDifference = homeTeam.GoalsFor - homeTeam.GoalsAgainst;
                awayTeam.GoalDifference = awayTeam.GoalsFor - awayTeam.GoalsAgainst;

                // Determine the match result
                if (homeGoals > awayGoals)
                {
                    // Home team wins
                    homeTeam.Wins++;
                    homeTeam.Points += 3; // 3 points for a win
                    awayTeam.Losses++;
                    homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'W'); // Update current streak
                    awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'L'); // Update current streak
                }
                else if (homeGoals < awayGoals)
                {
                    // Away team wins
                    awayTeam.Wins++;
                    awayTeam.Points += 3; // 3 points for a win
                    homeTeam.Losses++;
                    homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'L'); // Update current streak
                    awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'W'); // Update current streak
                }
                else
                {
                    // It's a draw
                    homeTeam.Draws++;
                    homeTeam.Points += 1; // 1 point for a draw
                    awayTeam.Draws++;
                    awayTeam.Points += 1; // 1 point for a draw
                    homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'D'); // Update current streak
                    awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'D'); // Update current streak
                }

                // Increment games played for both teams
                homeTeam.GamesPlayed++;
                awayTeam.GamesPlayed++;
          }
        }

        

    void Shuffle(List<FootballTeam> list, Random random)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            FootballTeam temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void FinalsSimulation(int roundIndex, List<FootballTeam> teamsData)
    {
        Random random = new Random();

        // Split teams into upper and lower divisions based on position
        var upperDivision = teamsData.Take(6).ToList();
        var lowerDivision = teamsData.Skip(teamsData.Count - 6).ToList();

        // Shuffle the teams within their respective divisions
        Shuffle(upperDivision, random);
        Shuffle(lowerDivision, random);

        // Simulate matches for the upper division
        for (int i = 0; i < upperDivision.Count; i += 2)
        {
            var homeTeam = upperDivision[i];
            var awayTeam = upperDivision[i + 1];

            // Simulate the number of goals in the match based on the calculated average
            int homeGoals = SimulateGoals(random);
            int awayGoals = SimulateGoals(random);

            // Update goals for and goals against
            homeTeam.GoalsFor += homeGoals;
            homeTeam.GoalsAgainst += awayGoals;
            awayTeam.GoalsFor += awayGoals;
            awayTeam.GoalsAgainst += homeGoals;

            // Update goal difference
            homeTeam.GoalDifference = homeTeam.GoalsFor - homeTeam.GoalsAgainst;
            awayTeam.GoalDifference = awayTeam.GoalsFor - awayTeam.GoalsAgainst;

            // Determine the match result
            if (homeGoals > awayGoals)
            {
                // Home team wins
                homeTeam.Wins++;
                homeTeam.Points += 3; // 3 points for a win
                awayTeam.Losses++;
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'W'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'L'); // Update current streak
            }
            else if (homeGoals < awayGoals)
            {
                // Away team wins
                awayTeam.Wins++;
                awayTeam.Points += 3; // 3 points for a win
                homeTeam.Losses++;
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'L'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'W'); // Update current streak
            }
            else
            {
                // It's a draw
                homeTeam.Draws++;
                homeTeam.Points += 1; // 1 point for a draw
                awayTeam.Draws++;
                awayTeam.Points += 1; // 1 point for a draw
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'D'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'D'); // Update current streak
            }

            // Increment games played for both teams
            homeTeam.GamesPlayed++;
            awayTeam.GamesPlayed++;
        }

        // Simulate matches for the lower division
        for (int i = 0; i < lowerDivision.Count; i += 2)
        {
            var homeTeam = lowerDivision[i];
            var awayTeam = lowerDivision[i + 1];

            // Simulate the number of goals in the match based on the calculated average
            int homeGoals = SimulateGoals(random);
            int awayGoals = SimulateGoals(random);

            // Update goals for and goals against
            homeTeam.GoalsFor += homeGoals;
            homeTeam.GoalsAgainst += awayGoals;
            awayTeam.GoalsFor += awayGoals;
            awayTeam.GoalsAgainst += homeGoals;

            // Update goal difference
            homeTeam.GoalDifference = homeTeam.GoalsFor - homeTeam.GoalsAgainst;
            awayTeam.GoalDifference = awayTeam.GoalsFor - awayTeam.GoalsAgainst;

            // Determine the match result
            if (homeGoals > awayGoals)
            {
                // Home team wins
                homeTeam.Wins++;
                homeTeam.Points += 3; // 3 points for a win
                awayTeam.Losses++;
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'W'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'L'); // Update current streak
            }
            else if (homeGoals < awayGoals)
            {
                // Away team wins
                awayTeam.Wins++;
                awayTeam.Points += 3; // 3 points for a win
                homeTeam.Losses++;
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'L'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'W'); // Update current streak
            }
            else
            {
                // It's a draw
                homeTeam.Draws++;
                homeTeam.Points += 1; // 1 point for a draw
                awayTeam.Draws++;
                awayTeam.Points += 1; // 1 point for a draw
                homeTeam.CurrentStreak = UpdateStreak(homeTeam.CurrentStreak, 'D'); // Update current streak
                awayTeam.CurrentStreak = UpdateStreak(awayTeam.CurrentStreak, 'D'); // Update current streak
            }

            // Increment games played for both teams
            homeTeam.GamesPlayed++;
            awayTeam.GamesPlayed++;
       }
    }


     string UpdateStreak(string currentStreak, char result)
    {
        if (currentStreak == null || currentStreak.Length == 0)
        {
            // If the streak is empty, start a new streak
            return $"{result}";
        }

        char lastResult = currentStreak[currentStreak.Length - 1];
        if (lastResult == result)
        {
            // If the last result matches the current result, continue the streak
            int streakLength = currentStreak.Length;
            if (char.IsDigit(lastResult))
            {
                streakLength = int.Parse(lastResult.ToString()) + 1;
            }
            return $"{streakLength}{result}";
        }
        else
        {
            // If the last result is different, start a new streak
            return $"{result}";
        }
    }



    int SimulateGoals(Random random)
    {
        // Simulate the number of goals using a Poisson distribution
        double lambda = 1.5; // Adjust this value based on your desired average goals per match
        int goals = PoissonRandom(random, lambda);

        return goals;
    }


    int PoissonRandom(Random random, double lambda)
    {
        double L = Math.Exp(-lambda);
        double p = 1.0;
        int k = 0;

        do
        {
            k++;
            p *= random.NextDouble();
        }
        while (p > L);

        return k - 1;
    } 

        static LeagueSetup? LoadSetupData(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    // Skip the header row
                    reader.ReadLine();

                    var line = reader.ReadLine(); // Read the data row
                    var values = line.Split(',');

                    if (values.Length != 6)
                    {
                        throw new InvalidOperationException("Invalid data format in setup CSV.");
                    }

                    var setupData = new LeagueSetup(
                        values[0].Trim(),
                        int.Parse(values[1]),
                        int.Parse(values[2]),
                        int.Parse(values[3]),
                        int.Parse(values[4]),
                        int.Parse(values[5])
                    );

                    return setupData;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error loading and validating setup data.", ex);
            }
        }

       static List<FootballTeam> LoadTeamsData(string filePath)
        {
            try
            {
                var teams = new List<FootballTeam>();
                int lineNumber = 0; // Initialize the line number counter

                using (var reader = new StreamReader(filePath))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineNumber++; // Increment the line number

                        if (lineNumber == 1) // Skip the header line
                        {
                            continue;
                        }

                        var values = line.Split(',');

                        // Check if the values array has the expected length (11)
                        if (values.Length != 11)
                        {
                            Console.WriteLine($"Invalid data format in teams CSV on line {lineNumber}: {line}");
                            continue; // Skip this line and continue to the next one
                        }

                        string shortClubName = values[1].Trim(); // Parse it as a string

                        int gamesPlayed;
                        if (!int.TryParse(values[2], out gamesPlayed))
                        {
                            // Handle the case where 'gamesPlayed' is not a valid integer
                            Console.WriteLine($"Invalid 'gamesPlayed' value on line {lineNumber}: {values[2]}");
                            continue; // Skip this line and continue to the next one
                        }

                        // Similarly, you can apply similar checks and parsing for other integer fields

                        var team = new FootballTeam(
                            values[0].Trim(),       // FullClubName (string)
                            values[1].Trim(),       // ShortClubName (string)
                            gamesPlayed,            // Parsed 'gamesPlayed' value
                            values[3].Trim(),       // SpecialRanking (string)
                            0,                      // wins (int)
                            0,                      // draws (int)
                            0,                      // losses (int)
                            int.Parse(values[4]),   // goalsFor (int)
                            int.Parse(values[5]),   // goalsAgainst (int)
                            int.Parse(values[6]),   // goalDifference (int)
                            int.Parse(values[7]),   // points (int)
                            int.Parse(values[8]),   // position
                            values[9].Trim()        // currentStreak (string)
                        );

                        teams.Add(team);
                    }
                }

                return teams;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error loading and validating teams data.", ex);
            }
        }
}