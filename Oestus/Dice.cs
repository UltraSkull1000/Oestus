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

            int count = 0, faces = 0, multiplier = 1, total = 0; // count is the number of dice. faces is the number of faces on the die. multiplier determines whether the number is negative. total is the current total of the given results.
            bool die = false; //whether we are definitively rolling a die or not.

            for (int i = 0; i < query.Length; i++) //for each character in the query. TODO: Possibly Parallelize by splitting across mathematical symbols.
            {
                bool EOQ = i >= query.Length - 1; //Whether we are at the end of the query or not
                char current = query[i];
                char next = EOQ ? '#' : query[i + 1];

                if (!isValidCharacter(current)) //validate whether the current character in the stream is valid in terms of dice. 
                    throw new ArgumentException("Invalid Character in Parsing Stream: " + current);

                if (char.IsNumber(current)) //If the character is a number, push it on to the digit stack. 
                {
                    digitStack.Push(current - '0');
                    if (!EOQ)
                        continue;
                }

                if (!die)
                {
                    if (current == 'd' && !die) //roll over into die mode
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

                    if (current == '+' || current == '-') //we're adding to an integer.
                    {
                        faces = digitStack.decompressStack(); //get that integer

                        if (faces != 0) //the integer isnt 0. otherwise, ignore.
                        {
                            total += multiplier * faces;
                            resultString += faces;
                        }

                        if (current == '-') // if we're subtracting, the multiplier needs to be -1.
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
                    if (current == '+' || current == '-' || EOQ && (current != 'a') && (current != 's') && (current != 'f')) //Roll a normal numerical die!
                    {
                        if (digitStack.Count() == 0)
                            throw new ArgumentException("Query format is invalid.");
                        faces = digitStack.decompressStack();
                        total += multiplier * ProcessDice(faces, count, out var dres); //helper function to process dice into dice rolls.
                        resultString += $"({count}d{faces} = {dres})"; //add this roll and its results to the result string
                        if (current == '-') //subtraction
                            multiplier = -1;
                        else //addition
                            multiplier = 1;
                        die = false; //no longer in die mode
                        count = 0; //reset dice counter
                    }


                    if (current == 'd')
                    {
                        if (digitStack.Count() == 0) //oh nevermind, we've double stacked d's
                            throw new ArgumentException("Query format is invalid (too many symbols in a row)");
                        faces = digitStack.decompressStack();
                        if (next == 'l' || next == 'h')
                        { //we are dropping the lowest or highest
                            i += 2;
                            int j = i;
                            for (j = i; j < query.Length; j++)
                            {
                                if (!char.IsNumber(query[j]))
                                    break;
                                digitStack.Push(query[j] - '0');
                            }
                            i = j;
                            int drop = digitStack.decompressStack();
                            if (next == 'l') //dropping lowest
                            {
                                total += multiplier * ProcessDice(faces, count, out var dres, ProcessType.DropLowest, drop);
                                resultString += $"{count}d{faces}dl{drop} = {dres}";
                                die = false;
                                count = 0;
                                faces = 0;
                            }
                            else
                            { //dropping highest
                                total += multiplier * ProcessDice(faces, count, out var dres, ProcessType.DropHighest, drop);
                                resultString += $"{count}d{faces}dh{drop} = {dres}";
                                die = false;
                                count = 0;
                                faces = 0;
                            }
                            continue;
                        }

                        else //okay, we have a nested die here.
                        {
                            count = ProcessDice(faces, count, out var _); //instead of pushing the result to the total, we roll the die and have a new count.
                            faces = 0; //reset faces.
                            continue;
                        }
                    }

                    if (current == 'f') //we're rolling fudge dice!
                    {
                        total += multiplier * ProcessDice(0, count, out var dres, ProcessType.Fudge);
                        resultString += $"{count}dF = {dres}";
                        die = false;
                        count = 0;
                        faces = 0;
                    }

                    if (current == 'a' || current == 's') //we are rolling with advantage or disadvantage
                    {
                        if (digitStack.Count() == 0)
                            throw new ArgumentException("Query format is invalid.");
                        faces = digitStack.decompressStack();
                        var dres = "";
                        if (current == 'a') //advantage
                            total += ProcessDice(faces, count, out dres, ProcessType.Advantage);
                        else //disadvantage
                            total += ProcessDice(faces, count, out dres, ProcessType.Disadvantage);
                        resultString += $"{count}d{faces}{current} = {dres}";
                        die = false;
                        count = 0;
                    }
                }

                if (current == '+' || current == '-') //print symbols to resultant string.
                {
                    resultString += $" {current} ";
                }
            }
            return total;
        }

        enum ProcessType
        {
            Default,
            Advantage,
            Disadvantage,
            DropLowest,
            DropHighest,
            Fudge
        }
        private static int ProcessDice(int faces, int count, out string resultString, ProcessType type = ProcessType.Default, int drop = 0)
        {
            if (count <= 0) //there are no dice to roll!
                throw new ArgumentOutOfRangeException("Number of Dice cannot be less than or equal to 0.");
            if (faces <= 0 && type != ProcessType.Fudge) //cannot have a number of faces that can't exist in 3 dimensions. Fudge defaults to 3 sides. 
                throw new ArgumentOutOfRangeException("Number of Faces cannot be less than or equal to 0.");

            resultString = ""; //init result string
            int result = 0;
            if (count == 1)
            {
                switch (type)
                {
                    case ProcessType.Default:
                        result = RollDice(faces);
                        resultString += $"{result}";
                        break;
                    case ProcessType.Advantage:
                        result = RollDiceAdv(faces, out var x);
                        resultString += $"{result} \u0336{x}";
                        break;
                    case ProcessType.Disadvantage:
                        result = RollDiceDis(faces, out var y);
                        resultString += $"{result} \u0336{y}";
                        break;
                    case ProcessType.DropLowest:
                    case ProcessType.DropHighest:
                        throw new ArgumentOutOfRangeException("Cannot Drop from a Single Die!");
                    case ProcessType.Fudge:
                        result = RollFudgeDice();
                        resultString += $"{result}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Type was outside of bounded enum.");
                }
            }
            else
            {
                switch (type)
                {
                    case ProcessType.Default:
                        result = RollDice(faces, count, out List<int> l1);
                        resultString += $"{result} {l1.ToDiceString()}";
                        break;
                    case ProcessType.Advantage:
                        result = RollDiceAdv(faces, count, out var m1, out var m2);
                        resultString += $"{result} {m1.ToDiceString(m2)}";
                        break;
                    case ProcessType.Disadvantage:
                        result = RollDiceDis(faces, count, out var n1, out var n2);
                        resultString += $"{result} {n1.ToDiceString(n2)}";
                        break;
                    case ProcessType.DropLowest:
                        RollDice(faces, count, out var o1);
                        result = Drop(drop, o1, out var _, out var o2);
                        resultString += $"{result} {o2}";
                        break;
                    case ProcessType.DropHighest:
                        RollDice(faces, count, out var p1);
                        result = Drop(drop, p1, out var _, out var p2, true);
                        resultString += $"{result} {p2}";
                        break;
                    case ProcessType.Fudge:
                        result = RollFudgeDice(count, out var q1);
                        resultString = $"{result} {q1.ToDiceString()}";
                        break;
                }
            }
            return result;
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
            'f',
            'l',
            'h'
        };
        private static bool isValidCharacter(char c) => validchars.Contains(c);
        public static int RollDice(int faces) => OestusRNG.Next(1, faces + 1);
        public static int RollDice(int faces, int count, out List<int> result)
        {
            result = OestusRNG.Next(1, faces + 1, count);
            return result.Sum();
        }
        public static int RollDiceAdv(int faces, out int alt)
        {
            int roll1 = OestusRNG.Next(1, faces);
            int roll2 = OestusRNG.Next(1, faces);
            int x = roll1 > roll2 ? roll1 : roll2;
            if (roll1 == x)
                alt = roll2;
            else
                alt = roll1;
            return x;
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
        public static int Drop(int amount, List<int> list, out List<int> results, out string listResults, bool highest = false)
        {
            if (amount >= list.Count())
                throw new ArgumentOutOfRangeException(nameof(amount));
            list = highest ? list.OrderDescending().Reverse().ToList() : list.OrderDescending().ToList();
            listResults = "{(";
            for (int i = 0; i < list.Count(); i++)
            {
                if (i > list.Count() - 1 - amount)
                    listResults += "\u0336";
                listResults += $"{list[i]}";
                if (i != list.Count() - 1)
                    listResults += "), (";
            }
            listResults += ")}";
            results = list.Take(list.Count() - amount).ToList();
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