﻿using System;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Moq;
using NUnit.Framework;
using Vim;
using Vim.UnitTest;

namespace VsVim.UnitTest
{
    [TestFixture]
    public class VsVimHostTest
    {
        private VsVimHost _hostRaw;
        private IVimHost _host;
        private MockRepository _factory;
        private Mock<IVsAdapter> _adapter;
        private Mock<ITextManager> _textManager;
        private Mock<IVsEditorAdaptersFactoryService> _editorAdaptersFactoryService;
        private Mock<ITextBufferUndoManagerProvider> _undoManagerProvider;
        private Mock<_DTE> _dte;
        private Mock<IVsUIShell4> _shell;
        private Mock<StatusBar> _statusBar;

        private void Create()
        {
            _factory = new MockRepository(MockBehavior.Strict);
            _adapter = _factory.Create<IVsAdapter>();
            _undoManagerProvider = _factory.Create<ITextBufferUndoManagerProvider>();
            _editorAdaptersFactoryService = _factory.Create<IVsEditorAdaptersFactoryService>();
            _statusBar = _factory.Create<StatusBar>();
            _shell = _factory.Create<IVsUIShell4>();
            _dte = _factory.Create<_DTE>();
            _dte.SetupGet(x => x.StatusBar).Returns(_statusBar.Object);
            _textManager = _factory.Create<ITextManager>();

            var sp = _factory.Create<SVsServiceProvider>();
            sp.Setup(x => x.GetService(typeof(_DTE))).Returns(_dte.Object);
            sp.Setup(x => x.GetService(typeof(SVsUIShell))).Returns(_shell.Object);
            _hostRaw = new VsVimHost(
                _adapter.Object,
                _undoManagerProvider.Object,
                _editorAdaptersFactoryService.Object,
                _textManager.Object,
                _factory.Create<ITextDocumentFactoryService>().Object,
                sp.Object);
            _host = _hostRaw;
        }


        [TearDown]
        public void TearDown()
        {
            _statusBar = null;
            _dte = null;
            _host = null;
            _hostRaw = null;
        }

        [Test]
        public void GotoDefinition1()
        {
            Create();
            var textView = EditorUtil.CreateView("");
            _textManager.SetupGet(x => x.ActiveTextView).Returns(textView);
            Assert.IsFalse(_host.GoToDefinition());
        }

        [Test]
        public void GotoDefinition2()
        {
            Create();
            var textView = EditorUtil.CreateView("");
            _textManager.SetupGet(x => x.ActiveTextView).Returns(textView);
            _dte.Setup(x => x.ExecuteCommand(VsVimHost.CommandNameGoToDefinition, String.Empty)).Throws(new Exception());
            Assert.IsFalse(_host.GoToDefinition());
        }

        [Test]
        public void GotoDefinition3()
        {
            Create();
            var textView = EditorUtil.CreateView("");
            _textManager.SetupGet(x => x.ActiveTextView).Returns(textView);
            _dte.Setup(x => x.ExecuteCommand(VsVimHost.CommandNameGoToDefinition, String.Empty));
            Assert.IsTrue(_host.GoToDefinition());
        }

        [Test]
        public void GotoDefinition4()
        {
            Create();
            var ct = EditorUtil.GetOrCreateContentType(VsVim.Constants.CPlusPlusContentType, "code");
            var textView = EditorUtil.CreateView(ct, "hello world");
            _textManager.SetupGet(x => x.ActiveTextView).Returns(textView);
            _dte.Setup(x => x.ExecuteCommand(VsVimHost.CommandNameGoToDefinition, "hello"));
            Assert.IsTrue(_host.GoToDefinition());
        }

        [Test]
        [Description("Non-C++ doesn't need the work around")]
        public void GotoDefinition5()
        {
            Create();
            var ct = EditorUtil.GetOrCreateContentType("csharp", "code");
            var textView = EditorUtil.CreateView(ct, "hello world");
            _textManager.SetupGet(x => x.ActiveTextView).Returns(textView);
            _dte.Setup(x => x.ExecuteCommand(VsVimHost.CommandNameGoToDefinition, ""));
            Assert.IsTrue(_host.GoToDefinition());
        }

        [Test]
        public void NavigateTo1()
        {
            Create();
            var buffer = EditorUtil.CreateBuffer("foo", "bar");
            var point = new VirtualSnapshotPoint(buffer.CurrentSnapshot, 2);
            _textManager.Setup(x => x.NavigateTo(point)).Returns(true);
            _host.NavigateTo(new VirtualSnapshotPoint(buffer.CurrentSnapshot, 2));
            _textManager.Verify();
        }

        [Test]
        public void GetName1()
        {
            Create();
            var buffer = new Mock<ITextBuffer>();
            _editorAdaptersFactoryService.Setup(x => x.GetBufferAdapter(buffer.Object)).Returns((IVsTextBuffer)null);
            Assert.AreEqual("", _host.GetName(buffer.Object));
        }

        [Test]
        public void GetName2()
        {
            Create();
            var buffer = new Mock<ITextBuffer>(MockBehavior.Strict);
            var vsTextBuffer = (new Mock<IVsTextLines>(MockBehavior.Strict));
            var userData = vsTextBuffer.As<IVsUserData>();
            var moniker = VsVim.Constants.VsUserDataFileNameMoniker;
            object ret = "foo";
            userData.Setup(x => x.GetData(ref moniker, out ret)).Returns(0);
            _editorAdaptersFactoryService.Setup(x => x.GetBufferAdapter(buffer.Object)).Returns(vsTextBuffer.Object);
            Assert.AreEqual("foo", _host.GetName(buffer.Object));
        }
    }
}
