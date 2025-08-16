using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ingredient
{
    public string Name { get; }
    public double Quantity { get; }
    public string Unit { get; }

    public Ingredient(string name, double quantity, string unit)
    {
        Name = name;
        Quantity = quantity;
        Unit = NormalizeUnit(unit);
    }

    private string NormalizeUnit(string unit)
    {
        unit = unit.Trim().ToLower();
        switch (unit)
        {
            case "grams":
            case "gram":
            case "g":
                return "g";
            case "kilograms":
            case "kg":
                return "kg";
            case "milliliters":
            case "ml":
                return "ml";
            case "liters":
            case "l":
                return "l";
            case "pieces":
            case "pcs":
                return "pcs";
            default:
                return unit; 
        }
    }

    public double ConvertTo(string targetUnit)
    {
        if (Unit == targetUnit) return Quantity;

        if (Unit == "g" && targetUnit == "kg") return Quantity / 1000;
        if (Unit == "kg" && targetUnit == "g") return Quantity * 1000;
        if (Unit == "ml" && targetUnit == "l") return Quantity / 1000;
        if (Unit == "l" && targetUnit == "ml") return Quantity * 1000;

        throw new InvalidOperationException($"Cannot convert {Unit} to {targetUnit}");
    }

    public Ingredient Scale(double factor)
    {
        return new Ingredient(Name, Quantity * factor, Unit);
    }

    public override string ToString() => $"{Quantity:0.##} {Unit} {Name}";
}

public class Recipe
{
    public List<Ingredient> Ingredients { get; }
    public int BaseServings { get; }

    public Recipe(int baseServings, List<Ingredient> ingredients)
    {
        BaseServings = baseServings;
        Ingredients = ingredients;
    }

    public List<Ingredient> ScaleTo(int targetServings)
    {
        double factor = (double)targetServings / BaseServings;
        return Ingredients.Select(i => i.Scale(factor)).ToList();
    }
}

public class Pantry
{
    public List<Ingredient> Stock { get; }

    public Pantry(List<Ingredient> stock)
    {
        Stock = stock;
    }

    public List<Ingredient> GetShortages(List<Ingredient> needed)
    {
        var shortages = new List<Ingredient>();

        foreach (var required in needed)
        {
            var pantryItem = Stock.FirstOrDefault(p => p.Name.Equals(required.Name, StringComparison.OrdinalIgnoreCase));

            if (pantryItem == null)
            {
                shortages.Add(required);
                continue;
            }

            double available = pantryItem.ConvertTo(required.Unit);
            if (available < required.Quantity)
            {
                shortages.Add(new Ingredient(required.Name, required.Quantity - available, required.Unit));
            }
        }

        return shortages;
    }
}
namespace _36__Recipe_Scaler___Pantry_Checker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var recipe = new Recipe(4, new List<Ingredient>
        {
            new Ingredient("Flour", 500, "g"),
            new Ingredient("Sugar", 200, "g"),
            new Ingredient("Milk", 1, "l"),
            new Ingredient("Eggs", 4, "pcs")
        });

            var pantry = new Pantry(new List<Ingredient>
        {
            new Ingredient("Flour", 1, "kg"),
            new Ingredient("Sugar", 150, "g"),
            new Ingredient("Milk", 500, "ml"),
            new Ingredient("Eggs", 2, "pcs")
        });

            int targetServings = 6;
            var scaled = recipe.ScaleTo(targetServings);

            Console.WriteLine($"Scaled recipe for {targetServings} servings:");
            foreach (var ing in scaled)
                Console.WriteLine(" - " + ing);

            var shortages = pantry.GetShortages(scaled);

            Console.WriteLine("\nShopping List (Shortages):");
            if (!shortages.Any())
            {
                Console.WriteLine(" - None, you have everything!");
            }
            else
            {
                foreach (var s in shortages)
                    Console.WriteLine(" - " + s);
            }
        }
    }
}
