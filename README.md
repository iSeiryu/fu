# Overview

`fu` is a CLI tool to get different types of information about the files and directories and get is as fast as possible.
It's in an experimental stage, so things might not work as expected and the API may change with each minor version upgrade.

Capabilities:
- Count the number of files and directories in a given directory
- Get the size of a directory and its subdirectories
    - Find the largest directories
- Count the number of files groupped by extension
    - Count code files only
- Count lines in a file(s)
- Display a tree of directories and files in a given directory

# Motivation

In the past few months I often needed to get different info about my file systems (Linux, Win, and sometimes OSX). `du`, `df`, `ls`, `wc`, `find` work well but have issues:
- too slow on large volumes.
- don't group/order things the way I needed - even via grep and sort.
- limited functionality.
- not cross-platform.

The alternatives:
- du - too slow and not cross-platform; not the output I needed which made me to pipe it to several other commands.
- [fd](https://github.com/sharkdp/fd) - fast, cross-platform, but is very limited on the info it can extract.

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

For PowerShell, the following command will enable UTF8 and Emoji support. You can add this to your `profile.ps1` file:

```pwsh
[console]::InputEncoding = [console]::OutputEncoding = [System.Text.UTF8Encoding]::new()
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