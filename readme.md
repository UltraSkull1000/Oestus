# Oestus TTRPG Random Number Generator
Developed for [UltraSkull1000/Malaco-5](https://github.com/UltraSkull1000/Malaco-5) as a universal ttrpg dice solution. Written in C# with .NET 8.0.

![Screenshot of a Discord Bot User, named Malaco, responding to a slash command, /roll, run by a user nickanamed Mac. The bot responded with \@Mac rolled d20+6 for a total of 8! with a breakdown of how that result was achieved.](https://cdn.discordapp.com/attachments/757811580552609843/1332410900480786514/image.png?ex=679527d9&is=6793d659&hm=c47edbb0be2e4b77abe322e9422d2ac6ff63d7251d28fc11ce4179b37808eed9&)

## Usage
Oestus Provides a number of ways to roll TTRPG Dice, for both backend and frontend uses. 

To roll dice in the backend, you can use the method `Oestus.Dice.RollDice(int faces)` to roll one die of the specified size, or its overload, `Oestus.Dice.RollDice(int faces, int count, out List<int> result)` to roll many dice, with the return providing a sum and the output list providing the individual dice.

Similarly, rolling fudge dice is done with the method `Oestus.Dice.RollFudgeDice()` for one die, and `Oestus.Dice.RollFudgeDice(int count, out List<int> result)` for multiple.

You can use the result lists with the helper methods, such as `Oestus.Dice.Drop()` to modify their results. Additionally, for a user-readable view of the dice, you can use `List<int>().ToDiceString()` as provided by `Oestus.Dice` to produce a formatted list.

For user-interactible dice rolling, it is recommended to use the `OestusRNG.Dice.Parse(string query, out string resultString)` to parse dice strings in the following format:

| Format          | Description                                                                                        | Example      | Sample Result                                                  | 
|-----------------|----------------------------------------------------------------------------------------------------|--------------|----------------------------------------------------------------|
| d(f)            | Rolls one die with (f) faces.                                                                      | `d20`        | `6 (1d20 = 6)`                                                 |
| (c)d(f)         | Rolls (c) dice with (f) faces.                                                                     | `3d8`        | `15 (3d8 = 15 {(2) + (6) + (7)})`                              |
| (c)dF           | Rolls (c) [fudge](https://en.wikipedia.org/wiki/Fudge_(role-playing_game_system)#Fudge_dice) dice. | `6dF`        | `0 (6dF = 0 {(1) + (-1) + (0) + (1) + (0) + (-1)})`            |
| (roll) + (i)    | Rolls a die/dice, then adds (i) to the result.                                                     | `d20+6`      | `8 (1d20 = 2) + 6`                                             |
| (roll)a         | Rolls a die with Advantage.                                                                        | `d20a`       | `11 (1d20a = 11 ̶2)`                                            |
| (roll)s         | Rolls a die with Disadvantage                                                                      | `d20s`       | `5 (1d20s = 5 ̶18)`                                             |
| (roll)dl(mn)    | Rolls dice, then drops the lowest (mn) results.                                                    | `4d6dl1`     | `12 (4d6dl1 = 12 {(6), (5), (1), (̶1)})`                        |
| (roll)dh(mx)    | Rolls dice, then drops the highest (mx) results.                                                   | `4d6dl1`     | `10 (6d8dh2 = 10 {(1), (3), (3), (3), (̶6), (̶8)})`              |
| (roll)dm(mn)    | Rolls dice, then replaces any results below (mn) with (mn).                                        | `d20dm10`    | `10 (1d20dm10 = (̶3 10))`                                       |
| (roll)dx(mx)    | Rolls dice, then replaces any results above (mx) with (mx).                                        | `3d8dx6`     | `15 (3d8dx6 = 15 {((̶7 6), 3, (̶8 6))})`                         |
| (roll) + (roll) | Any number of rolls can be added together.                                                         | `d20 + 1d4`  | `6 (1d20 = 2) + (1d4 = 4)`                                     |
| (c)d(f1)d(f2) | Trivial Nesting. For when you want to use shorthand for nesting. | `1d4d12` | `23 (3d12 = 23 {(12) + (1) + (10)})`
| (subquery)d20   | Queries can be nested using parenthesis, queries within parenthesis will be calculated first.[^1]  | `(3d6+2)d20` | `161 ((3d6 = 11 {(2) + (3) + (6)}) + 2) (13d20 = 161 {(4) ...` |

[^1]: Subqueries can also be nested within. It is recommended not to go more than 8 levels deep, but it *should* be fine. 