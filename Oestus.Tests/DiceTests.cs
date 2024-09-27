namespace Oestus.Tests;

using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;
public class DiceTests
{
    private readonly ITestOutputHelper output;

    public DiceTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    public class ConsoleWriter : StringWriter
    {
        private ITestOutputHelper output;
        public ConsoleWriter(ITestOutputHelper output)
        {
            this.output = output;
        }
        public override void WriteLine(string? m)
        {
            output.WriteLine(m);
        }
    }

    [Fact]
    public void Oestus_RNGTest()
    {
        int x = OestusRNG.Next(1, 101);
        output.WriteLine($"RNG Base Output: {x}");
        Assert.InRange(x, 1, 100);
    }

    [Fact]
    public void Dice_SingleRoll(){
        int x = Dice.RollDice(100);
        output.WriteLine($"Dice Roll Output: {x}");
        Assert.InRange(x, 1, 100);
    }

    [Fact]
    public void Dice_100Rolls(){
        int x = Dice.RollDice(100, 100, out var y);
        output.WriteLine($"100 Dice Output: {x} | {y.ToDiceString()}");
        Assert.InRange(x, 1,100*100);
        Assert.True(y.Count() == 100);
    }

    [Fact]
    public void Dice_100Adv(){
        int x = Dice.RollDiceAdv(100, 100, out var y, out var z4);
        output.WriteLine($"100 Dice Advantage: {x} | {y.ToDiceString(z4)}");
        Assert.InRange(x, 1, 100*100);
        Assert.True(y.Count() == 100 && z4.Count() == 100);
    }

    [Fact]
    public void Dice_ParseSimple(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("d20", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 200);
    }

    [Fact]
    public void Dice_ParseNested(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("10d8d20", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 10*8*20);
    }

    [Fact]
    public void Dice_ParseAdvantage(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("5d20a", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 100);
    }

    [Fact]
    public void Dice_ParseDisadvantage(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("5d20s", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 100);
    }

    [Fact]
    public void Dice_ParseFudge(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("6df", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, -6, 6);
    }

    [Fact]
    public void Dice_ParseAddInt(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20+6", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 7, 26);
    }

    [Fact]
    public void Dice_ParseAddDice(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20+d8", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 2, 28);
    }

    [Fact]
    public void Dice_ParseSubInt(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20-6", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, -5, 14);
    }

    [Fact]
    public void Dice_ParseSubDice(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20-d8", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, -7, 19);
    }

    [Fact]
    public void Dice_ParseAddFudge(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20+3dF", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, -2, 23);
    }

    [Fact]
    public void Dice_ParseSubtractFudge(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20-3dF", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, -2, 23);
    }

    [Fact]
    public void Dice_ParseComplex(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20+2d10+8dF+16-1d8+14df+100d3", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 89, 377);
    }

    [Fact]
    public void Dice_DropLowest()
    {
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("4d6dl1", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 3, 18);
    }

    [Fact]
    public void Dice_DropHighest()
    {
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("4d6dh1", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 3, 18);
    }

    [Fact]
    public void Dice_Parenthesis(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("(1d4)d20", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 80);
    }

    [Fact]
    public void Dice_Parenthesis2(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("(1d4+2)d(12d6)", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 432);
    }

    [Fact]
    public void Dice_Minimum(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20dm10", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 10, 20);
    }
    [Fact]
    public void Dice_Minimum2(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("8d20dm10", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 80, 160);
    }
    [Fact]
    public void Dice_Maximum(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("1d20dx10", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 1, 10);
    }
    [Fact]
    public void Dice_Maximum2(){
        Console.SetOut(new ConsoleWriter(output));
        int x = Dice.Parse("8d20dx10", out var y);
        output.WriteLine($"Dice Parser Output: {x} | {y}");
        Assert.InRange(x, 8, 80);
    }
}