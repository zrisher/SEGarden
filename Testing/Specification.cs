using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Logging;

namespace SEGarden.Testing {

    public class Specification {

        static Logger Log = new Logger("SEGarden.Testing.Specification");

        // Running tests for ... spec subject names ...
        // subject name 1 - 14 tests passed
        // subject name 2 - 10/15 tests passed. Failures:
        //
        //   it should do some awesome stuff
        //     error - error message
        //
        //   it should also do this awesome stuff
        //      failed assertion - assertion message
        //      error - error message
        //     
        public static bool RunSpecTests(
            String domain, List<Specification> specs
        ) {
            if (specs == null)
                throw new ArgumentException("Provided specs are null");

            bool passing = true;
            String specNames = String.Join(", ", specs.Select(x => x.Subject));

            if (String.IsNullOrWhiteSpace(domain))
                Log.Debug("Running Tests for " + specNames, "RunSpecTests");
            else
                Log.Debug("Running Tests for " + domain, "RunSpecTests");

            String logOut = "Test results:";
            foreach (var spec in specs)
                spec.RunDescriptions(ref passing, ref logOut);

            Log.Debug(logOut, "RunSpecTests");

            if (ModInfo.DebugMode) {
                String notificationMsg;
                VRage.Game.MyFontEnum notificationColor;

                if (passing) {
                    notificationMsg = "All Tests passed";
                    notificationColor = VRage.Game.MyFontEnum.Green;
                }
                else {
                    notificationMsg = "Some tests failed";
                    notificationColor = VRage.Game.MyFontEnum.Red;
                }

                if (!String.IsNullOrWhiteSpace(domain))
                    notificationMsg += " for " + domain + ".";

                new Notifications.AlertNotification {
                    Text = notificationMsg,
                    Color = notificationColor,
                    DisplaySeconds = 10
                }.Raise();
            }

            return passing;
        }

        public String Subject { get; protected set; }
        List<SpecCase> Descriptions = new List<SpecCase>();

        protected void Describe(String subject, Action<SpecCase> action) {
            Descriptions.Add(new SpecCase(subject, action));
        }

        protected void RunDescriptions(ref bool passed, ref String logOut) {
            logOut += "\r\n" + Subject + " - ";

            foreach (var description in Descriptions)
                description.Run();

            var failedDescrs = Descriptions.Where(x => !x.Passed).ToList();

            logOut += String.Format(
                "{0}/{1} tests passed.",
                Descriptions.Count - failedDescrs.Count, Descriptions.Count
            );

            if (failedDescrs.Count > 0) {
                logOut += " Failures:\r\n\r\n";
                passed = false;
            }
                
            foreach (var description in failedDescrs) {
                logOut += "  " + description.Description + "\r\n";

                foreach (var failureMsg in description.FailureMessages)
                    logOut += "    " + 
                        failureMsg.Replace("\r\n", "\r\n    ") + "\r\n";

                logOut += "\r\n";
            }
        }

    }

}
