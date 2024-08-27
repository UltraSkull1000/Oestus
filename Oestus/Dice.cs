using System.Diagnostics;

namespace Oestus
{
    public static class Dice
    {
        public static int Parse(string query, out string resultString)
        {
            resultString = ""; //initialize
            query = query.ToLower(); //for character read consistency
            query.Replace(" ", ""); //remove white spaces

            if (string.IsNullOrEmpty(query)) //query cannot be empty. 
                throw new ArgumentNullException($"{nameof(query)} must not be null or empty.");

            Stack<int> digitStack = new Stack<int>(); //used to stack digits while reading to next instruction.

            int count = 1, faces = 1, multiplier = 1, total = 0; // count is the number of dice. faces is the number of faces on the die. multiplier determines whether the number is negative. total is the current total of the given results.
            bool die = false; //whether we are definitively rolling a die or not.

            for (int i = 0; i < query.Length; i++) //for each character in the query. TODO: Possibly Parallelize by splitting across mathematical symbols.
            {
                bool EOQ = i == query.Length - 1; //Whether we are at the end of the query or not

                if (!isValidCharacter(query[i])) //validate whether the current character in the stream is valid in terms of dice. 
                    throw new ArgumentException("Invalid Character in Parsing Stream: " + query[i]);

                if (char.IsNumber(query[i])) //If the character is a number, push it on to the digit stack. 
                {
                    digitStack.Push(query[i] - '0');
                    if (!EOQ)
                        continue;
                }

                if (!die)
                {
                    if (query[i] == 'd' && !die) //roll over into die mode
                    {
                        if (digitStack.Count() == 0) //there was no number preceding d, so treat it as if you are rolling only one die.
                            count = 1;
                        else //otherwise, process the digit stack.
                        {
                            count = digitStack.decompressStack(); //helper function to break down the stack. 
                        }
                        die = true; //we're in die mode baby!
                        continue;
                    }

                    if (query[i] == '+' || query[i] == '-') //we're adding to an integer.
                    {
                        faces = digitStack.decompressStack(); //get that integer

                        if (faces != 0) //the integer isnt 0. otherwise, ignore.
                        {
                            total += multiplier * faces;
                            resultString += faces;
                        }

                        if (query[i] == '-') // if we're subtracting, the multiplier needs to be -1.
                            multiplier = -1;
                        else //otherwise, reset.
                            multiplier = 1;
                    }

                    if (EOQ) //we've reached the end of the query in an integer.
                    {
                        if (digitStack.Count() == 0)
                            continue;
                        faces = digitStack.decompressStack();
                        total += multiplier * faces;
                        resultString += faces;
                    }
                }
                if (die) //die mode
                {
                    if (query[i] == '+' || query[i] == '-' || EOQ && (query[i] != 'a') && (query[i] != 's') && (query[i] != 'f')) //Roll a normal numerical die!
                    {
                        if (digitStack.Count() == 0)
                            throw new ArgumentException("Query format is invalid.");
                        faces = digitStack.decompressStack(); 
                        total += multiplier * ProcessDice(count, faces, out var dres); //helper function to process dice into dice rolls.
                        resultString += $"({count}d{faces} = {dres})"; //add this roll and its results to the result string
                        if (query[i] == '-') //subtraction
                            multiplier = -1;
                        else //addition
                            multiplier = 1;
                        die = false; //no longer in die mode
                        count = 0; //reset dice counter
                    }


                    if (query[i] == 'd') //okay, we have a nested die here.
                    {
                        if (digitStack.Count() == 0) //oh nevermind, we've double stacked d's
                            throw new ArgumentException("Query format is invalid (too many symbols in a row)");
                        faces = digitStack.decompressStack();
                        count = ProcessDice(count, faces, out var _); //instead of pushing the result to the total, we roll the die and have a new count.
                        faces = 0; //reset faces.
                        continue;
                    }

                    if (query[i] == 'f') //we're rolling fudge dice!
                    {
                        total += multiplier * RollFudgeDice(count, out var list);
                        resultString += $"({count}dF = {list.Sum()} {list.ToDiceString()})";
                        die = false;
                        count = 0;
                        faces = 0;
                    }

                    if (query[i] == 'a' || query[i] == 's') //we are rolling with advantage or disadvantage
                    {
                        if (digitStack.Count() == 0)
                            throw new ArgumentException("Query format is invalid.");
                        faces = digitStack.decompressStack();
                        var dres = "";
                        if(query[i] == 'a') //advantage
                            total += ProcessDiceAdv(count, faces, out dres);
                        else //disadvantage
                            total += ProcessDiceDis(count, faces, out dres);
                        resultString += $"{count}d{faces}{query[i]} = {dres}";
                        die = false;
                        count = 0;
                    }
                }

                if (query[i] == '+' || query[i] == '-') //print symbols to resultant string.
                {
                    resultString += $" {query[i]} ";
                }
            }
            return total;
        }

        // TODO: compress processdice functions
        private static int ProcessDice(int c, int f, out string res)
        {
            if (f == 0)
                throw new ArgumentOutOfRangeException("Number of Faces on a Die cannot be 0.");
            res = "";
            if (c == 1)
            {
                var result = RollDice(f);
                res += $"{result}";
                return result;
            }
            else
            {
                var result = RollDice(f, c, out var list);
                res += $"{result} {list.ToDiceString()}";
                return result;
            }
        }
        private static int ProcessDiceAdv(int c, int f, out string res)
        {
            if (f == 0)
                throw new ArgumentOutOfRangeException("Number of Faces on a Die cannot be 0.");
            res = "";
            if (c == 1)
            {
                var result = RollDiceAdv(f, out var x);
                res += $"{result} ◌̶{x}";
                return result;
            }
            else
            {
                var result = RollDiceAdv(f, c, out var l1, out var l2);
                res += $"{result} {l1.ToDiceString(l2)}";
                return result;
            }
        }
        private static int ProcessDiceDis(int c, int f, out string res)
        {
            if (f == 0)
                throw new ArgumentOutOfRangeException("Number of Faces on a Die cannot be 0.");
            res = "";
            if (c == 1)
            {
                var result = RollDiceDis(f, out var x);
                res += $"{result} ◌̶{x}";
                return result;
            }
            else
            {
                var result = RollDiceDis(f, c, out var l1, out var l2);
                res += $"{result} {l1.ToDiceString(l2)}";
                return result;
            }
        }

        private static int decompressStack(this Stack<int> digitStack)
        {
            int count = 0;
            for (int j = 0; digitStack.Count() > 0; j++)
            {
                var ch = digitStack.Pop();
                count += ch * (int)Math.Pow(10, j);
            }
            return count;
        }
        private static char[] validchars = {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'd',
            'a',
            's',
            '+',
            '-',
            'f'
        };
        private static bool isValidCharacter(char c) => validchars.Contains(c);

        public static int RollDice(int faces) => OestusRNG.Next(1, faces + 1);

        public static int RollDiceAdv(int faces, out int alt)
        {
            int roll1 = OestusRNG.Next(1, faces);
            int roll2 = OestusRNG.Next(1, faces);
            int x = roll1>roll2 ? roll1 : roll2;
            if (roll1 == x)
                alt = roll2;
            else
                alt = roll1;
            return x;
        }

        public static int RollDiceDis(int faces, out int alt)
        {
            int roll1 = OestusRNG.Next(1, faces);
            int roll2 = OestusRNG.Next(1, faces);
            int x = Math.Min(roll1, roll2);
            if (roll1 == x)
                alt = roll2;
            else
                alt = roll1;
            return x;
        }

        public static int RollDice(int faces, int count, out List<int> result)
        {
            result = OestusRNG.Next(1, faces + 1, count);
            return result.Sum();
        }

        public static int RollDiceAdv(int faces, int count, out List<int> l1, out List<int> l2)
        {
            l1 = new List<int>();
            l2 = new List<int>();
            for (int i = 0; i < count; i++)
            {
                l1.Add(RollDiceAdv(faces, out var x));
                l2.Add(x);
            }
            return l1.Sum();
        }
        public static int RollDiceDis(int faces, int count, out List<int> l1, out List<int> l2)
        {
            l1 = new List<int>();
            l2 = new List<int>();
            for (int i = 0; i < count; i++)
            {
                l1.Add(RollDiceDis(faces, out var x));
                l2.Add(x);
            }
            return l1.Sum();
        }
        public static bool RollAgainst(int faces, int dc, out int result, int offset = 0)
        {
            result = RollDice(faces);
            if ((result + offset) >= dc) return true;
            else return false;
        }
        public static int RollAgainst(int faces, int count, int dc, out List<int> results, int offset = 0)
        {
            RollDice(faces, count, out results);
            int successes = results.Where(x => (x + offset) >= dc).Count();
            return successes;
        }
        public static int RollFudgeDice()
        {
            var faceroll = OestusRNG.Next(1, 4);
            return faceroll - 2;
        }
        public static int RollFudgeDice(int count, out List<int> results)
        {
            List<int> facerolls = OestusRNG.Next(1, 4, count);
            results = facerolls.Select(x => { return x - 2; }).ToList();
            return results.Sum();
        }
        public static List<string> TranslateRolls(List<int> rolls, List<string> translations)
        {
            if (rolls.Max() >= translations.Count())
                throw new ArgumentException($"Table {nameof(translations)} missing required definitions. Expected at least {rolls.Max()} options.");
            return rolls.Select(x => { return translations[x]; }).ToList();
        }
        public static string ToDiceString(this List<int> rolls)
        {
            string res = "{(";
            res += string.Join(") + (", rolls);
            res += ")}";
            return res;
        }
        public static string ToDiceString(this List<string> rolls)
        {
            string res = "{[";
            res += string.Join("], [", rolls);
            res += "]}";
            return res;
        }

        public static string ToDiceString(this List<int> rolls, List<int> alt)
        {
            string res = "{";
            for (int i = 0; i < rolls.Count; i++)
            {
                res += $"[{rolls[i]}, \u0336{alt[i]}]";
            }
            res += "}";
            return res;
        }
    }
}