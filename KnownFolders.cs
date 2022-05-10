using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{
    public enum KnownFolder
    {
        Contacts,
        Downloads,
        Favorites,
        Links,
        SavedGames,
        SavedSearches,
        Desktop,
        Document,
        Music,
        Pictures,
        Videos
    }

    public static class KnownFolders
    {
        private static readonly Dictionary<KnownFolder, Guid> _guids = new()
        {
            [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
            [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
            [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
            [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
            [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
            [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
        };

        private static readonly Dictionary<KnownFolder, string> path = new()
        {
           [KnownFolder.Desktop] = new(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)),
           [KnownFolder.Document] = new(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
           [KnownFolder.Music] = new(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)),
           [KnownFolder.Pictures] = new(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
           [KnownFolder.Videos] = new(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
        };
        private static List<string> allPath = new();

        public static List<string> GetPath()
        {
            foreach (KeyValuePair<KnownFolder, string> entry in path)
            {
                allPath.Add(entry.Value);
            }

            foreach (KeyValuePair<KnownFolder, Guid> entry in _guids)
            {
                allPath.Add(SHGetKnownFolderPath(_guids[entry.Key], 0));
            }
            return allPath;
        }


        [DllImport("shell32",CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, nint hToken = 0);
    }
}
