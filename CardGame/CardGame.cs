namespace CardGame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int SuitScore { get; set; }
        public List<string> Cards { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            SuitScore = 0;
            Cards = new List<string>();
        }
    }

    class CardGame
    {
        static void Main(string[] args)
        {
            // Parse command line arguments
            string inputFile, outputFile;
            (inputFile, outputFile) = GetCommandLineArguments(args);

            // Check if input and output files are provided
            if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine("Input and output files must be provided.");
                return;
            }

            // Read input file
            List<Player> players = new();
            try
            {
                DealCards(inputFile, players);
            }
            catch (Exception ex)
            {
                WriteOutput("Exception:" + ex.Message, outputFile);
                return;
            }

            // Calculate scores
            foreach (Player player in players)
            {
                foreach (string card in player.Cards)
                {
                    string face = card.Substring(0, card.Length - 1);
                    char suit = card[card.Length - 1];
                    int faceValue = GetFaceValue(face);
                    int suitValue = GetSuitValue(suit);
                    player.Score += faceValue;
                    player.SuitScore = suitValue;
                }
            }

            // Sort players by score and suit score
            players = players.OrderByDescending(x => x.Score).ThenByDescending(x => x.SuitScore).ToList();

            // Find winners
            List<Player> winners = new List<Player>();
            winners.Add(players[0]);
            for (int i = 1; i < players.Count; i++)
            {
                if (players[i].Score == winners[0].Score)
                    winners.Add(players[i]);
                else
                    i = players.Count;
            }
            // Write output
            if (winners.Count == 1)
                WriteOutput(winners[0].Name + ":" + winners[0].Score, outputFile);
            else
            {
                string output = "";
                for (int i = 0; i < winners.Count; i++)
                {
                    output += winners[i].Name;
                    if (i < winners.Count - 1)
                        output += ",";
                }
                output += ":" + winners[0].Score;
                WriteOutput(output, outputFile);
            }
        }

        private static void DealCards(string inputFile, List<Player> players)
        {
            using StreamReader sr = new StreamReader(inputFile);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] parts = line.Split(':');
                string name = parts[0].Trim();
                string[] cards = parts[1].Split(',');
                Player player = new Player(name);
                player.Cards.AddRange(cards);
                players.Add(player);
            }
        }

        private static Tuple<string, string> GetCommandLineArguments(string[] args)
        {
            string newInputFile = "";
            string newOutputFile = "";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--in")
                    newInputFile = args[i + 1];
                else if (args[i] == "--out")
                    newOutputFile = args[i + 1];
            }

            return Tuple.Create(newInputFile, newOutputFile);
        }

        static int GetFaceValue(string face)
        {
            if (int.TryParse(face, out int value))
                return value;
            else
            {
                switch (face)
                {
                    case "J": return 11;
                    case "Q": return 12;
                    case "K": return 13;
                    case "A": return 11;
                    default: throw new Exception("Invalid card face value."); 
                }
            }
        }

        static int GetSuitValue(char suit)
        {
            switch (suit)
            {
                case 'D': return 1;
                case 'H': return 2;
                case 'S': return 3;
                case 'C': return 4;
                default: throw new Exception("Invalid card face value.");
            }
        }

        static void WriteOutput(string output, string outputFile)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(outputFile);
                sw.WriteLine(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing output file: " + ex.Message);
            }
        }
    }
}