using System;

namespace Codezerg.OpenRouter.Examples;

[AttributeUsage(AttributeTargets.Class)]
public class ExampleAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public ExampleAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
