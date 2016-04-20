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
        public static bool RunSpecTests(List<Specification> specs) {
            if (specs == null)
                throw new ArgumentException("Provided specs are null");

            bool passing = true;

            String logOut = "\r\n" + "Running Tests for " +
                 String.Join(", ", specs.Select(x => x.Subject)) + "\r\n";

            foreach (var spec in specs)
                spec.RunDescriptions(ref passing, ref logOut);

            Log.Debug(logOut, "RunSpecTests");

            if (ModInfo.DebugMode) {
                if (passing)
                    new Notifications.AlertNotification {
                        Text = String.Format("All tests passed."),
                        Color = VRage.Game.MyFontEnum.Green,
                        DisplaySeconds = 5
                    }.Raise();
                else
                    new Notifications.AlertNotification {
                        Text = String.Format("Some tests failed, see log."),
                        Color = VRage.Game.MyFontEnum.Red,
                        DisplaySeconds = 5
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
            logOut += Subject + " - ";

            foreach (var description in Descriptions)
                description.Run();

            var failedDescrs = Descriptions.Where(x => !x.Passed).ToList();

            logOut += String.Format(
                "{0}/{1} tests passed.",
                Descriptions.Count - failedDescrs.Count, Descriptions.Count
            );

            if (failedDescrs.Count > 0) {
                logOut += " Failures:\r\n";
                passed = false;
            }
                
            logOut += "\r\n";

            foreach (var description in failedDescrs) {
                logOut += "  " + description.Description + "\r\n";

                foreach (var failureMsg in description.FailureMessages)
                    logOut += "    " + failureMsg + "\r\n";

                logOut += "\r\n";
            }
        }

    }

}
