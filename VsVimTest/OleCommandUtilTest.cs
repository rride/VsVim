﻿using System;
using System.Windows.Input;
using Microsoft.VisualStudio;
using NUnit.Framework;
using Vim;
using VsVim.UnitTest.Utils;

namespace VsVim.UnitTest
{
    [TestFixture()]
    public class OleCommandUtilTest
    {
        internal EditCommand ConvertTypeChar(char data)
        {
            using (var ptr = CharPointer.Create(data))
            {
                EditCommand command;
                Assert.IsTrue(OleCommandUtil.TryConvert(VSConstants.VSStd2K, (uint)VSConstants.VSStd2KCmdID.TYPECHAR, ptr.IntPtr, out command));
                return command;
            }
        }

        private void VerifyConvert(VSConstants.VSStd2KCmdID cmd, VimKey vimKey, EditCommandKind kind)
        {
            VerifyConvert(cmd, KeyInputUtil.VimKeyToKeyInput(vimKey), kind);
        }

        private void VerifyConvert(VSConstants.VSStd2KCmdID cmd, KeyInput ki, EditCommandKind kind)
        {
            EditCommand command;
            Assert.IsTrue(OleCommandUtil.TryConvert(VSConstants.VSStd2K, (uint)cmd, out command));
            Assert.AreEqual(ki, command.KeyInput);
            Assert.AreEqual(kind, command.EditCommandKind);
        }

        private void VerifyConvert(VSConstants.VSStd97CmdID cmd, VimKey vimKey, EditCommandKind kind)
        {
            VerifyConvert(cmd, KeyInputUtil.VimKeyToKeyInput(vimKey), kind);
        }

        private void VerifyConvert(VSConstants.VSStd97CmdID cmd, KeyInput ki, EditCommandKind kind)
        {
            EditCommand command;
            Assert.IsTrue(OleCommandUtil.TryConvert(VSConstants.GUID_VSStandardCommandSet97, (uint)cmd, out command));
            Assert.AreEqual(ki, command.KeyInput);
            Assert.AreEqual(kind, command.EditCommandKind);
        }

        // [Test, Description("Make sure we don't puke on missing data"),Ignore]
        public void TypeCharNoData()
        {
            EditCommand command;
            Assert.IsFalse(OleCommandUtil.TryConvert(VSConstants.GUID_VSStandardCommandSet97, (uint)VSConstants.VSStd2KCmdID.TYPECHAR, IntPtr.Zero, out command));
        }

        // [Test, Description("Delete key"), Ignore]
        public void TypeDelete()
        {
            var command = ConvertTypeChar('\b');
            Assert.AreEqual(Key.Back, command.KeyInput.Key);
        }

        // [Test, Ignore]
        public void TypeChar1()
        {
            var command = ConvertTypeChar('a');
            Assert.AreEqual(EditCommandKind.TypeChar, command.EditCommandKind);
            Assert.AreEqual(Key.A, command.KeyInput.Key);
        }

        // [Test,Ignore]
        public void TypeChar2()
        {
            var command = ConvertTypeChar('b');
            Assert.AreEqual(EditCommandKind.TypeChar, command.EditCommandKind);
            Assert.AreEqual(Key.B, command.KeyInput.Key);
        }

        [Test]
        public void ArrowKeys()
        {
            VerifyConvert(VSConstants.VSStd2KCmdID.LEFT, VimKey.Left, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.LEFT_EXT, VimKey.Left, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.LEFT_EXT_COL, VimKey.Left, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.RIGHT, VimKey.Right, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.RIGHT_EXT, VimKey.Right, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.RIGHT_EXT_COL, VimKey.Right, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.UP, VimKey.Up, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.UP_EXT, VimKey.Up, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.UP_EXT_COL, VimKey.Up, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.DOWN, VimKey.Down, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.DOWN_EXT, VimKey.Down, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.DOWN_EXT_COL, VimKey.Down, EditCommandKind.CursorMovement);
        }

        [Test]
        public void Tab1()
        {
            VerifyConvert(VSConstants.VSStd2KCmdID.TAB, KeyInputUtil.TabKey, EditCommandKind.TypeChar);
        }

        [Test]
        public void F1Help1()
        {
            VerifyConvert(VSConstants.VSStd97CmdID.F1Help, VimKey.F1, EditCommandKind.Unknown);
        }

        [Test]
        public void Escape()
        {
            VerifyConvert(VSConstants.VSStd97CmdID.Escape, KeyInputUtil.EscapeKey, EditCommandKind.Cancel);
            VerifyConvert(VSConstants.VSStd2KCmdID.CANCEL, KeyInputUtil.EscapeKey, EditCommandKind.Cancel);
        }

        [Test]
        public void PageUp()
        {
            VerifyConvert(VSConstants.VSStd2KCmdID.PAGEUP, VimKey.PageUp, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.PAGEUP_EXT, VimKey.PageUp, EditCommandKind.CursorMovement);
        }

        [Test]
        public void PageDown()
        {
            VerifyConvert(VSConstants.VSStd2KCmdID.PAGEDN, VimKey.PageDown, EditCommandKind.CursorMovement);
            VerifyConvert(VSConstants.VSStd2KCmdID.PAGEDN_EXT, VimKey.PageDown, EditCommandKind.CursorMovement);
        }

        [Test]
        public void Backspace()
        {
            VerifyConvert(VSConstants.VSStd2KCmdID.BACKSPACE, VimKey.Back, EditCommandKind.Backspace);
        }


    }
}
