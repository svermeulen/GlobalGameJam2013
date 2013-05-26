using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class AssertException : Exception
{
    public string message;

    public AssertException(string message)
    {
        this.message = message;
    }
}

public enum AssertHandleMethod
{
    Exception,
    MessageBox,
    LogAndContinue
}

public class Util
{
    private static bool _shouldStop;
    private static AssertHandleMethod _handleMethod = AssertHandleMethod.MessageBox;

    [Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition)
    {
        if (!condition)
        {
            TriggerAssert("");
        }
    }

    [Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            TriggerAssert(message);
        }
    }

    public static void SetAssertHandleMethod(AssertHandleMethod handleMethod)
    {
        _handleMethod = handleMethod;
    }

    [Conditional("UNITY_EDITOR")]
    private static void TriggerAssert(string message)
    {
        var myTrace = new StackTrace(true);
        var myFrame = myTrace.GetFrame(2);

        var assertMsg = "Assert Hit! \n\n" + message + "\n\n" + "Filename: " + myFrame.GetFileName() + "\nMethod: " + myFrame.GetMethod() +
            "\nLine: " + myFrame.GetFileLineNumber();

#if UNITY_EDITOR
        switch (_handleMethod)
        {
            case AssertHandleMethod.LogAndContinue:
                Debug.LogError(assertMsg);
                break;

            case AssertHandleMethod.MessageBox:

                if (!_shouldStop)
                {
                    var choice = EditorUtility.DisplayDialogComplex("Assert Hit!", assertMsg,
                                                                    "Go To", "Ignore", "Stop");

                    Debug.LogError(assertMsg);
                    if (choice == 0 || choice == 2)
                    {
                        EditorApplication.isPlaying = false;
                        _shouldStop = true;
                    }

                    if (choice == 0)
                    {
                        InternalEditorUtility.OpenFileAtLineExternal(myFrame.GetFileName(), myFrame.GetFileLineNumber());
                    }
                }

                break;

            case AssertHandleMethod.Exception:
                throw new AssertException(assertMsg);

            default:
                Util.Assert(false);
                break;
        }
#endif
    }
}