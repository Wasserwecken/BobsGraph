using BobsBuddy.Simulation;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Utility.Logging;
using System;
using System.Reflection;

namespace BobsGraphPlugin
{
    public static class BobsBuddyProvider
    {
        public const string INVOKER_ASSEMBLY = "HearthstoneDeckTracker";
        public const string INVOKER_TYPE = "Hearthstone_Deck_Tracker.BobsBuddy.BobsBuddyInvoker";
        public const string INSTANCE_METHOD = "GetInstance";
        public const string OUTPUT_PROPERTY = "Output";

        private static bool HasInvokerInfos => _invokerType != null && _getInstanceMethod != null && _outputProperty != null;
        private static Type _invokerType;
        private static MethodInfo _getInstanceMethod;
        private static PropertyInfo _outputProperty;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool TryGetTestOutput(int turn, Guid gameId, out TestOutput output)
        {
            output = null;

            // Try get access to the internal buddy invoker class, if not already established
            if (!HasInvokerInfos && !TryGetInvokerInfos(out _invokerType, out _getInstanceMethod, out _outputProperty))
                return false;

            // Gets the current instance that invoked the simulation
            if (!TryGetInvokerInstance(_getInstanceMethod, turn, gameId, out var invoker))
                return false;

            // Reads the results of the simmulation that has been invoked
            output = (TestOutput)_invokerType.GetProperty(OUTPUT_PROPERTY).GetValue(invoker);
            return output != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoker"></param>
        /// <returns></returns>
        private static bool TryGetInvokerInstance(MethodInfo getIntanceMethod, int turn, Guid gameId, out object invoker)
        {
            invoker = getIntanceMethod.Invoke(null, new object[] {gameId, turn, false });
            if (invoker == null)
            {
                Log.Warn($"Cannot get invoker instance of {gameId} {turn}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static bool TryGetInvokerInfos(out Type invokerType, out MethodInfo getInstanceMethod, out PropertyInfo testOutputProperty)
        {
            invokerType = null;
            getInstanceMethod = null;
            testOutputProperty = null;

            var assembly = Assembly.Load(INVOKER_ASSEMBLY);
            if (assembly == null)
            {
                Log.Warn($"Cannot find assembly '{INVOKER_ASSEMBLY}'");
                return false;
            }

            invokerType = assembly.GetType(INVOKER_TYPE);
            if (invokerType == null)
            {
                Log.Warn($"Cannot find invoker '{INVOKER_TYPE}' in '{INVOKER_ASSEMBLY}'");
                return false;
            }

            getInstanceMethod = invokerType.GetMethod(INSTANCE_METHOD, BindingFlags.Public | BindingFlags.Static);
            if (getInstanceMethod == null)
            {
                Log.Warn($"Cannot find method '{INSTANCE_METHOD}' of invoker '{INVOKER_TYPE}' in '{INVOKER_ASSEMBLY}'");
                return false;
            }

            testOutputProperty = invokerType.GetProperty(OUTPUT_PROPERTY);
            if (testOutputProperty == null)
            {
                Log.Warn($"Cannot find property '{OUTPUT_PROPERTY}' of invoker '{INVOKER_TYPE}' in '{INVOKER_ASSEMBLY}'");
                return false;
            }

            return true;
        }
    }
}
