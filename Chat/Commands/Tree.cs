using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SEGarden.Notifications;

namespace SEGarden.Chat.Commands {
    public class Tree : Node {

        private List<Node> Children;
        private String ChildrenInfo;

        public Tree(String word, String shortInfo, String longInfo, int security = 0,
            List<Node> children = null)
            : base(word, shortInfo, longInfo, security) {

            Children = new List<Node>();
            if (children != null)
                foreach (Node child in children)
                    addChild(child);
        }


        public Notifications.Notification Invoke(List<String> inputs, int security) {
            if (security < Security) return NoticeUnAuthorized;
            if (inputs == null) inputs = new List<String>();
            int inputsCount = inputs.Count;
            if (inputsCount == 0) return InfoNotice;

            String childWord = inputs[0];
            inputs.RemoveAt(0);
            List<String> remainingInputs = inputs;

            foreach (Node child in Children) {
                if (child.Matches(childWord))
                    return child.Invoke(remainingInputs, security);
            }

            return InfoNotice;
        }

        public void addChild(Node node) {
            if (Children.Count > 0)
                foreach (Node child in Children)
                    if (child.Word == node.Word)
                        throw new System.InvalidOperationException(
                            "Cannot add multiple commands of word " + node.Word + 
                            " to branch " + Word);

            Children.Add(node as Node);
            node.Parent = this;
        }

        public void addCommand(Command command) {
            addChild(command as Node);
        }
        public void addBranch(Tree branch) {
            addChild(branch as Node);
        }

        protected void refresh(int security) {

            ChildrenInfo = "";
            foreach (Node child in Children) {
                if (security >= child.Security)
                    ChildrenInfo += child.InfoAsChild;
            }

            InfoAsChild = Word + " - " + ShortInfo + "\n" + ChildrenInfo;
            InfoAsTop = FullCommand + "\n" + LongInfo + "\n" + ChildrenInfo;
            InfoNotice = new WindowNotification() {
                Text = InfoAsTop,
                SmallLabel = FullCommand
            };

        }


    }

}
