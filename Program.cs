using Pulumi;
using Pulumi.Automation;
using Pulumi.AzureNative.Resources;
using System;
using System.Collections.Generic;

var stackNameOne = "FirstStack";
var stackNameTwo = "SecondStack";

var projectName = "MyProject";

var stackOneArgs = new InlineProgramArgs(projectName, stackNameOne, PulumiFn.Create<MyFirstStack>());
var stackTwoArgs = new InlineProgramArgs(projectName, stackNameTwo, PulumiFn.Create<MySecondStack>());


using var stackOne = await LocalWorkspace.CreateOrSelectStackAsync(stackOneArgs);
using var stackTwo = await LocalWorkspace.CreateOrSelectStackAsync(stackTwoArgs);

// Destroy Stacks
if (args.Length > 0 && args[0] == "destroy")
{
    await stackOne.DestroyAsync(new DestroyOptions { OnStandardOutput = Console.WriteLine, OnStandardError = Console.WriteLine });
    await stackTwo.DestroyAsync(new DestroyOptions { OnStandardOutput = Console.WriteLine, OnStandardError = Console.WriteLine });
    return;
}

// Create the resources in Stack One
var stackOneResult = await stackOne.UpAsync(new UpOptions
{
    OnStandardOutput = Console.WriteLine,
    OnStandardError = Console.WriteLine,
});

// Get Resource Id from Stack One
var resourceGroupId = stackOneResult.Outputs["resourceGroupId"].Value.ToString();

var stackTwoOutputs = await stackTwo.GetOutputsAsync();

// Import the resources from Stack One into Stack Two if the resource doesn't exist
if (!stackTwoOutputs.ContainsKey("resourceGroupId"))
{
    await stackTwo.ImportAsync(new ImportOptions
    {
        Protect = false,
        GenerateCode = false,
        OnStandardError = Console.WriteLine,
        OnStandardOutput = Console.WriteLine,
        Resources = new List<ImportResource>
        {
            new()
            {
                Type = "azure-native:resources:ResourceGroup",
                Name = "resourceGroup",
                Id = resourceGroupId,
            },
        },
    });
}

// Run Up on Stack Two
await stackTwo.UpAsync(new UpOptions
{
    OnStandardOutput = Console.WriteLine,
    OnStandardError = Console.WriteLine,
});

public class MyFirstStack : Stack
{
    public MyFirstStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("resourceGroup", 
            new ResourceGroupArgs 
            { 
                Location = "westeurope", 
                ResourceGroupName = "My-Resource-Group"
            }, 
            new CustomResourceOptions
            {
                RetainOnDelete = true
            });

        this.ResourceGroupId = resourceGroup.Id;
    }

    [Output("resourceGroupId")]
    public Output<string> ResourceGroupId { get; set; }
}

public class MySecondStack : Stack
{
    public MySecondStack()
    {

        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("resourceGroup",
            new ResourceGroupArgs
            {
                Location = "westeurope",
                ResourceGroupName = "My-Resource-Group"
            }, new CustomResourceOptions { });

        this.ResourceGroupId = resourceGroup.Id;
    }

    [Output("resourceGroupId")]
    public Output<string> ResourceGroupId { get; set; }
}