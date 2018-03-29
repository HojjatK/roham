using System;
using System.Threading;
using Roham.Lib.Logger;
namespace Roham.Lib.WeakEvents
{
    public class GCNotifier
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger<GCNotifier>();

        private static ManualResetEvent garbageCollectedNotificationPending = new ManualResetEvent(true);
        private static WeakEvent<Action> garbageCollectedHandlers = new WeakEvent<Action>(e => e.h, false);

        static GCNotifier()
        {
            Start();
        }

        public static event Action GarbageCollected
        {
            add { garbageCollectedHandlers += value; }
            remove { garbageCollectedHandlers -= value; }
        }

        private bool GarbageCollectionNotificationsAreInProgress => !garbageCollectedNotificationPending.WaitOne(0);

        public static void Start()
        {
            new GCNotifier();
        }

        public static void WaitForPendingGarbageCollectionNotifications()
        {
            garbageCollectedNotificationPending.WaitOne();
        }

        private static void RaiseGarbageCollectedEvent(object state)
        {
            try
            {
                foreach (Action handler in garbageCollectedHandlers.GetInvocationList())
                {
                    try
                    {
                        handler();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error raising GarbageCollected event", ex);
                    }
                }
            }
            finally
            {
                new GCNotifier();
            }
        }

        public GCNotifier()
        {
            garbageCollectedNotificationPending.Set();
        }

        ~GCNotifier()
        {
            if (Environment.HasShutdownStarted)
                return;

#if !SILVERLIGHT
            if (AppDomain.CurrentDomain.IsFinalizingForUnload())
                return;
#endif
            if (!GarbageCollectionNotificationsAreInProgress && garbageCollectedHandlers.GetInvocationList().Length > 0)
            {
                garbageCollectedNotificationPending.Reset();
                ThreadPool.QueueUserWorkItem(RaiseGarbageCollectedEvent);
            }
        }

    }
}
