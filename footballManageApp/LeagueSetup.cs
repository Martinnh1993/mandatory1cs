namespace footballManager
{
    public class LeagueSetup
    {
        
        public string LeagueName { get; set; }
        public int PromoteToChampionsLeague { get; set; }
        public int PromoteToEuropeLeague { get; set; }
        public int PromoteToConferenceLeague { get; set; }
        public int PromoteToUpperLeague { get; set; }
        public int RelegateToLowerLeague { get; set; }

        public LeagueSetup(string leagueName, int promoteToChampionsLeague, int promoteToEuropeLeague, int promoteToConferenceLeague, int promoteToUpperLeague, int relegateToLowerLeague)
        {
            // Perform validation here
            if (string.IsNullOrWhiteSpace(leagueName))
            {
                throw new InvalidOperationException("LeagueName cannot be empty or null.");
            }

            // Add more validation logic for other properties as needed...

            // Assign values after validation
            LeagueName = leagueName;
            PromoteToChampionsLeague = promoteToChampionsLeague;
            PromoteToEuropeLeague = promoteToEuropeLeague;
            PromoteToConferenceLeague = promoteToConferenceLeague;
            PromoteToUpperLeague = promoteToUpperLeague;
            RelegateToLowerLeague = relegateToLowerLeague;
        }
    }
}