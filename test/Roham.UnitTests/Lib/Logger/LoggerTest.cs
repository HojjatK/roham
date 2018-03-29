using System;
using System.IO;
using NUnit.Framework;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Roham.Lib.Logger
{
    public class LoggerTest 
    {
        [TestFixture]
        [Category("UnitTests.Log.Logger")]
        public class GivenLoggerWithConsoleAppender : UnitTestFixture<ILogger>
        {
            private StringWriter logOut;
            private TextWriter consoleOut;
            private ILogger Subject;

            public GivenLoggerWithConsoleAppender() : base(() => LoggerFactory.GetLogger<LoggerTest>())
            {   
            }

            [TestFixtureSetUp]
            public void OneTimeSetup()
            {
                // init log config
                var hierarchy = (Hierarchy)LogManager.GetRepository();
                var patternLayout = new PatternLayout();
                patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
                patternLayout.ActivateOptions();

                var appender = new ConsoleAppender();
                appender.Layout = patternLayout;
                appender.ActivateOptions();

                hierarchy.Root.AddAppender(appender);
                hierarchy.Root.Level = log4net.Core.Level.All;
                hierarchy.Configured = true;

                // init log output
                logOut = new StringWriter();
                consoleOut = Console.Out;
                Console.SetOut(logOut);

                Subject = CreateSubject();
            }

            [TestFixtureTearDown]
            public void OneTimeTeardown()            
            {
                Console.SetOut(consoleOut);
                logOut.Dispose();
            }

            [Test]
            public void WhenDebugIsCalled_ThenDebugMessageIsLogged()
            {
                Subject.Debug("Test debug message");
                Assert.IsTrue(logOut.ToString().Contains("Test debug message"));
            }

            [Test]
            public void WhenInfoIsCalled_ThenInfoMessageIsLogged()
            {
                Subject.Info("Test info message");
                Assert.IsTrue(logOut.ToString().Contains("Test debug message"));
            }

            [Test]
            public void WhenWarnIsCalled_ThenWarnMessageIsLogged()
            {
                Subject.Warn("Test warn message");
                Assert.IsTrue(logOut.ToString().Contains("Test warn message"));
            }

            [Test]
            public void WhenErrorIsCalled_ThenErrorMessageIsLogged()
            {
                Subject.Error("Test error message", new Exception("test exception message!"));
                Assert.IsTrue(logOut.ToString().Contains("Test error message"));
                Assert.IsTrue(logOut.ToString().Contains("test exception message!"));
            }

            [Test]
            public void WhenFatalIsCalled_ThenFatalMessageIsLogged()
            {
                Subject.Error("Test fatal message", new Exception("test fatal exception message!"));
                Assert.IsTrue(logOut.ToString().Contains("Test fatal message"));
                Assert.IsTrue(logOut.ToString().Contains("test fatal exception message!"));
            }
        }
    }
}