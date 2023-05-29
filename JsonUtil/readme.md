# CypherPotato's Json utils

This simple project fixes one of the most glaring flaws of C#'s native JSON API, which is its lack of support for decoding anonymous objects.

It provides an extremely simple API for encoding and decoding JSON objects, including anonymous objects, and also provides a method to handle these methods.

```cs
static void Main(string[] args)
{
    string json = """
        {
            "name": "Molecule Man",
            "age": 29,
            "secretIdentity": "Dan Jukes",
            "powers": [
                "Radiation resistance",
                "Turning tiny",
                "Radiation blast"
            ]
        }
        """;

    dynamic obj = JsonUtil.Shared.Deserialize(json)!;

    var name = obj.name;
    var powers = obj.powers;

    Console.WriteLine(name);      // Molecule Man
    Console.WriteLine(powers[2]); // Radiation blast
}
```

And a useful function for you to decode uncertain objects:

```cs
if (!JsonUtil.Shared.TryGetArray(() => obj.powers, out string?[]? powersArr))
{
    Console.WriteLine("Couldn't get powers!");
}

if (!JsonUtil.Shared.TryGetValue(() => (int)obj.age, out int age))
{
    Console.WriteLine("Couldn't get age!");
}
```