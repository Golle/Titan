using System.Collections.ObjectModel;
using System.IO;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ManifestViewModel : ViewModelBase
{
    public string Title => "The manifest";
    public ObservableCollection<Node> Items { get; }


    public ManifestViewModel()
    {
        Items = GetSubfolders(@"f:\git\titan");
    }
    public ObservableCollection<Node> GetSubfolders(string strPath)
    {
        ObservableCollection<Node> subfolders = new ObservableCollection<Node>();
        string[] subdirs = Directory.GetDirectories(strPath, "*", SearchOption.TopDirectoryOnly);

        foreach (string dir in subdirs)
        {
            Node thisnode = new Node(dir);

            if (Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly).Length > 0)
            {
                thisnode.Subfolders = new ObservableCollection<Node>();

                thisnode.Subfolders = GetSubfolders(dir);
            }

            subfolders.Add(thisnode);
        }

        return subfolders;
    }

    public class Node
    {
        public ObservableCollection<Node> Subfolders { get; set; }

        public string strNodeText { get; }
        public string strFullPath { get; }

        public Node(string _strFullPath)
        {
            strFullPath = _strFullPath;
            strNodeText = Path.GetFileName(_strFullPath);
        }
    }
}
