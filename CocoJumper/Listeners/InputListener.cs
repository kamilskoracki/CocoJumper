using CocoJumper.Base.Enum;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace CocoJumper.Listeners
{
    public delegate void KeyboardEventDelegate(object oSender, char? key, KeyEventType eventType);

    public class InputListener : IOleCommandTarget, IDisposable
    {
        public KeyboardEventDelegate KeyPressEvent;

        private static readonly uint[] cmdIdsForCancelAction = {
            (uint)VSConstants.VSStd2KCmdID.LEFT,
            (uint)VSConstants.VSStd2KCmdID.UP,
            (uint)VSConstants.VSStd2KCmdID.DOWN,
            (uint)VSConstants.VSStd2KCmdID.RIGHT,
            (uint)VSConstants.VSStd2KCmdID.TAB,
            (uint)VSConstants.VSStd2KCmdID.PAGEDN,
            (uint)VSConstants.VSStd2KCmdID.PAGEUP,
            (uint)VSConstants.VSStd2KCmdID.HOME,
            (uint)VSConstants.VSStd2KCmdID.END,
            (uint)VSConstants.VSStd2KCmdID.CANCEL
        };

        private static readonly uint[] cmdIdsForAcceptSelectionAction = {
            (uint)VSConstants.VSStd2KCmdID.RETURN
        };

        private readonly IVsTextView _adapter;
        private IOleCommandTarget nextCommandHandler;

        public InputListener(IVsTextView adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException($"Argument {nameof(adapter)} in {nameof(InputListener)} was empty");
            _adapter.AddCommandFilter(this, out nextCommandHandler);
        }

        public void Dispose()
        {
            //TODO - as i know we dont need to take care of events when object will be deleted anyway?
            foreach (Delegate eventDelegate in KeyPressEvent.GetInvocationList())
                KeyPressEvent -= (KeyboardEventDelegate)eventDelegate;
            _adapter?.RemoveCommandFilter(this);
            nextCommandHandler = null;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (cmdIdsForCancelAction.Contains(nCmdID))
                KeyPressEvent?.Invoke(this, null, KeyEventType.Cancel);
            else if (cmdIdsForAcceptSelectionAction.Contains(nCmdID))
                KeyPressEvent?.Invoke(this, null, KeyEventType.ConfirmSearching);
            else if (nCmdID == (int)VSConstants.VSStd2KCmdID.BACKSPACE)
                KeyPressEvent?.Invoke(this, null, KeyEventType.Backspace);
            else if (TryGetTypedChar(pguidCmdGroup, nCmdID, pvaIn, out char typedChar))
                KeyPressEvent?.Invoke(this, typedChar, KeyEventType.KeyPress);
            else
                return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            return VSConstants.S_OK;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private static bool TryGetTypedChar(Guid cmdGroup, uint nCmdID, IntPtr pvaIn, out char typedChar)
        {
            typedChar = char.MinValue;
            if (cmdGroup != VSConstants.VSStd2K || nCmdID != (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                return false;
            typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            return true;
        }
    }
}