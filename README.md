# Overview

A CLI tool to get different types of information about the files and directories.
It's in an experimental stage, so things might not work as expected and the API may change with each minor version upgrade.

### Build and Run

```bash
cd /repo/folder

dotnet build
dotnet run --project .\FileInformation\FileInformation.Cli.csproj
```

### Publish AOT

```bash
cd /repo/folder

dotnet publish -c Release
```

### Install

It's only published to NuGet so far, so you can install it using the following command.
I'm planning to publish it to Snap and Homebrew in the future + add an `install.sh` script to make it easier to install.

#### On Linux
```bash
dotnet tool install --global fileinformation.cli
```
Make sure you follow any additional directions printed to the screen. You may need to update your PATH variable in order to use .NET global tools.

#### On Windows
```bash
dotnet tool install --global fileinformation.cli
```

#### On Mac
```bash
dotnet tool install --global fileinformation.cli
```
Make sure you follow any additional directions printed to the screen. You may need to update your PATH variable in order to use .NET global tools.

### Usage

```bash
# Show help
fu -h
fu count -h
fu size -h
```
```bash
# Available commands
fu count [searchPath] [OPTIONS]
fu size [searchPath] [OPTIONS]
fu tree [searchPath] [OPTIONS]
```
Examples:
```bash
# Count the size of all files in the home directory and its subdirectories 
fu size ~/ -r --hidden

# Count the size of regular files in the current directory
fu size

# Count the size of all files in the current directory and its subdirectories and display the top 10 largest directories
fu size -r --hidden --head 10

# Display all direcotries and files in the current directory and its subdirectories
fu tree -r --hidden
```