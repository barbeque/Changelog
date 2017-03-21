open LibGit2Sharp

let getCommitsSinceTag (repo : Repository) (tag : Tag) =
    let filter = new CommitFilter (IncludeReachableFrom = tag)
    repo.Commits.QueryBy(filter)

let findLatestTag (repo : LibGit2Sharp.Repository) =
    repo.Tags |> Seq.cast<Tag> 
    |> Seq.maxBy(fun t -> 
        let commit : Commit = downcast t.Target
        commit.Committer.When
    ) // TODO: This is probably not the best way to do it?

[<EntryPoint>]
let main argv = 
    let repo = new LibGit2Sharp.Repository(@"C:\code\alpha-tracker")
    let lastTag = findLatestTag repo
    let commitsSinceTag = getCommitsSinceTag repo lastTag |> Seq.cast<Commit>
    printfn "%i commits since tag %s" (Seq.length commitsSinceTag) lastTag.FriendlyName
    0 // return an integer exit code
