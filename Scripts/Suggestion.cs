//
// Copyright (c) Sandro Figo
//
namespace DebugMenu
{
    public class Suggestion
    {
        public Suggestion(string path, string typeText, Node node)
        {
            this.path = path;
            this.typeText = typeText;
            this.node = node;
        }

        public string path;
        public string typeText;
        public Node node;
    }
}