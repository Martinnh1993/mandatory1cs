namespace footballManager
{
    public class FootballTeam
    {
        public string FullClubName { get; set; }
        public string ShortClubName { get; set; }
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
        public string SpecialRanking { get; set; }
        public string CurrentStreak { get; set; }
        public int Position { get; set; } // Include the Position property

        public FootballTeam(
            string fullClubName, 
            string shortClubName,
            int gamesPlayed, 
            string specialRanking,
            int wins, 
            int draws, 
            int losses, 
            int goalsFor, 
            int goalsAgainst, 
            int goalDifference, 
            int points, 
            int position,
            string currentStreak
            )
        {
            FullClubName = fullClubName;
            ShortClubName = shortClubName;
            GamesPlayed = gamesPlayed;
            SpecialRanking = specialRanking; // Initialize with an empty string
            Wins = wins;
            Draws = draws;
            Losses = losses;
            GoalsFor = goalsFor;
            GoalsAgainst = goalsAgainst;
            GoalDifference = goalDifference;
            Points = points;
            Position = position; // Initialize the Position property
            CurrentStreak = currentStreak; // Initialize 'CurrentStreak' with an empty string
        }
    }
}
