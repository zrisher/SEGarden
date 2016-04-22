using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEGarden.Logging;

namespace SEGarden.Testing {

    public class SpecCase {

        /*
        struct Assertion {
            public bool Passed;
            public String Description = "";
        }
        */

        public readonly Action<SpecCase> Test;
        public readonly String Description;

        public bool Passed { get; private set; }
        public List<String> FailureMessages { get; private set; }

        //public List<Assertion> Assertions = new List<Assertion>();

        public SpecCase(String description, Action<SpecCase> test) {
            if (description == null)
                throw new ArgumentException("description cannot be null");
            if (test == null)
                throw new ArgumentException("test cannot be null");

            Description = description;
            Test = test;
            FailureMessages = new List<String>();
        }

        public void Run() {
            FailureMessages.Clear();

            try {
                Test.Invoke(this);
            }
            catch (Exception e) {
                FailureMessages.Add("Error - " + e);
            }

            Passed = (FailureMessages.Count == 0);
        }

        /*
        int PassedCount {
            get { return Assertions.Where(x => x.Passed).Count(); }
        }

        int FailedCount {
            get { return FailedAssertions.Count(); }
        }

        List<Assertion> FailedAssertions {
            get { return Assertions.Where(x => !x.Passed).ToList(); }
        }
        */

        public void Assert(bool condition, String description) {
            /*
            Assertions.Add(new Assertion() {
                Passed = condition,
                Description = description
            });
             */

            if (!condition)
                FailureMessages.Add("Failed Assertion - " + description);
 
        }
    }

}
