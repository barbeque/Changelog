# Changelog
A basic tool for extracting commits made since the most recent tag in a git repository.

Based on [libgit2sharp](https://github.com/libgit2/libgit2sharp).

## Usage
 1. Check out a repository containing tags to your local hard drive.
 2. Run `Changelog.exe {your repository path on disk}`
 3. The commits since the most recent tag will be printed to the console.

## Known Issues
 * Does not handle repositories with no tags very well.
 * Does not offer useful command-line arguments (help, etc.)
 * Does not support directly outputting to a file.
 * Does not support custom export of commit information.