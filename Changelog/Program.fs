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

[<EntryPoint>]
let main argv = 
    try
        let config = parseArgs argv
        let repo = new Repository(config.repoPath)
        let lastTag = latestTag repo
        let commitsSinceTag = commitsSinceTag lastTag repo |> Seq.cast<Commit>
        printfn "%i commits since tag %s" (Seq.length commitsSinceTag) lastTag.FriendlyName // TODO: hide this print behind a 'verbose' option.
        // Print all the commits to the console. TODO: Make a nice output file so we don't have to pipe.
        Seq.iter (printf "%s\n") (commitsSinceTag |> Seq.map(fun c -> c.MessageShort))
    with
        | Failure msg -> printfn "Failed: %s" msg; Environment.Exit(1) // TODO: usage printer?
    0
