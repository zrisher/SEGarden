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
        private String ChildWords;
        //private Notification UnknownChildNotice;

        public Tree(String word, String shortInfo, String longInfo, int security = 0,
            List<Node> children = null, String alias = null)
            : base(word, shortInfo, longInfo, security, alias) {

            Children = new List<Node>();
            if (children != null)
                foreach (Node child in children)
                    addChild(child);
        }




        public override Notifications.Notification Invoke(List<String> inputs, int security) {
            //Logger.Trace("Invoking " + FullCommand + " with inputs " +
            //    String.Join(", ", inputs), "Invoke");

            if (security < Security) return NoticeUnAuthorized;
            if (inputs == null) inputs = new List<String>();
            int inputsCount = inputs.Count;
            if (inputsCount == 0) return InfoNotice;

            //"Invoking command " + FullCommand + " with inputs " + String.Join(" ", inputs),

            String childWord = inputs[0];
            inputs.RemoveAt(0);
            List<String> remainingInputs = inputs;

            foreach (Node child in Children) {
                //Logger.Trace("Checking if " + childWord + " matches " + child.Word, "Invoke");
                if (child.Matches(childWord)) {
                    //Logger.Trace("It does! Invoking lower command", "Invoke");
                    return child.Invoke(remainingInputs, security);
                }
            }

            return new ChatNotification() {
                Text = "Unknown subcommand \"" + childWord + "\" for " + FullCommand +
                ", did you mean on of these? : " + ChildWords
            };
        }

        public void addChild(Node node) {
            if (node == null) return;

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
            //InfoBadusage = InfoAsTop;
            //String unknownChildText = "Unknown subcommand for " + FullCommand + 
            //    ", did you mean on of these? :"; 
            InfoAsChild = FullCommand + " - " + ShortInfo;
            ChildrenInfo = "";

            if (Children.Count > 0) {
                foreach (Node child in Children) {
                    child.Refresh(security);
                    if (security >= child.Security)
                        ChildrenInfo += child.InfoAsChild + "\n";
                        ChildWords += " " + child.Word;
                }

                InfoAsTop += "\n\nCommands:\n\n" + ChildrenInfo;
                //InfoBadusage += "\n\n Incorrect usage. Commands: ";
                InfoAsChild += "\n" + ChildrenInfo;
            }

            InfoNotice = new WindowNotification() {
                Text = InfoAsTop,
                BigLabel = "Command Tree Info",
                SmallLabel = FullCommand
            };

            /*
            UnknownChildNotice = new ChatNotification() {
                Text = unknownChildText
            };
             * */
        }


    }

}
