using System;
using System.Collections;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using System.Collections.Generic;

namespace GitCsCodeAnalyzer
{
    public class GitRepository
    {

        private DirectoryInfo root;
        private Repository repo;

        public GitRepository(DirectoryInfo root)
        {
            this.root = root;
            repo = new Repository(root.FullName);
        }

        public IEnumerable<(string Path, string OldFile, string NewFile)> GetCsFilesNewOrChangedAtHead()
        {
            var head = repo.Head.Commits.First();
            var firstParent = head.Parents.First();

            return repo.Diff.Compare<TreeChanges>(firstParent.Tree, head.Tree)
                .Where(p => p.Path.EndsWith(".cs"))
                .Select(d =>
                    (d.Path,
                    GetBlobContentsOrEmptyString(d.OldOid),
                    GetBlobContentsOrEmptyString(d.Oid)));
        }

        private string GetBlobContentsOrEmptyString(ObjectId blobId)
        {
            if (blobId == null) return string.Empty;
            return repo.Lookup<Blob>(blobId)?.GetContentText() ?? string.Empty;
        }
    }
}