open LibGit2Sharp

let commitsSinceTag (tag : Tag) (repo : Repository) =
    let filter = new CommitFilter (IncludeReachableFrom = tag)
    repo.Commits.QueryBy(filter)

let latestTag (repo : Repository) =
    repo.Tags |> Seq.cast<Tag> 
    |> Seq.maxBy(fun t -> 
        let commit : Commit = downcast t.Target
        commit.Committer.When
    ) // TODO: This is probably not the best way to do it?

[<EntryPoint>]
let main argv = 
    let repo = new Repository(@"C:\code\alpha-tracker")
    let lastTag = latestTag repo
    let commitsSinceTag = commitsSinceTag lastTag repo |> Seq.cast<Commit>
    printfn "%i commits since tag %s" (Seq.length commitsSinceTag) lastTag.FriendlyName
    Seq.iter (printf "%s\n") (commitsSinceTag |> Seq.map(fun c -> c.MessageShort))
    0 // return an integer exit code
