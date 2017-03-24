open LibGit2Sharp
open System.IO
open System

type CommandLineArgs = {
    // Location of the repo on disk
    repoPath: string;
}

let commitsSinceTag (tag : Tag) (repo : Repository) =
    let filter = new CommitFilter (ExcludeReachableFrom = tag, FirstParentOnly = true)
    repo.Commits.QueryBy(filter)

let latestTag (repo : Repository) =
    // TODO: What if tags is empty? This should probably be an option type returned.
    repo.Tags |> Seq.cast<Tag> 
    |> Seq.maxBy(fun t -> 
        let commit : Commit = downcast t.Target
        commit.Committer.When
    ) // TODO: This is probably not the best way to do it?

let parseArgs argv =
    // TODO: make a fancier parser later
    let maybeArgs = 
        match argv with
        | [|arg|]   -> { repoPath = arg }
        | _         -> failwith "Needs path to a checked-out git repository on disk." // TODO: better errors later
    match Directory.Exists maybeArgs.repoPath with
        | true  -> maybeArgs
        | _     -> failwithf "The directory specified (%s) does not exist." maybeArgs.repoPath

let formatCommit (commit : Commit) =
    // TODO: Format each message in markdown bullet points
    // TODO: Embed author name, date in the bullet point
    // TODO: Somehow add a hyperlink from the bullet point to the commit (TBD?)
    commit.MessageShort

let pluralize num singularNoun =
    if num = 1 then singularNoun else singularNoun + "s"

let prependToFile filepath lines =
    let existingLines = if File.Exists(filepath) then File.ReadAllLines(filepath) else [||]
    let newline = [|""|]
    let newLines = List.toArray lines
    let allLines = Array.concat [newLines; newline; existingLines]
    File.WriteAllLines(filepath, allLines)

[<EntryPoint>]
let main argv = 
    try
        let config = parseArgs argv
        let repo = new Repository(config.repoPath)
        let lastTag = latestTag repo
        let commitsSinceTag = commitsSinceTag lastTag repo |> Seq.cast<Commit>
        let numCommits = (Seq.length commitsSinceTag)

        // Generate the header string.
        let header = sprintf "%i %s since tag %s" numCommits (pluralize numCommits "commit") lastTag.FriendlyName
        // Print all the commits to the head of the file.
        ignore (prependToFile "CHANGELOG.md" (header :: Seq.toList (commitsSinceTag |> Seq.map(formatCommit))))
    with
        | Failure msg -> printfn "Failed: %s" msg; Environment.Exit(1) // TODO: usage printer?
    0

// TODO: Prepend to an existing CHANGELOG.md, or create a new one if it does not exist