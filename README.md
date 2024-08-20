# Overview

This repo contains a sample on using the Automation API from Pulumi to create a resource using one stack and to import this into another.

## Steps to Reproduce

1. Log in to Pulumi backend `pulumi login --local`
2. Set `PULUMI_CONFIG_PASSPHRASE` environment variable to a suitable passphrase
3. [Configure Environment settings for Azure provider](https://www.pulumi.com/registry/packages/azure-native/installation-configuration/#authentication-methods)
4. Run `dotnet build` to build the project
5. Identify the built `ImportBug.exe` binary in [bin/Debug/net8.0](bin/Debug/net8.0) directory
6. Run the binary `./ImportBug.exe` to attempt to create and import the resources

### Expected Behavior

Pulumi executes successfully to create the resources and then run the import command to import the resources into the second stack.

### Actual Behavior

Pulumi executes successfully to create the resources but fails to import the resources into the second stack with the following error message:

```
Importing (SecondStack):
    pulumi:pulumi:Stack MyProject-SecondStack  running 'dotnet build -nologo .'
    pulumi:pulumi:Stack MyProject-SecondStack  MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.
    pulumi:pulumi:Stack MyProject-SecondStack  MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.
    pulumi:pulumi:Stack MyProject-SecondStack  1 message
Diagnostics:
  pulumi:pulumi:Stack (MyProject-SecondStack):
    MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.

error: failed to discover plugin requirements: 'dotnet build -nologo .' exited with non-zero exit code: 1
Unhandled exception. Pulumi.Automation.Commands.Exceptions.CommandException: code: -1
stdout: Importing (SecondStack):
    pulumi:pulumi:Stack MyProject-SecondStack  running 'dotnet build -nologo .'
    pulumi:pulumi:Stack MyProject-SecondStack  MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.
    pulumi:pulumi:Stack MyProject-SecondStack  MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.
    pulumi:pulumi:Stack MyProject-SecondStack  1 message
Diagnostics:
  pulumi:pulumi:Stack (MyProject-SecondStack):
    MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.


stderr: error: failed to discover plugin requirements: 'dotnet build -nologo .' exited with non-zero exit code: 1


   at Pulumi.Automation.Commands.LocalPulumiCommand.RunAsyncInner(IList`1 args, String workingDir, IDictionary`2 additionalEnv, Action`1 onStandardOutput, Action`1 onStandardError, EventLogFile eventLogFile, CancellationToken cancellationToken)
   at Pulumi.Automation.Commands.LocalPulumiCommand.RunAsync(IList`1 args, String workingDir, IDictionary`2 additionalEnv, Action`1 onStandardOutput, Action`1 onStandardError, Action`1 onEngineEvent, CancellationToken cancellationToken)
   at Pulumi.Automation.Workspace.RunStackCommandAsync(String stackName, IList`1 args, Action`1 onStandardOutput, Action`1 onStandardError, Action`1 onEngineEvent, CancellationToken cancellationToken)
   at Pulumi.Automation.WorkspaceStack.RunCommandAsync(IList`1 args, Action`1 onStandardOutput, Action`1 onStandardError, Action`1 onEngineEvent, CancellationToken cancellationToken)
```

## Cleaning Up

Use the [workaround](#workaround) to correct the state and then run `dotnet run -- destroy` to clean-up the resources.

## Workaround

A workaround is to run `dotnet run` against the source code. This allows Pulumi to execute `dotnet build` successfully.

