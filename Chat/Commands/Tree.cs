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


        public override Notifications.Notification Invoke(List<String> inputs, int security) {
            if (security < Security) return NoticeUnAuthorized;
            if (inputs == null) inputs = new List<String>();
            int inputsCount = inputs.Count;
            return InfoNotice;
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

        public override void Refresh(int security) {
            RefreshFullCommand();
            InfoAsTop = LongInfo;
            InfoAsChild = FullCommand + " - " + ShortInfo;
            ChildrenInfo = "";

            if (Children.Count > 0) {
                foreach (Node child in Children) {
                    child.Refresh(security);
                    if (security >= child.Security)
                        ChildrenInfo += child.InfoAsChild + "\n";
                }

                InfoAsTop += "\n\nCommands:\n\n" + ChildrenInfo;
                InfoAsChild += "\n" + ChildrenInfo;
            }

            InfoNotice = new WindowNotification() {
                Text = InfoAsTop,
                BigLabel = "Garden Performance",
                SmallLabel = FullCommand
            };
        }


    }

}
